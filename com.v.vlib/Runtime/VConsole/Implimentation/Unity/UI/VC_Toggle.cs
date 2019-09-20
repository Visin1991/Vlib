using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace V
{
    public class VC_Toggle : MonoBehaviour
    {
        private bool VC_windowOpen = false;

        public List<GameObject> toggleObject = new List<GameObject>();
        Image buttonImage;
        static Color Open = new Color(0.7f, 0.7f, 0.7f);
        static Color Close = new Color(0.43f, 0.43f, 0.43f);

        private void OnEnable()
        {

            for (int i = 0; i < toggleObject.Count; i++)
            {
                toggleObject[i].SetActive(VC_windowOpen);
            }
        
            Button button = GetComponent<Button>();
            if (button)
            {
                button.onClick.AddListener(VC_WindowToggle);
            }
            buttonImage = GetComponent<Image>();
            if (buttonImage)
            {
                buttonImage.color = VC_windowOpen ? Open : Close;
            }
        }

        private void OnDisable()
        {
            Button button = GetComponent<Button>();
            if (button)
            {
                button.onClick.RemoveListener(VC_WindowToggle);
            }
        }

        void VC_WindowToggle()
        {
            VC_windowOpen = !VC_windowOpen;

            if (buttonImage)
            {
                buttonImage.color = VC_windowOpen ? Open : Close;
            }

            for (int i = 0; i < toggleObject.Count; i++)
            {
                toggleObject[i].SetActive(VC_windowOpen);
            }
        }
    }
}
