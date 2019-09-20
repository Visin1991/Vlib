using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System;
using System.Linq;

namespace V
{
    public static class LogTypes
    {
        public static string Log = "Log";
        public static string Warning = "Warning";
        public static string Error = "Error";
        public static string Exception = "Exception";
    }

    public static class VC_Logger
    {
        public static Action<VC_Log> logAdded;
        public static Action logsCleard;
        public static Action logRemoved;

        public static void OnLogAdded(VC_Log log)
        {
            if (logAdded != null)
            {
                logAdded(log);
            }
        }

        public static void OnLogsCleard()
        {
            if (logsCleard!=null)
            {
                logsCleard();
            }
        }

        public static void OnLogRemoved()
        {
            if (logRemoved != null)
            {
                logRemoved();
            }
        }

    }


    public static class VConsole
    {     
        private static bool initialed = false;

        private static HashSet<Type> commandClasses = new HashSet<Type>();

        private static Action<VC_Log> onCommandRecive;

        public static void AddCommandReciveListener(Action<VC_Log> _Listener)
        {
            onCommandRecive -= _Listener;
            onCommandRecive += _Listener;
        }

        public static void DeleteCommandReciveListener(Action<VC_Log> _Listener)
        {
            onCommandRecive -= _Listener;
        }

        public static void AddComandClass(Type classType)
        {
            commandClasses.Add(classType);
        }

        //=================================================================================

        public static void Init()
        {
            VTypeCollection.Init();
            AddComandClass(commandClasses.ToArray());
            initialed = true;
        }

        public static void Execute(string command)
        {
            if (initialed == false) { return; }

            if (String.IsNullOrWhiteSpace(command))
            {
                return;
            }

            VC_PaserResult_Cmd result_Cmd = VC_Parser_Cmd.Paser(command);

            if (!result_Cmd.Valid)
            {
                Log(new VC_Log(LogTypes.Error, command + string.Join(Environment.NewLine, result_Cmd.errors)));
                return;
            }

            VC_FunctionInfo funcInfo;
            if (!VC_FunctionInfo.functionInfos.TryGetValue(result_Cmd.functionAlia, out funcInfo))
            {
                Log(new VC_Log(LogTypes.Error, command + string.Format("{0} is not a valid command.", result_Cmd.functionAlia)));
                return;
            }

            ParameterInfo[] methodParams = funcInfo.Method.GetParameters();
            var result_Func = VC_Parser_Func.Paser(result_Cmd, methodParams);
            if (!result_Func.Valid)
            {
                string error = string.Join(Environment.NewLine, result_Func.errors);
                Log(LogTypes.Error, command, error);
                return;
            }

            int minParamsCount = methodParams.Count(p => p.IsOptional);
            var parserRes = result_Func.parameters;
            if (parserRes.Length < minParamsCount)
            {
                Log(LogTypes.Error, command, "Minimum parameters required: {0}{1}Got: {2}", minParamsCount, Environment.NewLine, parserRes.Length);
                return;
            }

            //Invoke Static Function
            funcInfo.Method.Invoke(null, result_Func.parameters);

            Log(LogTypes.Log,command);
        }

        public static void Log(VC_Log log)
        {
            if (onCommandRecive != null)
            {
                onCommandRecive(log);
            }
        }

        public static void Log(string type,string input)
        {
            if (onCommandRecive != null)
            {
                onCommandRecive(new VC_Log(type,input));
            }
        }

        public static void Log(string type, string input, string format, params object[] args)
        {
            if (onCommandRecive != null)
            {
                onCommandRecive(new VC_Log(type, input + "|" + string.Format(format, args)));
            }
        }

        public static void Log(string input)
        {
            if (onCommandRecive != null)
            {
                onCommandRecive(new VC_Log(LogTypes.Log, input));
            }
        }

        static void AddComandClass(Type[] types)
        {
            VC_FunctionInfo.AddClassType(types);
        }

    }
}
