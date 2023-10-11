using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

//using WindowsInput;
//using NewtonVR;

namespace Word_Prediction
{
    public class TextFieldBehaviour : MonoBehaviour, ISelectHandler
    {
        [FormerlySerializedAs("NGramHandler")] public NGramGenerator nGramHandler;
        //public NVRButton Space;

        private InputField inputField;

        private void Start()
        {
            inputField = gameObject.GetComponent<InputField>();
        }

        private void Update()
        {
            //if(Input.GetKeyUp(KeyCode.Space) || Space.ButtonUp && inputField.isFocused)
            if (!Input.GetKeyUp(KeyCode.Space)) return;
            var inputText = inputField.text.TrimEnd();
            var lastWord = inputText.Split(' ').Last();
            nGramHandler.PredictNextWords(lastWord);
        }

        public void OnSelect(BaseEventData eventData)
        {
            StartCoroutine(DisableHighlight());
        }

        public void MoveCaretToEnd()
        {
            StartCoroutine(DisableHighlight());
        }

        private IEnumerator DisableHighlight()
        {
            var originalTextColor = inputField.selectionColor;
            originalTextColor.a = 0f;

            inputField.selectionColor = originalTextColor;

            //Wait for one frame
            yield return null;

            //Scroll the view with the last character
            inputField.MoveTextEnd(true);
            //Change the caret pos to the end of the text
            inputField.caretPosition = inputField.text.Length;

            originalTextColor.a = 1f;
            inputField.selectionColor = originalTextColor;
        }
    }
}