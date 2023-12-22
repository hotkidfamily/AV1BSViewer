namespace AV1bitsAnalyzer.Library
{
    public delegate void FrameCallbackHandle (ref FramesInfo v);
    public enum ParseType {
        ParseType_IVF,
        ParseType_OBU_Section5,
        ParseType_OBU_AnnexB,
    }
    public class FramesInfo
    {
        long address;
        OBUType type;
        long offset;
        int size;
        int tid;
        int sid;
        byte[]? data;

        float progress;
        string pkgType = "-";
        string pktHeader = string.Empty;

        public long Address { get => address; set => address = value; }
        public OBUType Type { get => type; set => type = value; }
        public long Offset { get => offset; set => offset = value; }
        public int Size { get => size; set => size = value; }
        public int Tid { get => tid; set => tid = value; }
        public int Sid { get => sid; set => sid = value; }
        public byte[]? Data { get => data; set => data = value; }
        public float Progress { get => progress; set => progress = value; }
        public string Pkgtype { get => pkgType; set => pkgType = value; }
        public string PkgHeader { get => pktHeader; set => pktHeader = value; }

        public string[] Info ()
        {
            return [$"{Address:X8}", $"{Offset}", $"{Type}", $"{Size}", $"{Tid}", $"{Sid}", $"{Pkgtype}"];
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
            long offset = 0;
            int tid = 0, sid = 0, size = 0;
            OBPError error = new();
            OBUType obuType = 0;

            consumerLength = 0x20; // skip ivf header
            int readLength = 12;

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

                offset = 0;

                long pLength = 0;
                while ( pLength < s.Length )
                {
                    var res = ObuOperator.ObpGetNextObu(s[(int)pLength..], out obuType, out offset, out size, out tid, out sid, ref error);
                    if ( res < 0 )
                    {
                        pLength = s.Length;
                        break;
                    }

                    var add = consumerLength  + pLength;
                    FramesInfo info = new()
                    {
                        Address = add,
                        Type = obuType,
                        Offset = offset,
                        Size = size,
                        Tid = tid,
                        Sid = sid,
                        Data = s.Slice((int)pLength,(int)offset+size).ToArray(),
                        Progress = add*1.0f / fsLength,
                    };

                    _callback?.Invoke(ref info);
                    pLength += offset + size;
                }
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
            int tid = 0, sid = 0, size = 0;

            while ( !_exit && fs_reader_Length < fsLength )
            {
                reader.BaseStream.Position = fs_reader_Length;
                var readArray = reader.ReadBytes(readLength);
                Span<byte> rbtry = new(readArray);

                int ret = AV1Bits.ObpLeb128(rbtry, out ulong value, out long consumed, ref error);
                fs_reader_Length += consumed;

                int readLen = (int)Math.Min(value, (ulong)fsLength);
                reader.BaseStream.Position = fs_reader_Length;
                readArray = reader.ReadBytes((int) readLen);

                int temporal_unit_size = Math.Min((int) value, readArray.Length);
                var temporalSpan = new Span<byte>(readArray);
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
                            if ( res < 0 )
                            {
                                break;
                            }
                            var valid = ObuOperator.ObpIsValidObu(obuType);
                            if ( valid )
                            {
                                var add = fs_reader_Length + temporal_parse_length + frame_parse_length;
                                FramesInfo info = new()
                                {
                                    Address = add,
                                    Type = obuType,
                                    Offset = offset,
                                    Size = (int)(obu_length - offset),
                                    Tid = tid,
                                    Sid = sid,
                                    Data = frameSpan.Slice((int) frame_parse_length, (int) obu_length).ToArray(),
                                    Progress = add*1.0f / fsLength,
                                };

                                _callback?.Invoke(ref info);
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

            long offset = 0;
            int tid = 0, sid = 0, size = 0;
            OBPError error = new();
            OBUType obuType = 0;
            int readLength = 12;
            long fs_reader_Length = 0;
            while ( !_exit && fs_reader_Length < fsLength )
            {
                reader.BaseStream.Position = fs_reader_Length;
                var header = reader.ReadBytes(readLength);
                if ( header.Length < 2 )
                {
                    break;
                }
                var s = new Span<byte>(header);
                var res = ObuOperator.ObpGetNextObu(s, out obuType, out offset, out size, out tid, out sid, ref error);
                if ( res < 0 )
                {
                    break;
                }

                reader.BaseStream.Position = fs_reader_Length;
                long obuSize = offset + size;
                var data = reader.ReadBytes((int)obuSize);
                FramesInfo info = new()
                {
                    Address = fs_reader_Length,
                    Type = obuType,
                    Offset = offset,
                    Size = size,
                    Tid = tid,
                    Sid = sid,
                    Data = data,
                    Progress = fs_reader_Length*1.0f / fsLength,
                };

                _callback?.Invoke(ref info);
                fs_reader_Length += obuSize;
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
