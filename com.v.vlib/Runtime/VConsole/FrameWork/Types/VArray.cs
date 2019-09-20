namespace V
{
    public class VArray : VType
    {
        VType parser;

        public VArray(VType _paser)
        {
            parser = _paser;
        }

        public string Regex
        {
            get
            {
                return string.Format(@"\s*\[(\s*{0}\s*)*\]\s*", parser.Regex);
            }
        }

        public System.Type Type
        {
            get
            {
                return parser.Type.MakeArrayType();
            }
        }

        public object ConvertFrom(string regexMatch)
        {
            var matches = System.Text.RegularExpressions.Regex.Matches(regexMatch, parser.Regex);
            var arr = System.Array.CreateInstance(parser.Type, matches.Count);

            for (int i = 0; i < matches.Count; i++)
            {
                arr.SetValue(parser.ConvertFrom(matches[i].Value), i);
            }
            return arr;
        }

    }
}
