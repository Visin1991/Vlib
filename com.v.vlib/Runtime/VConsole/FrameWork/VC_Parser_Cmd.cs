using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

namespace V
{
    public struct VC_PaserResult_Cmd
    {
        public string functionAlia;
        public string parameters;
        public string[] errors;

        public VC_PaserResult_Cmd(string _commandName,string _paramters,string[] _errors = default(string[]))
        {
            functionAlia = _commandName;
            parameters = _paramters;
            errors = _errors;
        }
        public bool Valid {
            get {
                return errors == null ||errors.Length == 0;
            }
        }
    }

    public static class VC_Parser_Cmd
    {
        private static List<string> errors = new List<string>();
        public static VC_PaserResult_Cmd Paser(string command)
        {
            if (string.IsNullOrEmpty(command))
            {
                errors.Add("No command specified");
                return new VC_PaserResult_Cmd(string.Empty,string.Empty,errors.ToArray());
            }

            //Paser for parameter
            var token = command.Split(new[] { ' ' }, 2);

            string args = token.Length == 2 ? token[1] : string.Empty;
            if (string.IsNullOrEmpty(args))
            {
                //Sytex Check for Array ...............
            }



            var result = new VC_PaserResult_Cmd(token[0], args, errors.ToArray());
            errors.Clear();
            return result;
        }
    }

}