using System;
using UnityEngine;

namespace Endsley
{
    //TODO: Make this agnostic of the magic numbers inside the animation controller (Like 0.5 for walking, 1.0 for running for example)
    public class MechAnimationController : MonoBehaviour
    {

        [SerializeField] private Animator anim;
        [SerializeField] private MechController mechController;

        [ReadOnly] public float speed;

        private void Awake()
        {
            mechController.OnSpeedChange += SetSpeed;
            mechController.OnRotationChange += SetRotating;
            mechController.OnJump += () => anim.SetTrigger("Jump");
            mechController.OnGroundedChange += grounded => anim.SetBool("Grounded", grounded);
        }

        float NormalizeSpeed(float currentSpeed)
        {
            //TODO: Same anim for backward and forwards walking until more are addeed
            currentSpeed = Math.Abs(currentSpeed);

            float walkSpeed = mechController.LocomotionConfig.walkSpeed;
            float runSpeed = mechController.LocomotionConfig.runSpeed;

            if (currentSpeed <= walkSpeed)
            {
                return Mathf.Lerp(0f, 0.5f, currentSpeed / walkSpeed);
            }
            else
            {
                return Mathf.Lerp(0.5f, 1f, (currentSpeed - walkSpeed) / (runSpeed - walkSpeed));
            }
        }

        void SetSpeed(float speed)
        {
            anim.SetFloat("Speed", NormalizeSpeed(speed));
            // Inspection purposes only
            this.speed = speed;
        }

        // Allows animator to react to turning (including in-place)
        void SetRotating(float dir)
        {
            Debug.Log("SetRotating called");
            // For now, just play the step animation at a slower speed, but allow it to be overriden by faster movements (turning while walking)
            if (dir == 0)
            {
                anim.SetBool("Rotating", false);
            }
            else
            {
                anim.SetBool("Rotating", true);
            }
        }

        //TODO: Triggers for jump and bool for ground

    }
}