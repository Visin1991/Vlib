using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace V
{
    public class VC_Input : MonoBehaviour
    {

        private InputField inputField;
        private Button enterButton;
        private KeyCode enterKey = KeyCode.Return;

        private void OnEnable()
        {
            Focus();
        }

        void Start()
        {
            inputField = GetComponentInChildren<InputField>();
            enterButton = GetComponentInChildren<Button>();

            if(inputField !=null)
                inputField.onEndEdit.AddListener(ReviceInput);
            if (enterButton != null)
                enterButton.onClick.AddListener(ProcessInput);
            
        }


        void ReviceInput(string input)
        {
            if (Input.GetKeyDown(enterKey))
            {
                ProcessInput(input);
            }
        }

        void ProcessInput(string input)
        {
            if (!String.IsNullOrWhiteSpace(input))
            {
                VConsole.Execute(input.TrimEnd());

                inputField.text = string.Empty;
                Focus();
            }
        }

        void ProcessInput()
        {
            ProcessInput(inputField.text);
        }

        public void Focus()
        {
            if (inputField == null) { return; }
            inputField.ActivateInputField();
            if (this.isActiveAndEnabled)
            {
                StartCoroutine(MoveTextEnd_NextFrame());
            }
        }

        private IEnumerator MoveTextEnd_NextFrame()
        {
            yield return 0;
            inputField.MoveTextEnd(false);
        }
    }
}
