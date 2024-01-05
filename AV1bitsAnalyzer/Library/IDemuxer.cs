using System.Security.Policy;

namespace AV1bitsAnalyzer.Library
{
    public delegate void FrameCallbackHandle (ref FramesInfo v);
    public enum ParseType {
        ParseType_IVF,
        ParseType_OBU_Section5,
        ParseType_OBU_AnnexB,
    }

    struct OBU
    {
        OBUType type;
        int obuOffset;
        int obuDataOffset;
        int size;
        int tid;
        int sid;

        public OBUType Type { readonly get => type; set => type = value; }
        public int ObuOffset { readonly get => obuOffset; set => obuOffset = value; }
        public int Size { readonly get => size; set => size = value; }
        public int Tid { readonly get => tid; set => tid = value; }
        public int Sid { readonly get => sid; set => sid = value; }
        public int ObuDataOffset { get => obuDataOffset; set => obuDataOffset = value; }

        public string [] Strings ()
        {
            return [$"{Type}", $"{ObuOffset}", $"{Size}", $"{Tid}", $"{Sid}"];
        }
    }
    public class FramesInfo
    {
        int frameIdx = 0;
        string frametype = string.Empty;
        long address;
        List<OBU> obus = [];
        byte[] data = [];

        float progress;

        public long Address { get => address; set => address = value; }
        public byte[] Data { get => data; set => data = value; }
        public float Progress { get => progress; set => progress = value; }
        public int FrameIdx { get => frameIdx; set => frameIdx = value; }
        public string Frametype { get => frametype; set => frametype = value; }
        internal List<OBU> Obus { get => obus; set => obus = value; }

        public override string ToString ()
        {
            return $"Frame {FrameIdx:D8} @ {Address:X8}         {Frametype}     {Data.Length} ";
        }
    }

    class AV1Demuxer
    {
        private bool _exit;
        private Thread? _thread;
        private FrameCallbackHandle? _callback;
        public bool Exit { get => _exit; set => _exit = value; }

        public void Parse (string name, ParseType t, FrameCallbackHandle delegateHanlde)
        {
            if ( !File.Exists(name) ) { return; }

            _callback = delegateHanlde;
            _exit = false;
            _thread = new Thread(() =>
            {
                switch ( t )
                {
                    case ParseType.ParseType_IVF:
                        ParseIVF(name);
                        break;
                    case ParseType.ParseType_OBU_Section5:
                        ParseSection5(name);
                        break;
                    case ParseType.ParseType_OBU_AnnexB:
                        ParseAnnexB(name);
                        break;
                }
            });
            _thread.Start();
        }

        private void ParseIVF (string name)
        {
            long consumerLength;
            using var fs = new FileStream(name, FileMode.Open, FileAccess.Read, FileShare.Read);
            using var reader = new BinaryReader(fs);
            long fsLength = fs.Length;
            OBPError error = new();
            OBUType obuType = 0;

            consumerLength = 0x20; // skip ivf header
            int readLength = 12, frameIdx = 0;

            while ( !_exit && consumerLength < fsLength )
            {
                reader.BaseStream.Position = consumerLength;
                var header = reader.ReadBytes(readLength);
                if ( header.Length != readLength )
                {
                    break;
                }
                consumerLength += readLength;

                long packetSize = 0;
                {
                    packetSize = header[0] << 0 | header[1] << 8 | header[2] << 16 | header[3] << 24;
                }

                var v = reader.ReadBytes((int)packetSize);
                var s = new Span<byte>(v);

                int tid = 0, sid = 0, size = 0;

                FramesInfo info = new()
                {
                    Address = consumerLength,
                    Data = v,
                    FrameIdx = frameIdx++,
                    Progress = consumerLength * 1.0f / fsLength,
                };
                List<OBU> Obus = info.Obus;

                long offset = 0;
                long pLength = 0;
                while ( pLength < s.Length )
                {
                    var res = ObuOperator.ObpGetNextObu(s[(int)pLength..], out obuType, out offset, out size, out tid, out sid, ref error);
                    if ( res < 0 )
                    {
                        pLength = s.Length;
                        break;
                    }
                    int obulen = (int)offset + size;
                    OBU oBU = new ()
                    {
                        Type = obuType,
                        ObuOffset = (int)pLength,
                        ObuDataOffset = (int)offset,
                        Sid = sid,
                        Tid = tid,
                        Size = size,
                    };
                    Obus.Add(oBU);

                    pLength += obulen;
                }
                _callback?.Invoke(ref info);

                consumerLength += pLength;
            }

            FramesInfo end = new()
            {
                Address = long.MaxValue,
                Progress = 1.0f,
            };

            _callback?.Invoke(ref end);

            reader.Close();
            fs.Close();
        }

