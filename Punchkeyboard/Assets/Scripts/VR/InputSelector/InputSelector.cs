using System;
using Keyboard;
using UnityEngine;

namespace VR.InputSelector
{
    public enum InputMethod
    {
        Drumstick,
        RayCasting,
        XRSlate
    }

    public class InputSelector : MonoBehaviour
    {
        public InputMethod currentInputMethod;
        
        public GameObject leftController;
        public GameObject rightController;

        private GameObject rightDrumStick;
        private GameObject leftDrumStick;
        
        private ControllerRayCasting rightControllerRaycast;
        private ControllerRayCasting leftControllerRaycast;

        public KeyboardManager keyboardManager;
        private MotionDetection rightMotionDetector;
        private MotionDetection leftMotionDetector;
        
        private void Start()
        {
            rightDrumStick = rightController.transform.Find("Drumsticks (right)").gameObject;
            leftDrumStick = leftController.transform.Find("Drumsticks (left)").gameObject;
            
            rightControllerRaycast = rightController.GetComponent<ControllerRayCasting>();
            leftControllerRaycast = leftController.GetComponent<ControllerRayCasting>();

            rightMotionDetector = rightController.GetComponent<MotionDetection>();
            leftMotionDetector = leftController.GetComponent<MotionDetection>();

            SetInputMethod(InputMethod.Drumstick);
        }

        public void SetInputMethod(InputMethod inputMethod)
        {
            currentInputMethod = inputMethod;
            DisableAllInputs();
            switch (currentInputMethod)
            {
                case InputMethod.Drumstick:
                    rightDrumStick.SetActive(true);
                    leftDrumStick.SetActive(true);
                    break;
                case InputMethod.XRSlate:
                    rightMotionDetector.enabled = true;
                    leftMotionDetector.enabled = true;
                    goto case InputMethod.RayCasting;
                case InputMethod.RayCasting:
                    rightControllerRaycast.enabled = true;
                    leftControllerRaycast.enabled = true;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void DisableAllInputs()
        {
            rightDrumStick.SetActive(false);
            leftDrumStick.SetActive(false);
            
            rightControllerRaycast.enabled = false;
            leftControllerRaycast.enabled = false;
            
            rightMotionDetector.enabled = false;
            leftMotionDetector.enabled = false;
            keyboardManager.ResetRefinement();
        }
    }
}