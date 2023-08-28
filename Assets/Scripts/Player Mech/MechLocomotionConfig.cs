using UnityEngine;
[CreateAssetMenu(fileName = "MechLocomotionConfig", menuName = "Configs/MechLocomotionConfig", order = 1)]
public class MechLocomotionConfig : ScriptableObject
{
    //Walking speed of the mech
    public float walkSpeed;
    //Running speed of the mech
    public float runSpeed;
    //How fast the mech ramps between its various speeds
    public float acceleration;
    public float deceleration;
    //Rotation speed of the mech
    public float rotationSpeed;
    //How high this mech can jump
    public float jumpHeight;
    //Beyond this distance, mechs will run instead of walk to their destination
    public float runThreshold;
}