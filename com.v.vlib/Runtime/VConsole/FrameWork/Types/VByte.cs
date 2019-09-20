namespace V
{
    public class VByte : VType
    {
        public string Regex
        {
            get
            {
                return @"[0-2]\d{0,2}";
            }
        }

        public System.Type Type
        {
            get
            {
                return typeof(byte);
            }
        }

        public object ConvertFrom(string regexMatch)
        {
            return System.Convert.ToByte(regexMatch);
        }

    }
}
