using System.Collections;
using UnityEngine;
using Valve.VR;

// The purpose of this script is to give the user feedback when a key is actuated.
// The script is attached to the tip of the drumstick and in unity it is indicated which hand the controller represents.

public class CollisionFeedbackController : MonoBehaviour
{

    public SteamVR_Action_Vibration hapticAction;
    public bool leftHand;
    private const int KeyPressFeedbackStrength = 1500;
    private bool isColliding = false;

    void Start()
    {
        Key.keyPressed += KeyPressedHapticFeedback;
    }

    private void OnCollisionStay(Collision collision)
    {
        isColliding = true;
    }

    private void OnCollisionExit(Collision collision)
    {
        isColliding = false;
    }

    private void KeyPressedHapticFeedback()
    {
        if (isColliding)
        {
            StartCoroutine ("TriggerHapticFeedback", KeyPressFeedbackStrength);
        }
    }

    private void OnDisable()
    {
        Key.keyPressed -= KeyPressedHapticFeedback;
    }

    private IEnumerator TriggerHapticFeedback(int strength)
    {
        var inputSource = leftHand ? SteamVR_Input_Sources.LeftHand : SteamVR_Input_Sources.RightHand;
        hapticAction.Execute(0, (float) 0.0005, 150, 75, inputSource);
        yield return new WaitForEndOfFrame();
        hapticAction.Execute(0, (float) 0.002, 150, 75, inputSource);
        yield return new WaitForEndOfFrame();
    }
}