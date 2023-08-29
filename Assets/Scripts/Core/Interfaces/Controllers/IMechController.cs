using System;
using UnityEngine;

namespace Endsley
{

    public enum RotationDirection { CW, CCW }
    public interface IMechController
    {
        // SO
        MechLocomotionConfig LocomotionConfig { get; }

        // Basic movement
        void MoveForward(float speed);
        void MoveBackward(float speed);
        void StopMoving();

        // Rotation
        void RotateContinuous(float direction); // For player-like continuous rotation
        void RotateToHeading(float targetHeading); // For AI-like discrete rotation

        // Jumping
        void Jump();

        // // Shooting and combat
        // void Shoot();
        // void AimAt(Vector3 target);

        // Status queries
        Vector3 GetPosition();
        float GetCurrentSpeed();
        float GetCurrentHeading();
        bool IsGrounded();

        // // Advanced
        // void MoveToPoint(Vector3 point); // For AI pathing

        // Events for external components to subscribe
        event Action<float> OnSpeedChange;
        event Action<RotationDirection> OnRotationChange;
        event Action OnJump;
        event Action<bool> OnGroundedChange;
    }

}



