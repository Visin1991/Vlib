
namespace V
{
    public class VString : VType
    {
        public string Regex
        {
            get
            {
                return "\"[^\"]*\"";
            }
        }

        public System.Type Type
        {
            get
            {
                return typeof(string);
            }
        }

        public object ConvertFrom(string regexMatch)
        {
            return regexMatch.Substring(1, regexMatch.Length - 2);
        }

    }

}