        private void ParseAnnexB (string name)
        {
            using var fs = new FileStream(name, FileMode.Open, FileAccess.Read, FileShare.Read);
            using var reader = new BinaryReader(fs);

            long fsLength = reader.BaseStream.Length;
            OBUType obuType = 0;
            OBPError error = new();
            int readLength = 12;
            long fs_reader_Length = 0;
            long offset = 0;
            int tid = 0, sid = 0, size = 0, frameIdx = 0;

            while ( !_exit && fs_reader_Length < fsLength )
            {
                reader.BaseStream.Position = fs_reader_Length;
                var readArray = reader.ReadBytes(readLength);
                Span<byte> rbtry = new(readArray);

                int ret = AV1Bits.ObpLeb128(rbtry, out ulong value, out long consumed, ref error);
                fs_reader_Length += consumed;

                int readLen = (int)Math.Min(value, (ulong)fsLength);
                reader.BaseStream.Position = fs_reader_Length;
                readArray = reader.ReadBytes(readLen);

                int temporal_unit_size = Math.Min((int) value, readArray.Length);
                var temporalSpan = new Span<byte>(readArray);

                FramesInfo info = new()
                {
                    Address = fs_reader_Length,
                    Data = readArray,
                    FrameIdx = frameIdx ++,
                    Progress = fs_reader_Length*1.0f / fsLength,
                };
                var obus = info.Obus;

                long temporal_parse_length = 0;
                while ( temporal_parse_length < temporal_unit_size )
                {
                    ret = AV1Bits.ObpLeb128(temporalSpan[(int) temporal_parse_length..], out ulong value2, out long consumed2, ref error);
                    temporal_parse_length += consumed2;

                    long frame_unit_size = Math.Min((long)value2, temporal_unit_size-temporal_parse_length);

                    var frameSpan = temporalSpan[(int)temporal_parse_length..];
                    long frame_parse_length = 0;
                    while ( frame_parse_length < frame_unit_size )
                    {
                        ret = AV1Bits.ObpLeb128(frameSpan[(int) frame_parse_length..], out ulong value3, out long consumed3, ref error);
                        frame_parse_length += consumed3;

                        long obu_length = Math.Min((long)value3, frame_unit_size - frame_parse_length);

                        if ( obu_length >= 0 )
                        {
                            var res = ObuOperator.ObpGetNextObu(frameSpan[(int)frame_parse_length..], out obuType, out offset, out size, out tid, out sid, ref error);

                            if ( res >= 0 && ObuOperator.ObpIsValidObu(obuType))
                            {
                                OBU oBU = new ()
                                {
                                    Type = obuType,
                                    ObuOffset = (int)(temporal_parse_length + frame_parse_length),
                                    ObuDataOffset = (int)offset,
                                    Sid = sid,
                                    Tid = tid,
                                    Size = (int)obu_length,
                                };
                                obus.Add(oBU);
                            }
                        }
                        else
                        {
                            obu_length = frame_unit_size - frame_parse_length; // skip rest data
                        }

                        frame_parse_length += obu_length;
                    }
                    temporal_parse_length += frame_unit_size;
                }
                _callback?.Invoke(ref info);
                fs_reader_Length += temporal_parse_length;
            }

            FramesInfo end = new()
            {
                Address = long.MaxValue,
                Progress = 1.0f,
            };

            _callback?.Invoke(ref end);
        }

        private void ParseSection5 (string name)
        { 
            using var fs = new FileStream(name, FileMode.Open, FileAccess.Read, FileShare.Read);
            using var reader = new BinaryReader(fs);

            long fsLength = reader.BaseStream.Length;
            List<OBU> rawObus = [];

            {
                int readLength = 12, pre_frame_size = 0;
                long fs_reader_Length = 0;
                while ( !_exit && fs_reader_Length < fsLength )
                {
                    reader.BaseStream.Position = fs_reader_Length;
                    var header = reader.ReadBytes(readLength);
                    if ( header.Length < 2 )
                    {
                        break;
                    }

                    long offset = 0;
                    int tid = 0, sid = 0, size = 0;
                    OBPError error = new();
                    OBUType obuType = 0;

                    var s = new Span<byte>(header);
                    var res = ObuOperator.ObpGetNextObu(s, out obuType, out offset, out size, out tid, out sid, ref error);
                    if ( res < 0 )
                    {
                        break;
                    }

                    if ( obuType == OBUType.OBU_TEMPORAL_DELIMITER )
                    {
                        pre_frame_size = 0;
                    }

                    long frame_size = offset + size;

                    OBU oBU = new ()
                    {
                        Type = obuType,
                        ObuOffset = pre_frame_size,
                        ObuDataOffset = (int)offset,
                        Sid = sid,
                        Tid = tid,
                        Size = size,
                    };
                    pre_frame_size += (int) frame_size;
                    rawObus.Add(oBU);

                    fs_reader_Length += frame_size;
                }
            }

            {
                int frameIdx = 0;
                long fs_reader_Length = 0;
                reader.BaseStream.Position = fs_reader_Length;

                FramesInfo info = new()
                {
                    Address = fs_reader_Length,
                    FrameIdx = frameIdx++,
                    Progress = fs_reader_Length * 1.0f / fsLength,
                };
                var obus = info.Obus;
                var zero = rawObus.ElementAt(0);
                int frame_length = zero.ObuDataOffset + zero.Size;
                obus.Add(zero);
                rawObus.Remove(zero);

                foreach ( var obu in rawObus )
                {
                    if ( obu.Type == OBUType.OBU_TEMPORAL_DELIMITER )
                    {
                        var d = reader.ReadBytes(frame_length);
                        info.Data = d;
                        _callback?.Invoke(ref info);

                        fs_reader_Length += frame_length;
                        
                        info = new()
                        {
                            Address = fs_reader_Length,
                            FrameIdx = frameIdx++,
                            Progress = fs_reader_Length * 1.0f / fsLength,
                        };
                        obus = info.Obus;
                        frame_length = 0;
                    }
                    obus.Add(obu);
                    frame_length += obu.Size + obu.ObuDataOffset;
                }

            }

            FramesInfo end = new()
            {
                Address = long.MaxValue,
                Progress = 1.0f,
            };

            _callback?.Invoke(ref end);
        }

        public void Stop ()
        {
            _exit = true;
            _thread?.Join();
        }

    }
}
