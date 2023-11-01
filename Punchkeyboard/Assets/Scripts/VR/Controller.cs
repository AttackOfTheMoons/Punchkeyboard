using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using Valve.VR;

// The purpose of this script is to be able to move the keyboard 

namespace VR
{
    public class Controller : MonoBehaviour
    {
        public SteamVR_Action_Boolean grabGripAction;
        [FormerlySerializedAs("inputSources")] public SteamVR_Input_Sources inputSource;

        private readonly List<Rigidbody> keyRigidbodies = new();


        private void Start()
        {
            var keys = GameObject.FindGameObjectsWithTag("Key");
            foreach (var t in keys)
                keyRigidbodies.Add(t.GetComponent<Rigidbody>());
        }


        private void OnTriggerStay(Collider col)
        {
            // moving keyboard around by gripping it.
            if (grabGripAction.GetStateDown(inputSource) && col.gameObject.tag == "Keyboard")
            {
                foreach (var rb in keyRigidbodies) rb.isKinematic = true;
                col.transform.SetParent(gameObject.transform);
            }
            else if (grabGripAction.stateUp && col.gameObject.tag == "Keyboard")
            {
                foreach (var rb in keyRigidbodies) rb.isKinematic = false;
                col.transform.SetParent(null);
            }
        }
    }
}