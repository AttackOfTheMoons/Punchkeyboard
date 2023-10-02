using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;

// This is the script to select which of the predictions you want to use with the 
// joystick (originally trackpad).

public class ControllerResponseHandler : MonoBehaviour
{
	public InputField TextInputField;
	public Button SecondarySuggestionBtn;
	public Button PrimarySuggestionBtn;
	public Button TertiarySuggestionBtn;

	public Color HighlightedBtnColor = Color.cyan;
	private Color initialBtnColor;
	public SteamVR_Action_Boolean forwardDPad;
	public SteamVR_Action_Boolean leftDPad;
	public SteamVR_Action_Boolean rightDPad;
	public SteamVR_Action_Boolean activateTextField;


	// private SteamVR_Controller.Device device;
	private const int hapticFeedbackStrength = 600;

	void Start ()
	{
		initialBtnColor = PrimarySuggestionBtn.GetComponent<Image> ().color;
	}

	void Update()
	{
		// device = SteamVR_Controller.Input (deviceID);
		// device.TriggerHapticPulse (hapticFeedbackStrength);

		if (leftDPad.GetStateDown(SteamVR_Input_Sources.Any))
		{
			SecondarySuggestionBtn.onClick.Invoke ();
			StartCoroutine ("HighlightButton", SecondarySuggestionBtn.GetComponent<Image> ());
		}
		else if (forwardDPad.GetStateDown(SteamVR_Input_Sources.Any))
		{
			PrimarySuggestionBtn.onClick.Invoke ();
			StartCoroutine ("HighlightButton", PrimarySuggestionBtn.GetComponent<Image> ());
		}
		else if (rightDPad.GetStateDown(SteamVR_Input_Sources.Any))
		{
			TertiarySuggestionBtn.onClick.Invoke ();
			StartCoroutine ("HighlightButton", TertiarySuggestionBtn.GetComponent<Image> ());
		}

		if (activateTextField.GetStateDown(SteamVR_Input_Sources.Any))
		{
			TextInputField.ActivateInputField ();
		}
	}

	private IEnumerator HighlightButton (Image img)
	{
		float elapsedTime = 0.0f;
		float totalTime = 0.1f;
		while (elapsedTime < totalTime)
		{
			elapsedTime += Time.deltaTime;
			img.color = Color.Lerp (initialBtnColor, HighlightedBtnColor, (elapsedTime / totalTime));
			StartCoroutine ("FadeButton", img);
			yield return null;
		}
	}

	private IEnumerator FadeButton (Image img)
	{
		float elapsedTime = 0.0f;
		float totalTime = 0.05f;
		while (elapsedTime < totalTime)
		{
			elapsedTime += Time.deltaTime;
			img.color = Color.Lerp (HighlightedBtnColor, initialBtnColor, (elapsedTime / totalTime));
			yield return null;
		}
	}

}