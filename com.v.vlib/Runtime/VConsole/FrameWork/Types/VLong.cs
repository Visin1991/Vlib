
namespace V
{
    public class VLong : VType
    {
        public string Regex
        {
            get
            {
                return @"\-?\d+";
            }
        }

        public System.Type Type
        {
            get
            {
                return typeof(long);
            }
        }

        public object ConvertFrom(string regexMatch)
        {
            return System.Convert.ToInt64(regexMatch);
        }
    }
}
