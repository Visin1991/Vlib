namespace V
{
    public class VDecimal
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
                return typeof(decimal);
            }
        }

        public object ConvertFrom(string regexMatch)
        {
            return System.Convert.ToDecimal(regexMatch, System.Globalization.CultureInfo.InvariantCulture);
        }
    }
}
