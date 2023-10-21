using UnityEngine;
using UnityEngine.Serialization;
using Valve.VR;

namespace VR
{
    [RequireComponent(typeof(LineRenderer))]
    public class ControllerRayCasting : MonoBehaviour
    {
        public HandType handType;
        public SteamVR_Action_Boolean triggerAction;

        public Transform raycastOrigin;
        public float maxRaycastDistance = 10f;

        [FormerlySerializedAs("raycastLayerMask")]
        public LayerMask keyLayerMask;

        private const float ActivationDelay = 0.3f;
        private InputKey hitKey;
        private InputKey hitNextFrame;
        private float lastActivated = float.PositiveInfinity;
        private LineRenderer rayLine;
        private SteamVR_Input_Sources source;

        private bool startupFailed = false;

        private void Start()
        {
            source = handType == HandType.Left ? SteamVR_Input_Sources.LeftHand : SteamVR_Input_Sources.RightHand;
            if (raycastOrigin == null) raycastOrigin = transform.Find("Model")?.Find("tip")?.Find("attach")?.transform;
            if (raycastOrigin == null)
            {
                startupFailed = true;
                Debug.LogWarning("Didn't find the " + handType + " controller.");
                return;
            }
            rayLine = GetComponent<LineRenderer>();
            rayLine.enabled = true;
        }

        private void Update()
        {
            if (startupFailed)
            {
                return;
            }
            var raycastOriginPosition = raycastOrigin.position;
            var raycastOriginForward = raycastOrigin.forward;
            var ray = new Ray(raycastOriginPosition, raycastOriginForward);
            rayLine.SetPosition(0, raycastOriginPosition);
            if (Physics.Raycast(ray, out var shortenLine))
            {
                rayLine.SetPosition(1, shortenLine.point);
            }
            else {
                rayLine.SetPosition(1, raycastOriginPosition + raycastOriginForward);
            }
            // Check if the trigger button is pressed
            if (triggerAction.GetState(source))
            {
                if (Physics.Raycast(ray, out var hit, maxRaycastDistance, keyLayerMask))
                    hitKey = hit.collider.GetComponent<InputKey>();
            }
            else
            {
                hitKey = null;
            }
        }

        private void FixedUpdate()
        {
            lastActivated += Time.fixedDeltaTime;
            if (!(lastActivated > ActivationDelay) || !hitKey) return;
            lastActivated = 0f;
            hitKey.ExternalHit();
        }

        private void OnEnable()
        {
            Start();
        }


        private void OnDisable()
        {
            if (!rayLine || !rayLine.enabled) return;
            rayLine.SetPosition(0, Vector3.zero);
            rayLine.SetPosition(1, Vector3.zero);
            rayLine.enabled = false;
        }
    }
}