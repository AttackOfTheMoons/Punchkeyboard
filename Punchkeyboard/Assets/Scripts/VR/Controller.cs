using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

// The purpose of this script is to be able to move the keyboard 

public class Controller : MonoBehaviour
{
    public SteamVR_Action_Boolean grabGripAction;
    public SteamVR_Input_Sources inputSources;

    private List<Rigidbody> keyRigidbodies = new List<Rigidbody>();

    void Start()
    {
        GameObject[] keys = GameObject.FindGameObjectsWithTag("Key");
        for (int i = 0; i < keys.Length; i++)
        {
            keyRigidbodies.Add(keys[i].GetComponent<Rigidbody>());
        }
    }


    void OnTriggerStay(Collider col)
    {
        if (grabGripAction.stateDown && col.gameObject.tag == "Keyboard")
        {
            foreach (Rigidbody rb in keyRigidbodies)
            {
                rb.isKinematic = true;
            }
            col.transform.SetParent(this.gameObject.transform);
        }
        else if (grabGripAction.stateUp && col.gameObject.tag == "Keyboard")
        {
            foreach (Rigidbody rb in keyRigidbodies)
            {
                rb.isKinematic = false;
            }
            col.transform.SetParent(null);
        }
    }

    
}
