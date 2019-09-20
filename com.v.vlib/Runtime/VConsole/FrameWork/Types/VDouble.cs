namespace V
{
    public class VDouble : VType
    {
        public string Regex
        {
            get
            {
                return @"-?[0-9]+\.?[0-9]*";
            }
        }

        public System.Type Type
        {
            get
            {
                return typeof(double);
            }
        }

        public object ConvertFrom(string regexMatch)
        {
            return System.Convert.ToDouble(regexMatch, System.Globalization.CultureInfo.InvariantCulture);
        }

    }
}
