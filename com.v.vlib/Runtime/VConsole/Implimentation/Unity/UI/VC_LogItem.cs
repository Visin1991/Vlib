using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace V
{
    public class VC_LogItem : MonoBehaviour
    {
        public Text input;

        public Color TitleColor
        {
            set
            {
                input.color = value;
            }
        }

        public string Input
        {
            get
            {
                return input.text;
            }
            set
            {
                input.text = value;
            }
        }

        public VC_Log Log { get; set; }

        public Image icon;

        public Text typeText;

        public Image counterBackground;

        public Text counterLabel;

        public Image background;

        public int maxStackSize = 99;

        private VC_LogContainer logContainer;

        private int count = 1;

        public Color BackgroundColor
        {
            set
            {
                background.color = value;
            }
        }

        public Color Icon
        {
            set
            {
                icon.color = value;
            }
        }

        public string StackTrace { get; set; }

        public string Output { get; set; }

        public int Count
        {
            get
            {
                return count;
            }
            set
            {
                count = value;
                counterBackground.gameObject.SetActive(count > 1);
                counterLabel.text = count < maxStackSize ? count.ToString() : string.Format("{0}+", maxStackSize.ToString());
            }
        }

        public int MaxStackSize
        {
            get
            {
                return maxStackSize;
            }
        }

        private void Start()
        {
            logContainer = GetComponentInParent<VC_LogContainer>();
            
        }

        public void Initialize(VC_Log log)
        {
            Log = log;
            Input = log.Input;
            StackTrace = log.StackTrace;
            Count = 1;
        }
    }
}
