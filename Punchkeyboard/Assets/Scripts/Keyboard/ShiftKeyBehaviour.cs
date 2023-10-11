using UnityEngine;
using UnityEngine.Serialization;
using WindowsInput;

public class ShiftKeyBehaviour : MonoBehaviour
{
    [FormerlySerializedAs("Housing")] public GameObject housing;
    private GameObject keyCap;
    private BoxCollider keyCollider;
    private Key[] keyControllers;
    private Renderer keyRenderer;
    private GameObject[] keys;

    private Key shiftKeyController;
    private bool shiftToggle = true;

    private void Start()
    {
        InputKey.keyPressedEvent += ShiftKeyPressed;
        shiftKeyController = gameObject.GetComponent<Key>();
        keys = GameObject.FindGameObjectsWithTag("Key");
        keyControllers = new Key[keys.Length];
        for (var i = 0; i < keys.Length; i++) keyControllers[i] = keys[i].GetComponent<Key>();

        keyRenderer = gameObject.GetComponent<Renderer>();
        keyCollider = gameObject.GetComponent<BoxCollider>();
        keyCap = gameObject.transform.GetChild(0).gameObject;
    }

    private void OnDisable()
    {
        InputKey.keyPressedEvent -= ShiftKeyPressed;
        if (!shiftToggle) InputSimulator.SimulateKeyPress(VirtualKeyCode.CAPITAL);
    }

    private void ShiftKeyPressed()
    {
        if (!shiftKeyController.keyPressed) return;
        foreach (var key in keyControllers)
            key.SwitchKeyCapCharCase();

        if (shiftToggle)
        {
            shiftKeyController.keyCapColor = shiftKeyController.pressedKeyCapColor;
            InputSimulator.SimulateKeyPress(VirtualKeyCode.CAPITAL);
            shiftToggle = false;
        }
        else
        {
            shiftKeyController.keyCapColor = shiftKeyController.initialKeyCapColor;
            InputSimulator.SimulateKeyPress(VirtualKeyCode.CAPITAL);
            shiftToggle = true;
        }
    }

    public void ShiftVisibilityToggle(bool state)
    {
        keyRenderer.enabled = state;
        keyCollider.enabled = state;
        keyCap.SetActive(state);
        housing.SetActive(state);
        shiftKeyController.keyCapColor = shiftKeyController.initialKeyCapColor;
    }
}