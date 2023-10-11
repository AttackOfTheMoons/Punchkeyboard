using UnityEngine;
using UnityEngine.Serialization;
using WindowsInput;

public class KeycodeAdder : MonoBehaviour
{
    [FormerlySerializedAs("KeyKeycode")] public VirtualKeyCode keyKeycode;
    [FormerlySerializedAs("KeyKeycodeModifier")] public VirtualKeyCode keyKeycodeModifier;
    [FormerlySerializedAs("AlternateKeyKeycode")] public VirtualKeyCode[] alternateKeyKeycode;

    public void SimulateKeyPress()
    {
        InputSimulator.SimulateKeyPress(keyKeycode);
    }

    public void SimulateAlternateKeyPress()
    {
        InputSimulator.SimulateModifiedKeyStroke(keyKeycodeModifier, alternateKeyKeycode);
    }
}