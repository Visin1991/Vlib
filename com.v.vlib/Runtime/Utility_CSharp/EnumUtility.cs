using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace V
{
    public class Enum
    {
        public static List<T> GetListFromEnum<T>()
        {
            List<T> enumList = new List<T>();
            System.Array enums = System.Enum.GetValues(typeof(T));
            foreach (T e in enums)
            {
                enumList.Add(e);
            }
            return enumList;
        }

        public static List<string> GetListStringFromEnum<T>()
        {
            List<string> enumList = new List<string>();
            System.Array enums = System.Enum.GetValues(typeof(T));
            foreach (T e in enums)
            {
                enumList.Add(e.ToString());
            }
            return enumList;
        }

    }
}
