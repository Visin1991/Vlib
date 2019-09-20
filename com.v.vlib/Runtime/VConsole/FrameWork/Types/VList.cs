using System.Reflection;

namespace V
{
    public class VList : VType
    {
        VType parser;

        public VList(VType _paser)
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
                return typeof(System.Collections.Generic.List<>).MakeGenericType(parser.Type);
            }
        }

        public object ConvertFrom(string regexMatch)
        {
            var matches = System.Text.RegularExpressions.Regex.Matches(regexMatch, parser.Regex);

            object list = System.Activator.CreateInstance(Type);
            for (int i = 0; i < matches.Count; i++)
            {
                list.GetType().InvokeMember("Add", BindingFlags.InvokeMethod | BindingFlags.Instance | BindingFlags.Public, null, list, new object[] { parser.ConvertFrom(matches[i].Value) });
            }

            return list;
        }
    }
}