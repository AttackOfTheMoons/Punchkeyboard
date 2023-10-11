using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public abstract class InputKey : MonoBehaviour
{
    public delegate void OnKeyPressed();
    public static OnKeyPressed keyPressedEvent;
    [FormerlySerializedAs("Rigidbody")] public new Rigidbody rigidbody;
    [FormerlySerializedAs("KeyPressed")] public bool keyPressed;
    [FormerlySerializedAs("PressedKeycapColor")] public Color pressedKeyCapColor;
    [FormerlySerializedAs("KeycapColor")] public Color keyCapColor;
    [FormerlySerializedAs("InitialKeycapColor")] public Color initialKeyCapColor;
    
    protected Text keyCapText;

    private const float DistanceToBePressed = 0.01f;
    private const float KeyBounceBackMultiplier = 1500f;
    
    private bool checkForButton;
    private Vector3 constrainedPosition;
    private Quaternion constrainedRotation;
    private float currentDistance = -1;
    private Vector3 initialLocalPosition;
    private Quaternion initialLocalRotation;
    private Transform initialPosition;
    private KeySoundController keySoundController;
    private Material material;

    protected void Start()
    {
        keyCapText = GetComponentInChildren<Text>();
        material = GetComponent<Renderer>().material;
        keyCapColor = material.color;
        initialKeyCapColor = keyCapColor;

        initialPosition = new GameObject($"[{gameObject.name}] initialPosition").transform;
        var thisTransform = transform;
        initialPosition.parent = thisTransform.parent;
        initialPosition.localPosition = Vector3.zero;
        initialPosition.localRotation = Quaternion.identity;

        if (rigidbody == null) rigidbody = GetComponent<Rigidbody>();

        initialLocalPosition = thisTransform.localPosition;
        initialLocalRotation = thisTransform.localRotation;

        constrainedPosition = initialLocalPosition;
        constrainedRotation = initialLocalRotation;

        keySoundController = thisTransform.parent.root.gameObject.GetComponent<KeySoundController>();
        checkForButton = true;
    }

    protected void Update()
    {
        if (checkForButton)
        {
            if (currentDistance > DistanceToBePressed) InternalHit();
        }
        else if (!checkForButton)
        {
            if (currentDistance < DistanceToBePressed)
            {
                keyPressed = false;
                checkForButton = true;
            }
        }

        ChangeKeyColorOnPress();
    }

    private void FixedUpdate()
    {
        ConstrainPosition();
        var initialPositionPosition = initialPosition.position;
        var transformPosition = transform.position;
        currentDistance = Vector3.Distance(transformPosition, initialPositionPosition);
        var positionDelta = initialPositionPosition - transformPosition;
        rigidbody.velocity = positionDelta * (KeyBounceBackMultiplier * Time.deltaTime);
    }

    private void LateUpdate()
    {
        ConstrainPosition();
    }

    public void ExternalHit()
    {
        transform.position += Vector3.forward * (DistanceToBePressed * 5);
    }

    protected abstract void Hit();

    private void InternalHit()
    {
        keyPressed = true;
        keyPressedEvent();
        keySoundController.StartKeySound(gameObject.transform);
        checkForButton = false;
        Hit();
    }

    private void ChangeKeyColorOnPress()
    {
        material.color = keyPressed ? pressedKeyCapColor : keyCapColor;
    }

    private void ConstrainPosition()
    {
        var currentTransform = transform;
        constrainedPosition.y = currentTransform.localPosition.y;
        if (currentTransform.localPosition.y > initialLocalPosition.y) constrainedPosition.y = initialLocalPosition.y;
        currentTransform.localPosition = constrainedPosition;
        currentTransform.localRotation = constrainedRotation;
    }
}