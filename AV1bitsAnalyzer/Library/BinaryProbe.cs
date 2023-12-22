namespace AV1bitsAnalyzer.Library
{
    internal class BinaryProbe
    {
        public static bool AV1IVF (string path)
        {
            using var fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            using var br = new BinaryReader (fs);
            var v = br.ReadBytes(4);
            var ivf = new byte[] { 0x44, 0x4b, 0x49, 0x46 };
            br.Close();
            fs.Close();
            fs.Close();

            bool equal = v.SequenceEqual(ivf);

            return equal;
        }

        public static bool ObuAnnexB(string path)
        {
            using var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read);
            using var reader = new BinaryReader(fs);
            OBPError error = new();

            bool isAnnexB = true;
            {
                ulong parse_length = 0;
                while ( parse_length <= (ulong) fs.Length )
                {
                    var annexb = reader.ReadBytes(8);
                    if ( annexb.Length == 0 ) break;
                    int ret = AV1Bits.ObpLeb128(annexb, out ulong value, out long consumed, ref error);
                    if ( ret < 0 || value > (1L << 32) || (value > (ulong) fs.Length) )
                    {
                        isAnnexB = false;
                    }
                    parse_length += ((ulong) consumed + value);
                    reader.BaseStream.Position = (long) parse_length;
                }
            }
            return isAnnexB;
        }
    }
}
