using System;
using UnityEngine;

namespace Endsley
{
    public class HitDetectionManager : MonoBehaviour, IDamageable
    {

        [Tooltip("This allows for things made of multiple game objects to connect multiple colliders to the same HDM.")]
        //TODO: Automatically determine this on Start()
        [SerializeField] HitDetectionManager parentHDM;
        HealthManager healthManager;

        public event Action<int, HitDetectionManager> OnDamageTakenWithSource;
        public event Action<int> OnDamageTaken;

        private void Start()
        {
            if (!healthManager && !parentHDM)
            {
                if (TryGetComponent(out healthManager))
                {
                    //Debug.Log("Health manager automatically applied.");
                }
                else
                {
                    Debug.LogError("No health manager found! Are you creating an orphan HDM or a health manager?");
                }
            }
        }

        public void TakeDamage(int damage)
        {
            OnDamageTakenWithSource?.Invoke(damage, this);
            if (parentHDM)
            {
                //NOTE: There are currently no reasons to know the damage source but it is easy to add. If the information is needed in the future, implement this branch.
                parentHDM.OnDamageTakenWithSource?.Invoke(damage, this);
                parentHDM.TakeDamage(damage);
            }
            else
            {
                //Handle damage, talk to the HealthManager
                healthManager.TakeDamage(damage);
            }
        }
    }

}