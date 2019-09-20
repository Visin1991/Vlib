namespace V
{
    public class VChar : VType
    {
        public string Regex
        {
            get
            {
                return @"\'.\'";
            }
        }

        public System.Type Type
        {
            get
            {
                return typeof(char);
            }
        }

        public object ConvertFrom(string regexMatch)
        {
            return System.Convert.ToChar(regexMatch.Trim('\''));
        }
    }

}