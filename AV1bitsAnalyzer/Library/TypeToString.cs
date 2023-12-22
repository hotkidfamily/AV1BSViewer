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
    }
}
