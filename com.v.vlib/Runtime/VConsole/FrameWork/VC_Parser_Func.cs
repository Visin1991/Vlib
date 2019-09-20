using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Reflection;
using System;


namespace V
{

    public class VC_ParserResult_Func
    {
        public VC_ParserResult_Func(object[] _result, string[] _errors)
        {
            parameters = _result;
            errors = _errors;
        }

        public string[] errors { get; private set; }
        public object[] parameters { get; private set; }

        public bool Valid
        {
            get {
                return errors == null || errors.Length == 0;
            }
        }
    }

    public static class VC_Parser_Func
    {
        public static VC_ParserResult_Func Paser(VC_PaserResult_Cmd result, ParameterInfo[] paramterInfos)
        {
            string cmd = result.parameters;

            List<string> errors = new List<string>();
            List<object> convertedParameters = new List<object>();

            foreach (ParameterInfo paramInfo in paramterInfos)
            {
                VType vType = VTypeCollection.Get(paramInfo.ParameterType);
                if (vType == null)
                {
                    errors.Add(string.Format("No converter found for type: {0}", paramInfo.ParameterType.FullName));
                    break;
                }

                cmd = cmd.Trim();
                Match ma = Regex.Match(cmd, string.Format("^{0}", vType.Regex));

                if (String.IsNullOrWhiteSpace(ma.Value))
                {
                    if (paramInfo.IsOptional)
                    {
                        convertedParameters.Add(paramInfo.DefaultValue);
                        continue;
                    }
                    errors.Add("Invalid parameters");
                    break;
                }
                else
                {
                    try
                    {
                        convertedParameters.Add(vType.ConvertFrom(ma.Value));
                    }
                    catch(Exception ex)
                    {
                        errors.Add(string.Format("Failed to convert '{0}' to type {1}.{2}{3}", ma.Value, paramInfo.ParameterType.Name, Environment.NewLine, ex.Message));
                        break;
                    }

                    //
                    cmd = cmd.Substring(ma.Length);
                }
            }

            if (cmd.TrimEnd().Length > 0 && errors.Count == 0)
            {
                errors.Add("Provided parameters do not match with the expected parameters.");
            }


            return new VC_ParserResult_Func(convertedParameters.ToArray(), errors.ToArray());
        }
    }
}
