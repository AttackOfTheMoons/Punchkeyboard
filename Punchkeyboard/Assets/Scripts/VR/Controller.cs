﻿using System.Collections.Generic;
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
        private readonly List<Rigidbody> inputRigidBodies = new();
        

        private void Start()
        {
            var keys = GameObject.FindGameObjectsWithTag("Key");
            foreach (var t in keys)
                keyRigidbodies.Add(t.GetComponent<Rigidbody>());
            var inputOptions = GameObject.FindGameObjectsWithTag("InputOption");
            foreach (var r in inputOptions)
                inputRigidBodies.Add(r.GetComponent<Rigidbody>());
        }

        //Disabled moving the keyboard around because of an issue with refining keys.
        // private void OnTriggerStay(Collider col)
        // {
        //     // moving keyboard around by gripping it.
        //     if (grabGripAction.GetStateDown(inputSource))
        //     {
        //         if (col.gameObject.tag == "Keyboard")
        //         {
        //             foreach (var rb in keyRigidbodies) rb.isKinematic = true; 
        //             col.transform.SetParent(gameObject.transform);
        //         }
        //         else if (col.gameObject.tag == "InputSelector")
        //         {
        //             foreach (var rb in inputRigidBodies) rb.isKinematic = true; 
        //             col.transform.SetParent(gameObject.transform);
        //         }
        //
        //     }
        //     else if (grabGripAction.stateUp)
        //     {
        //         if (col.gameObject.tag == "Keyboard")
        //         {
        //             foreach (var rb in keyRigidbodies) rb.isKinematic = false;
        //             col.transform.SetParent(null);
        //         }
        //         else if (col.gameObject.tag == "InputSelector")
        //         {
        //             foreach (var rb in inputRigidBodies) rb.isKinematic = false; 
        //             col.transform.SetParent(null);
        //         }
        //     }
        // }
    }
}