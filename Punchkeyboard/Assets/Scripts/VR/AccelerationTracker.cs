using System;
using UnityEngine;
using Valve.VR;

namespace VR
{

using UnityEngine;

public class AccelerationTracker : MonoBehaviour
{
    private Vector3 previousPosition;
    private Vector3 previousVelocity;
    private float horizontalAcceleration;
    private Quaternion previousRotation;
    private Vector3 previousAngularVelocity;
    private float angularAcceleration;
    public SteamVR_Action_Boolean triggerAction;
    private float maxHorizontalAcceleration;
    private float maxAngularAcceleration;
    private Vector2 maxCombinedValue;

    // Define the axis around which the controller should rotate (e.g., Y for side-to-side rotation).
    public Vector3 rotationAxis = Vector3.up;

    // Define scaling factors for horizontal acceleration and angular acceleration.
    private float horizontalAccelerationScale = 1.0f;
    private float angularAccelerationScale = 0.0001f;

    void Start()
    {
        previousPosition = transform.position;
        previousVelocity = Vector3.zero;
        horizontalAcceleration = 0;
        previousRotation = transform.rotation;
        previousAngularVelocity = Vector3.zero;
        angularAcceleration = 0;
        maxHorizontalAcceleration = 0;
        maxAngularAcceleration = 0;
        maxCombinedValue = Vector2.zero;
    }

    void Update()
    {
        // Track positional acceleration.
        Vector3 currentPosition = transform.position;
        Vector3 currentVelocity = (currentPosition - previousPosition) / Time.deltaTime;
        Vector3 currentAcceleration = (currentVelocity - previousVelocity) / Time.deltaTime;

        // Calculate the magnitude of horizontal acceleration as a scalar value (along the X-axis).
        horizontalAcceleration = Math.Abs(currentAcceleration.x) * horizontalAccelerationScale;

        // Track angular acceleration.
        Quaternion currentRotation = transform.rotation;
        Vector3 currentAngularVelocity = (currentRotation.eulerAngles - previousRotation.eulerAngles) / Time.deltaTime;
        Vector3 currentAngularAcceleration = (currentAngularVelocity - previousAngularVelocity) / Time.deltaTime;

        // Calculate the magnitude of angular acceleration along the specified axis.
        angularAcceleration = Math.Abs(Vector3.Dot(currentAngularAcceleration, rotationAxis) * angularAccelerationScale);

        // Update the previous position, velocity, rotation, and angular velocity for the next frame.
        previousPosition = currentPosition;
        previousVelocity = currentVelocity;
        previousRotation = currentRotation;
        previousAngularVelocity = currentAngularVelocity;

        // Update peak values for horizontal acceleration and angular acceleration.
        maxHorizontalAcceleration = Mathf.Max(horizontalAcceleration, maxHorizontalAcceleration);
        maxAngularAcceleration = Mathf.Max(angularAcceleration, maxAngularAcceleration);

        // Combine the scaled values of horizontal acceleration and angular acceleration.
        var combinedValue = new Vector2(horizontalAcceleration, angularAcceleration);

        // Update the peak combined value.
        maxCombinedValue = combinedValue.x + combinedValue.y > maxCombinedValue.x + maxCombinedValue.y ? combinedValue : maxCombinedValue;
    }

    void LateUpdate()
    {
        // Check for the 'stateUp' event to output peak values.
        if (triggerAction.stateUp && maxHorizontalAcceleration > 0)
        {
            // Output peak values in the 'stateUp' event.
            Debug.Log("Peak Horizontal Acceleration: " + maxHorizontalAcceleration);
            Debug.Log("Peak Angular Acceleration: " + maxAngularAcceleration);
            Debug.Log("Peak Combined Value: " + maxCombinedValue);

            // Reset peak values.
            maxHorizontalAcceleration = 0;
            maxAngularAcceleration = 0;
            maxCombinedValue = Vector2.zero;
        }
    }
}





}