using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;


namespace V
{
    [AttributeUsage(AttributeTargets.Method,AllowMultiple = false,Inherited = false)]
    public class VC_FunctionAttribute : Attribute
    {
        public string Alias { get;set; }

        public string Usage { get; set; }

        public string Description { get; set; }
    }

    public class VC_FunctionInfo
    {
        public static Dictionary<string, VC_FunctionInfo> functionInfos = new Dictionary<string, VC_FunctionInfo>();
        private VC_FunctionInfo() { }

        internal VC_FunctionInfo(VC_FunctionAttribute attr,MethodInfo method) {
            Alias = attr.Alias;
            Usage = attr.Usage;
            Description = attr.Description;
            Method = method;
        }

        public string Alias { get; private set; }
        public string Usage { get; private set; }
        public string Description { get; private set; }
        internal MethodInfo Method { get; private set; }

        public static void AddClassType(Type[] types)
        {
            types = types.Distinct().ToArray();
            for (int i = 0; i < types.Length; i++)
            {
                Type t = types[i];
                MethodInfo[] allMethods = t.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static);
                MethodInfo[] validMethods = allMethods.Where(m => m.GetCustomAttributes(typeof(VC_FunctionAttribute), false).Length > 0).ToArray();
                foreach (MethodInfo method in validMethods)
                {
                    VC_FunctionAttribute cmdAttr = method.GetCustomAttribute(typeof(VC_FunctionAttribute), false) as VC_FunctionAttribute;

                    string alias = string.Empty;

                    if (string.IsNullOrWhiteSpace(cmdAttr.Alias))
                    {
                        alias = string.Format("{0}.{1}", t.Name, method.Name);
                        string error = string.Format("Unnamed command!{0}Generated alias: {1}", Environment.NewLine, alias);
                    }
                    else
                    {
                        alias = cmdAttr.Alias;
                    }

                    alias = alias.Trim();

                    if (functionInfos.ContainsKey(alias))
                    {
                        cmdAttr.Alias = cmdAttr.Alias == alias ? string.Format("{0}.{1}", t.Name, method.Name) : string.Format("{0}.{1}", t.FullName, method.Name);
                        string error = string.Format("{0} is already defined and has been replaced with {1}", alias, cmdAttr.Alias);
                        //RTConsole.Instance.LogInternal(LogTypes.Warning, "Commands provider", error);
                    }
                    else
                    {
                        cmdAttr.Alias = alias;
                    }

                    functionInfos.Add(cmdAttr.Alias.Trim(), new VC_FunctionInfo(cmdAttr, method));
                }
            }
        }
    }
}
