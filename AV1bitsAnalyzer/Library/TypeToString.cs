using System.Text;

namespace AV1bitsAnalyzer.Library
{
    internal class TypeToString
    {
        public static string ByteArray (byte[] b)
        {
            string d = string.Join(",", b.Select( c => c.ToString()));
            return d;
        }

        public static string UintArray (uint[] b)
        {
            string d = string.Join(",", b.Select( c => c.ToString()));
            return d;
        }

        public static string IntArray (int[] b)
        {
            string d = string.Join(",", b.Select( c => c.ToString()));
            return d;
        }

        public static string Bits8 (byte b)
        {
            StringBuilder sb = new();
            for ( int i = 0; i <= 7; i++ )
            {
                byte mask = (byte) (b & (byte)(1 << i));
                if(mask != 0 )
                {
                    sb.Append($"{i},");
                }
            }
            return sb.ToString();
        }
    }
}
