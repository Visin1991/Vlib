namespace V
{
    public class VFloat : VType
    {
        public string Regex
        {
            get
            {
                return @"-?[0-9]+(\.[0-9]+)?";
            }
        }

        public System.Type Type
        {
            get
            {
                return typeof(float);
            }
        }

        public object ConvertFrom(string regexMatch)
        {
            return System.Convert.ToSingle(regexMatch, System.Globalization.CultureInfo.InvariantCulture);
        }

    }
}
