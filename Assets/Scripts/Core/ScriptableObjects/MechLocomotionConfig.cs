using UnityEngine;

namespace Endsley
{
    [CreateAssetMenu(fileName = "MechLocomotionConfig", menuName = "Configs/MechLocomotionConfig", order = 1)]
    public class MechLocomotionConfig : ScriptableObject
    {
        [Tooltip("Walking speed of the mech")]
        public float walkSpeed = 5.0f;

        [Tooltip("Running speed of the mech")]
        public float runSpeed = 10.0f;

        [Tooltip("How fast the mech ramps between its various speeds")]
        public float acceleration = 2.0f;

        [Tooltip("Deceleration speed")]
        public float deceleration = 2.0f;

        [Tooltip("How high this mech can jump")]
        public float jumpForce = 5.0f;

        [Tooltip("Beyond this distance, mechs will run instead of walk to their destination")]
        public float runThreshold = 10.0f;
    }
}
