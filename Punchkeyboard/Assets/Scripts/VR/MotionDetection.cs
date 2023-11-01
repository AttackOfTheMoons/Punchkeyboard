using Keyboard;
using UnityEngine;

namespace VR
{
    using UnityEngine;

    public class MotionDetection : MonoBehaviour
    {
        public KeyboardManager keyboardManager;
        private Quaternion previousRotation;
        private Vector3 previousPosition;
        private const float AngularVelocityThreshold = 900f; // Degrees per second
        private const float MotionDurationThreshold = 0.016f; // Seconds
        private const float MotionCooldown = 0.1f; // Cooldown period in seconds
        private float motionTimer;
        private float cooldownTimer;

        private void Start()
        {
            var controllerTransform = transform;
            previousRotation = controllerTransform.rotation;
            previousPosition = controllerTransform.position;
        }

        private void Update()
        {
            cooldownTimer += Time.deltaTime;

            if (cooldownTimer < MotionCooldown) return;
            // Calculate the change in rotation since the last frame
            var currentRotation = transform.rotation;
            var angle = Quaternion.Angle(previousRotation, currentRotation);
            var deltaTime = Time.deltaTime;

            // Calculate the angular velocity in degrees per second
            var angularVelocity = angle / deltaTime;
                
            Vector3 currentPosition = transform.position;

            if (angularVelocity >= AngularVelocityThreshold)
            {
                // Angular velocity exceeds the threshold OR significant position change; motion is detected.
                motionTimer += deltaTime;

                if (motionTimer >= MotionDurationThreshold)
                {
                    // Sustained motion for the specified duration.
                    Debug.Log("Motion detected and sustained for " + motionTimer + " seconds.");
                    // Calculate the change in position since the last frame
                    var positionChange = currentPosition - previousPosition;
                    if (Mathf.Abs(positionChange.x) > Mathf.Abs(positionChange.y) && Mathf.Abs(positionChange.x) > Mathf.Abs(positionChange.z))
                    {
                        if (positionChange.x > 0)
                            keyboardManager.RefineRight();
                        else
                            keyboardManager.RefineLeft();
                    }
                    else if (Mathf.Abs(positionChange.y) > Mathf.Abs(positionChange.x) && Mathf.Abs(positionChange.y) > Mathf.Abs(positionChange.z))
                    {
                        // if (positionChange.y > 0)
                        //     Debug.Log("Motion direction: Up");
                        // else
                        //     Debug.Log("Motion direction: Down");
                    }

                    // Reset the timer and enter cooldown.
                    motionTimer = 0f;
                    cooldownTimer = 0f;
                }
            }
            else
            {
                motionTimer = 0f;
            }

            previousRotation = currentRotation;
            previousPosition = currentPosition;
        }
    }


}