using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace V
{
    public enum VCFilterType
    {
        Infos,
        Warning,
        Error
    }

    public class VC_LogFilterButton : MonoBehaviour
    {


        public VCFilterType FilterType = VCFilterType.Infos;

        public VC_LogContainer VC_LogContainer;

        private void OnEnable()
        {
            Button button = GetComponent<Button>();
            if (button)
            {
                button.onClick.AddListener(LogInfoToggle);
            }
        }

        private void OnDisable()
        {
            Button button = GetComponent<Button>();
            if (button)
            {
                button.onClick.RemoveListener(LogInfoToggle);
            }
        }

        void LogInfoToggle()
        {
            if (VC_LogContainer)
            {
                VC_LogContainer.DisableTypes(FilterType);
            }
        }
    }

}