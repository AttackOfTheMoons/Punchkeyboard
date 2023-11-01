using System.Collections;
using Keyboard;
using UnityEngine;
using Valve.VR;

namespace VR
{
    public class CollisionResponseFeedback : MonoBehaviour
    {
        public HandType handType;
        public SteamVR_Action_Vibration hapticAction;

        private void Start()
        {
            InputKey.keyPressedEvent += KeyPressedHapticFeedback;
        }

        private void OnDisable()
        {
            InputKey.keyPressedEvent -= KeyPressedHapticFeedback;
        }

        private void KeyPressedHapticFeedback()
        {
            StartCoroutine(nameof(TriggerHapticFeedback));
        }

        private IEnumerator TriggerHapticFeedback()
        {
            // TODO: Make the haptics only occur for the source.
            var inputSource = handType == HandType.Left
                ? SteamVR_Input_Sources.LeftHand
                : SteamVR_Input_Sources.RightHand;
            hapticAction.Execute(0, (float)0.0005, 150, 75, inputSource);
            yield return new WaitForEndOfFrame();
            hapticAction.Execute(0, (float)0.002, 150, 75, inputSource);
            yield return new WaitForEndOfFrame();
        }
    }
}