using System.Collections;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Valve.VR;

// This is the script to select which of the predictions you want to use with the 
// joystick (originally trackpad).

namespace VR
{
    public class ControllerResponseHandler : MonoBehaviour
    {
        [FormerlySerializedAs("TextInputField")] public InputField textInputField;
        [FormerlySerializedAs("SecondarySuggestionBtn")] public Button secondarySuggestionBtn;
        [FormerlySerializedAs("PrimarySuggestionBtn")] public Button primarySuggestionBtn;
        [FormerlySerializedAs("TertiarySuggestionBtn")] public Button tertiarySuggestionBtn;

        [FormerlySerializedAs("HighlightedBtnColor")] public Color highlightedBtnColor = Color.cyan;
        public SteamVR_Action_Boolean forwardDPad;
        public SteamVR_Action_Boolean leftDPad;
        public SteamVR_Action_Boolean rightDPad;
        public SteamVR_Action_Boolean activateTextField;
        private Color initialBtnColor;

        private void Start()
        {
            initialBtnColor = primarySuggestionBtn.GetComponent<Image>().color;
        }

        private void Update()
        {
            // device = SteamVR_Controller.Input (deviceID);
            // device.TriggerHapticPulse (hapticFeedbackStrength);

            if (leftDPad.GetStateDown(SteamVR_Input_Sources.Any))
            {
                secondarySuggestionBtn.onClick.Invoke();
                StartCoroutine(nameof(HighlightButton), secondarySuggestionBtn.GetComponent<Image>());
            }
            else if (forwardDPad.GetStateDown(SteamVR_Input_Sources.Any))
            {
                primarySuggestionBtn.onClick.Invoke();
                StartCoroutine(nameof(HighlightButton), primarySuggestionBtn.GetComponent<Image>());
            }
            else if (rightDPad.GetStateDown(SteamVR_Input_Sources.Any))
            {
                tertiarySuggestionBtn.onClick.Invoke();
                StartCoroutine(nameof(HighlightButton), tertiarySuggestionBtn.GetComponent<Image>());
            }

            if (activateTextField.GetStateDown(SteamVR_Input_Sources.Any)) textInputField.ActivateInputField();
        }

        private IEnumerator HighlightButton(Graphic img)
        {
            var elapsedTime = 0.0f;
            const float totalTime = 0.1f;
            while (elapsedTime < totalTime)
            {
                elapsedTime += Time.deltaTime;
                img.color = Color.Lerp(initialBtnColor, highlightedBtnColor, elapsedTime / totalTime);
                StartCoroutine(nameof(FadeButton), img);
                yield return null;
            }
        }

        private IEnumerator FadeButton(Graphic img)
        {
            var elapsedTime = 0.0f;
            const float totalTime = 0.05f;
            while (elapsedTime < totalTime)
            {
                elapsedTime += Time.deltaTime;
                img.color = Color.Lerp(highlightedBtnColor, initialBtnColor, elapsedTime / totalTime);
                yield return null;
            }
        }
    }
}