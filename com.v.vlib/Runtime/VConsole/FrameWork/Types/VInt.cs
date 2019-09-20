namespace V
{
    public class VInt : VType
    { 
        public System.Type Type
        {
            get
            {
                return typeof(int);
            }
        }

        public string Regex
        {
            get {
                return @"\-?\d+";
            }
        }

        public object ConvertFrom(string regexMatch)
        {
            return System.Convert.ToInt32(regexMatch);
        }

    }
}
