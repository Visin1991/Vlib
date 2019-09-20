using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace V
{

    public class VC_LogContainer : MonoBehaviour
    {
        public int maxInstance = 100;
        public VC_LogItem logItemPrefab;

        [SerializeField]
        private Color logDefaultColor = new Color(0,0,0);
        [SerializeField]
        private Color logAlternateColor = new Color(0.2f,0.2f,0.2f);

        public Scrollbar scrollbar;

        private Dictionary<VC_Log, VC_LogItem> logDictionary = new Dictionary<VC_Log, VC_LogItem>();
        private List<VC_LogItem> activeLogs = new List<VC_LogItem>();
        private bool logActive = true;
        private List<VC_LogItem> activeWarning = new List<VC_LogItem>();
        private bool warningActive = true;
        private List<VC_LogItem> activeError = new List<VC_LogItem>();
        private bool errorActive = true;

        private void Awake()
        {
            VConsole.AddCommandReciveListener(OnLogAdded);
        }

        private void OnDestroy()
        {
            VConsole.DeleteCommandReciveListener(OnLogAdded);
        }

        void OnLogAdded(VC_Log log)
        {
            VC_LogItem instance;

            //Increase the Instance count
            if (logDictionary.ContainsKey(log))
            {
                instance = logDictionary[log];
                instance.Count += 1;
            }
            else
            {
                //Create a New Instance
                if (transform.childCount < maxInstance)
                {
                    instance = GameObject.Instantiate(logItemPrefab);
                    instance.Initialize(log);
                    instance.transform.SetParent(transform, false);
                    instance.transform.SetAsLastSibling();

                    CacheLogItem(log, instance);

                    instance.BackgroundColor = instance.transform.GetSiblingIndex() % 2 != 0 ? logDefaultColor : logAlternateColor;
                }
                else {
                    //Remove the top
                    GameObject firstChild = transform.GetChild(0).gameObject;
                    instance = firstChild.GetComponent<VC_LogItem>();
                    RemoveCache(instance);
                    CacheLogItem(log,instance);

                    //Reset the Log Item
                    instance.Initialize(log);
                    instance.transform.SetAsLastSibling();
                    logDictionary.Add(log, instance);
                    instance.BackgroundColor = instance.transform.GetSiblingIndex() % 2 != 0 ? logDefaultColor : logAlternateColor;
                }

                if (gameObject.activeInHierarchy)
                {
                    if(scrollbar)
                        scrollbar.value = 0;
                }
            }        
        }

        //private IEnumerator MoveScrollbarNextFrame()
        //{
        //    // Updating all canvases can produce frame drops if there is too much to draw. If you remove this, the scrollbar won't snap at the bottom when a new log is added.
        //    Canvas.ForceUpdateCanvases();
        //    yield return null;
        //    _scrollbar.value = 0;
        //}


        void CacheLogItem(VC_Log log,VC_LogItem instance)
        {
            logDictionary.Add(log, instance);

            if (log.Type == LogTypes.Error)
            {
                instance.Icon = new Color(0.9607843f, 0.2538381f, 0.0588235f);
                instance.typeText.text = "Error";
                activeError.Add(instance);
            }
            else if (log.Type == LogTypes.Warning)
            {
                instance.Icon = new Color(0.9622642f, 0.7508726f, 0.05900679f);
                instance.typeText.text = "Warning";
                activeWarning.Add(instance);
            }
            else if (log.Type == LogTypes.Log)
            {
                instance.Icon = new Color(0.6911268f, 0.8204549f, 0.8773585f);
                instance.typeText.text = "Log";
                activeLogs.Add(instance);
            }
        
        }

        void RemoveCache(VC_LogItem instance)
        {        
            logDictionary.Remove(instance.Log);
            if (instance.Log.Type == LogTypes.Log)
            {
                activeLogs.Remove(instance);
            }
            else if (instance.Log.Type == LogTypes.Warning)
            {
                activeWarning.Remove(instance);
            }
            else if (instance.Log.Type == LogTypes.Error)
            {
                activeError.Remove(instance);
            }
        }

        public void DisableTypes(VCFilterType vCFilterType)
        {
            if (vCFilterType == VCFilterType.Infos)
            {
                ToggleLogs();
            }
            else if (vCFilterType == VCFilterType.Warning)
            {
                ToggleWaring();
            }
            else if (vCFilterType == VCFilterType.Error)
            {
                ToggleError();
            }
        }

        void ToggleLogs()
        {
            logActive = !logActive;
            for ( int i = 0; i < activeLogs.Count;i++)
            {
                activeLogs[i].gameObject.SetActive(logActive);
            }
            RefreshAllChildre();
        }

        void ToggleWaring()
        {
            warningActive = !warningActive;
            for (int i = 0; i < activeWarning.Count; i++)
            {
                activeWarning[i].gameObject.SetActive(warningActive);
            }
            RefreshAllChildre();

        }

        void ToggleError()
        {
            errorActive = !errorActive;
            for (int i = 0; i < activeError.Count; i++)
            {
                activeError[i].gameObject.SetActive(errorActive);
            }
            RefreshAllChildre();
        }

        void RefreshAllChildre()
        {
            int activeIndex = 0;
            for (int i = 0; i < transform.childCount; i++)
            {
                Transform tf = transform.GetChild(i);
                if (tf.gameObject.activeSelf)
                {
                    VC_LogItem instance = tf.GetComponent<VC_LogItem>();
                    instance.BackgroundColor = activeIndex % 2 != 0 ? logDefaultColor : logAlternateColor;
                    activeIndex++;
                }
               
            }
        }

    }
}
