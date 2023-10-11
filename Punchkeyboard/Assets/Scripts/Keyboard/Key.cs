using UnityEngine.Serialization;

public class Key : InputKey
{
    [FormerlySerializedAs("KeyCapChar")] public string keyCapChar;
    [FormerlySerializedAs("AlterateKeyCapChar")] public string alternateKeyCapChar;
    private KeycodeAdder keycodeAdder;
    private bool symbolSwitch;
    private bool uppercaseSwitch = true;

    private new void Start()
    {
        base.Start();
        keycodeAdder = gameObject.GetComponent<KeycodeAdder>();
        SwitchKeyCapCharCase();
    }

    protected override void Hit()
    {
        if (symbolSwitch)
            keycodeAdder.SimulateAlternateKeyPress();
        else
            keycodeAdder.SimulateKeyPress();
    }

    public void SwitchKeyCapCharCase()
    {
        if (uppercaseSwitch)
        {
            keyCapText.text = keyCapChar.ToLower();
            uppercaseSwitch = false;
        }
        else
        {
            keyCapText.text = keyCapChar.ToUpper();
            uppercaseSwitch = true;
        }
    }

    public void SwitchToSymbols()
    {
        if (!symbolSwitch)
        {
            keyCapText.text = alternateKeyCapChar;
            symbolSwitch = true;
        }
        else
        {
            keyCapText.text = keyCapChar;
            keyCapText.text = keyCapChar.ToLower();
            symbolSwitch = false;
        }
    }
}