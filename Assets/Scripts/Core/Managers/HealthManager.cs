// HealthManager is responsible for managing the health and shields of objects 
// that implement IDamageable. When an object gets hit, the HealthManager will
// be the one to apply any health or shield modifications. It will rely on 
// IDamageable to know what it should manage and will interact with the 
// HitDetectionManager to know when something gets hit.
using System;
using UnityEngine;
using System.Runtime.CompilerServices;
[assembly: InternalsVisibleTo("EditHealthTests")]

namespace Endsley
{
    public class HealthManager : MonoBehaviour, IDamageable
    {
        [SerializeField] internal HealthConfig healthConfig;

        private IDie deathBehavior;
        [SerializeField][ReadOnly] int currentHealth;
        [SerializeField][ReadOnly] int currentShields;
        private bool isDead = false;
        // For all instances of damage
        public event Action<int> OnDamageTaken;
        //For health and shields specific damage (UI)
        public event Action<int> OnHealthDamageTaken;
        public event Action<int> OnShieldsDamageTaken;

        internal void Start()
        {
            currentHealth = healthConfig.maxHealth;
            currentShields = healthConfig.maxShields;

            if (!TryGetComponent(out deathBehavior))
            {
                Debug.LogWarning("No death behavior attached.");
            }
        }

        public void TakeDamage(int damage)
        {
            if (!isDead)
            {
                Debug.Log("HealthManager: Damage taken");
                //TODO: potential YAGNI but shield overkill mechanic might be interesting
                OnDamageTaken?.Invoke(damage);
                if (damage < 0)
                {
                    Debug.LogWarning("<0 damage taken, this could indicate a problem");
                    return;
                }
                else
                {
                    // You have shields left
                    if (currentShields > 0)
                    {
                        currentShields = Math.Max(currentShields - damage, 0);
                        OnShieldsDamageTaken?.Invoke(damage);
                    }
                    else
                    {
                        // You have health (hull) left
                        if (currentHealth > 0)
                        {
                            currentHealth = Math.Max(currentHealth - damage, 0);
                            OnHealthDamageTaken?.Invoke(damage);
                        }
                        else
                        {
                            Debug.Log("This object should be dead");
                        }
                    }
                }
                if (!isDead && currentHealth == 0 && currentShields == 0)
                {
                    isDead = true;
                    deathBehavior?.Die();
                }
            }
        }

        public void SetHealthManagerConfig(HealthConfig healthConfig)
        {
            this.healthConfig = healthConfig;
        }

        public int GetCurrentHealth()
        {
            return currentHealth;
        }

        public int GetCurrentShields()
        {
            return currentShields;
        }

        public bool SetIsDead(bool value)
        {
            bool val = isDead;
            isDead = true;
            return val;
        }

    }
}