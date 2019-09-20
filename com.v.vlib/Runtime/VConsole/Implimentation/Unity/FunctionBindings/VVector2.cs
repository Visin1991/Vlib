using System;
using System.Linq;
using UnityEngine;

namespace V
{
    public class VVector2 : VType
    {
        public string Regex
        {
            get
            {
                return @"{(\s*-?[0-9]+(\.[0-9]+)?\s*){2}}";
            }
        }

        public Type Type
        {
            get
            {
                return typeof(Vector2);
            }
        }

        public object ConvertFrom(string regexMatch)
        {
            var m = System.Text.RegularExpressions.Regex.Match(regexMatch, @"(\s*-?[0-9]+(\.[0-9]+)?\s*){2}");
            var values = System.Text.RegularExpressions.Regex.Split(m.Value, @"\s+")
                .Where(s => !string.IsNullOrEmpty(s))
                .Select(s => Convert.ToSingle(s))
                .ToArray();
            return new Vector2(values[0], values[1]);
        }
    }

}