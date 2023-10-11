using System;
using UnityEngine;

namespace VR.InputSelector
{
    public enum InputMethod
    {
        Drumstick,
        RayCasting,
        RayCastingWithSwiping
    }

    public class InputSelector : MonoBehaviour
    {
        public InputMethod currentInputMethod;
        public GameObject leftController;
        public GameObject rightController;

        private ControllerRayCasting leftControllerRaycast;

        private GameObject leftDrumStick;
        private ControllerRayCasting rightControllerRaycast;
        private GameObject rightDrumStick;

        private void Start()
        {
            leftDrumStick = leftController.transform.Find("Drumsticks (left)").gameObject;
            rightDrumStick = rightController.transform.Find("Drumsticks (right)").gameObject;
            leftControllerRaycast = leftController.GetComponent<ControllerRayCasting>();
            rightControllerRaycast = rightController.GetComponent<ControllerRayCasting>();

            SetInputMethod(InputMethod.Drumstick);
        }

        public void SetInputMethod(InputMethod inputMethod)
        {
            currentInputMethod = inputMethod;
            DisableAllInputs();
            switch (currentInputMethod)
            {
                case InputMethod.Drumstick:
                    leftDrumStick.SetActive(true);
                    rightDrumStick.SetActive(true);
                    return;
                case InputMethod.RayCasting:
                case InputMethod.RayCastingWithSwiping:
                    leftControllerRaycast.enabled = true;
                    rightControllerRaycast.enabled = true;
                    return;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void DisableAllInputs()
        {
            leftDrumStick.SetActive(false);
            rightDrumStick.SetActive(false);
            leftControllerRaycast.enabled = false;
            rightControllerRaycast.enabled = false;
        }
    }
}