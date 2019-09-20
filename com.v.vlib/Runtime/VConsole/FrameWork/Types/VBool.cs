namespace V
{
    public class VBool : VType
    {
        public string Regex
        {
            get
            {
                return @"(?i)(false|true)|(0|1)(?-i)";
            }
        }

        public System.Type Type
        {
            get
            {
                return typeof(bool);
            }
        }

        public object ConvertFrom(string regexMatch)
        {
            return regexMatch == "1" ? true : regexMatch == "0" ? false : System.Convert.ToBoolean(regexMatch.ToLower());
        }
    }
}
