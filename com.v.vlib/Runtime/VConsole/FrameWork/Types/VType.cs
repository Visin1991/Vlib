using System;
using System.Collections.Generic;
using System.Linq;

namespace V
{
    public static class VTypeCollection
    {
        private static Dictionary<Type, VType> vTypeDictionary;
        public static void Init()
        {
            vTypeDictionary = new Dictionary<Type, VType>();
            AddType(new VByte());
            AddType(new VBool());
            AddType(new VInt());
            AddType(new VFloat());
            AddType(new VDouble());
            AddType(new VChar());
            AddType(new VString());
            AddType(new VVector2());
            AddType(new VVector3());
            AddType(new VVector4());

            vTypeDictionary.ToList().ForEach(t => AddType(new VArray(t.Value)));
        }

        public static void Add(VType vType)
        {
            VType val;
            if (vTypeDictionary.TryGetValue(vType.Type, out val))
            {
                string error = string.Format("There is already a converter for the type {0}. Use the replace method to replace the default one.", vType.Type.FullName);
                //
                return;
            }

            AddType(vType);
        }

        public static VType Get(Type type)
        {
            VType val;
            vTypeDictionary.TryGetValue(type, out val);
            return val;
        }
        private static void AddType(VType vType)
        {
            vTypeDictionary[vType.Type] = vType;
        }
    }

    public interface VType
    {
        Type Type { get; }
        string Regex { get; }
        object ConvertFrom(string regexMatch);
    }

}