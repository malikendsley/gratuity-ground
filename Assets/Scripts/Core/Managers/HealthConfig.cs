using UnityEngine;

[CreateAssetMenu(fileName = "HealthConfig", menuName = "Mechs/HealthConfig")]
public class HealthConfig : ScriptableObject
{
    public int maxHealth;
    public int maxShields;
}