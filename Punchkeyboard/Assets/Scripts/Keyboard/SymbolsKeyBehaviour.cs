using Keyboard;
using UnityEngine;

public class SymbolsKeyBehaviour : MonoBehaviour
{
    public ShiftKeyBehaviour ShiftBehaviour;
    private Key[] keyControllers;
    private GameObject[] keys;

    private Key symbolKeyController;
    private bool symbolToggle = true;

    private void Start()
    {
        InputKey.keyPressedEvent += SpecialKeyPressed;

        symbolKeyController = gameObject.GetComponent<Key>();
        keys = GameObject.FindGameObjectsWithTag("Key");
        keyControllers = new Key[keys.Length];
        for (var i = 0; i < keys.Length; i++) keyControllers[i] = keys[i].GetComponent<Key>();
    }

    private void OnDisable()
    {
        InputKey.keyPressedEvent -= SpecialKeyPressed;
    }

    private void SpecialKeyPressed()
    {
        if (!symbolKeyController.keyPressed) return;
        foreach (var key in keyControllers)
            key.SwitchToSymbols();

        if (symbolToggle)
        {
            ShiftBehaviour.ShiftVisibilityToggle(false);
            symbolKeyController.keyCapColor = symbolKeyController.pressedKeyCapColor;
            symbolToggle = false;
        }
        else
        {
            ShiftBehaviour.ShiftVisibilityToggle(true);
            symbolKeyController.keyCapColor = symbolKeyController.initialKeyCapColor;
            symbolToggle = true;
        }
    }
}