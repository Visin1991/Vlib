namespace V
{
    public class VUint : VType
    {
        public string Regex
        {
            get
            {
                return @"\d+";
            }
        }

        public System.Type Type
        {
            get
            {
                return typeof(uint);
            }
        }

        public object ConvertFrom(string regexMatch)
        {
            return System.Convert.ToUInt32(regexMatch);
        }
    }
}
