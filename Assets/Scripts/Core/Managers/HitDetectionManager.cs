using System;
using UnityEngine;

namespace Endsley
{
    public enum BulletAllegiance
    {
        Ally,
        Enemy,
        Unset,
    }
    public class HitDetectionManager : MonoBehaviour, IDamageable
    {

        [Tooltip("This allows for things made of multiple game objects to connect multiple colliders to the same HDM.")]
        //TODO: Automatically determine this on Start()
        [SerializeField] HitDetectionManager parentHDM;
        HealthManager healthManager;

        public event Action<int, HitDetectionManager> OnDamageTakenWithSource;
        public event Action<int> OnDamageTaken;
        [Tooltip("This is the allegiance of the object that owns this HDM. If this is null, the allegiance will be determined by the parent HDM. If there is no parent HDM, the allegiance will be determined by the HealthManager.")]
        [SerializeField]
        private BulletAllegiance allegiance = BulletAllegiance.Unset;

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
            Transform currentTransform = transform;
            while (allegiance == BulletAllegiance.Unset)
            {
                // Look for parent HDM or HealthManager
                HitDetectionManager parentHDM = currentTransform.GetComponent<HitDetectionManager>();
                HealthManager healthManager = currentTransform.GetComponent<HealthManager>();

                if (parentHDM && parentHDM.allegiance != BulletAllegiance.Unset)
                {
                    allegiance = parentHDM.allegiance;
                    break;
                }
                else if (healthManager)
                {
                    allegiance = healthManager.GetBulletAllegiance();
                    if (allegiance != BulletAllegiance.Unset)
                        break;
                }

                // Move up to the parent transform for the next iteration
                if (currentTransform.parent != null)
                {
                    currentTransform = currentTransform.parent;
                }
                else
                {
                    // We've reached the top and found nothing
                    Debug.LogWarning("No allegiance set and no parent HDM or health manager found with set allegiance!");
                    break;
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

        public BulletAllegiance GetBulletAllegiance()
        {
            if (parentHDM)
            {
                return parentHDM.GetBulletAllegiance();
            }
            else
            {
                return allegiance;
            }
        }
    }

}