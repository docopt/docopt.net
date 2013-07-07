namespace DocoptNet
{
    public struct StringPartition
    {
        public string LeftString;
        public string Separator;
        public string RightString;

        /// <summary>
        /// Split the <paramref name="stringToPartition"/> at the first occurrence of <paramref name="separator"/>, and stores the part before the separator,
        /// the separator itself, and the part after the separator. If the separator is not found, stores the string itself, and
        /// two empty strings.
        /// </summary>
        /// <param name="stringToPartition"></param>
        /// <param name="separator"></param>
        public StringPartition(string stringToPartition, string separator)
        {
            LeftString = stringToPartition;
            Separator = "";
            RightString = "";

            var i = stringToPartition.IndexOf(separator, System.StringComparison.Ordinal);
            if (i > 0)
            {
                LeftString = stringToPartition.Substring(0, i);
                Separator = separator;
                RightString = stringToPartition.Substring(i + separator.Length);
            }
        }

        public bool NoSeparatorFound 
        {
            get { return Separator=="" && RightString == ""; }
        }
    }
}