using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;
using Valve.VR.Input;

public class ControllerResponseHandler : MonoBehaviour
{
	public InputField TextInputField;
	public Button SecondarySuggestionBtn;
	public Button PrimarySuggestionBtn;
	public Button TertiarySuggestionBtn;

	public Color HighlightedBtnColor;
	private Color initialBtnColor;

	private SteamVR_Input_Action trackpadAction;
	private SteamVR_Input_Action triggerAction;
	private const int hapticFeedbackStrength = 600;

	void Start ()
	{
		trackpadAction = SteamVR_Actions.default_.TrackpadPressed;
		triggerAction = SteamVR_Actions.default_.GrabPinch;

		// Bind the trackpad and trigger buttons to the actions
		trackpadAction.onStateUpdate += OnTrackpadStateUpdate;
		triggerAction.onStateUpdate += OnTriggerStateUpdate;


		initialBtnColor = PrimarySuggestionBtn.GetComponent<Image> ().color;
	}

	private void OnTrackpadStateUpdate(SteamVR_Action_State_t state)
	{
		// If the trackpad is pressed, trigger haptic feedback
		if (state.delta > 0)
		{
			SteamVR_Actions.default_.TriggerHapticPulse(state.deviceIndex, hapticFeedbackStrength);
		}

		// Get the side of the trackpad that was pressed
		string side = state.side;

		// Highlight the corresponding button
		switch (side)
		{
			case "left":
				StartCoroutine("HighlightButton", SecondarySuggestionBtn.GetComponent<Image>());
				break;
			case "center":
				StartCoroutine("HighlightButton", PrimarySuggestionBtn.GetComponent<Image>());
				break;
			case "right":
				StartCoroutine("HighlightButton", TertiarySuggestionBtn.GetComponent<Image>());
				break;
		}
	}

	// This method is called when the state of the trigger changes
	private void OnTriggerStateUpdate(SteamVR_Action_State_t state)
	{
		// If the trigger is pressed, activate the input field
		if (state.delta > 0)
		{
			TextInputField.ActivateInputField();
		}
	}

	private IEnumerator HighlightButton (Image img)
	{
		// Highlight the button for a certain amount of time
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
		// Fade the button back to its original color for a certain amount of time
		float elapsedTime = 0.0f;
		float totalTime = 0.05f;
		while (elapsedTime < totalTime)
		{
			elapsedTime += Time.deltaTime;
			img.color = Color.Lerp (HighlightedBtnColor, initialBtnColor, (elapsedTime / totalTime));
			yield return null;
		}
	}

	void OnDisable()
	{
		// Unbind the trackpad and trigger buttons from the actions
		trackpadAction.onStateUpdate -= OnTrackpadStateUpdate;
		triggerAction.onStateUpdate -= OnTriggerStateUpdate;
	}
}