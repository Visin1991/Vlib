﻿using System;
using System.Linq;
using UnityEngine;

namespace V
{
    public class VVector4 : VType
    {
        public string Regex
        {
            get
            {
                return @"{(\s*-?[0-9]+(\.[0-9]+)?\s*){4}}";
            }
        }

        public Type Type
        {
            get
            {
                return typeof(Vector4);
            }
        }

        public object ConvertFrom(string match)
        {
            var m = System.Text.RegularExpressions.Regex.Match(match, @"(\s*-?[0-9]+(\.[0-9]+)?\s*){3}");
            var values = System.Text.RegularExpressions.Regex.Split(m.Value, @"\s+")
                .Where(s => !string.IsNullOrEmpty(s))
                .Select(s => Convert.ToSingle(s))
                .ToArray();
            return new Vector4(values[0], values[1], values[2],values[3]);
        }

    }
}