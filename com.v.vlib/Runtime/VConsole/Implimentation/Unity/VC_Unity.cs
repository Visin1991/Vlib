using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace V
{
    public class VC_Unity : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            VConsole.AddComandClass(typeof(VC_Unity_Binding));
            VConsole.Init();

        }

        private void OnEnable()
        {
            Application.logMessageReceived += Application_logMessageReceived;
        }

        private void OnDisable()
        {
            Application.logMessageReceived -= Application_logMessageReceived;
        }


        private void Application_logMessageReceived(string condition, string stackTrace, LogType type)
        {
            VC_Log log = new VC_Log(LogTypes.Error, "Unity log callback" + string.Format("Undefined log type: {0}!", type.ToString()));
            switch (type)
            {
                case LogType.Assert:
                    log = new VC_Log(LogTypes.Exception, condition + " : stackTrace: " + stackTrace);
                    break;
                case LogType.Error:
                    log = new VC_Log(LogTypes.Error, condition + " : stackTrace: " + stackTrace);
                    break;
                case LogType.Exception:
                    log = new VC_Log(LogTypes.Exception, condition + " : stackTrace: " + stackTrace);
                    break;
                case LogType.Log:
                    log = new VC_Log(LogTypes.Log, condition + " : stackTrace: " + stackTrace);
                    break;
                case LogType.Warning:
                    log = new VC_Log(LogTypes.Warning, condition + " : stackTrace: " + stackTrace);
                    break;
            }
            VConsole.Log(log);
        }
    }

}