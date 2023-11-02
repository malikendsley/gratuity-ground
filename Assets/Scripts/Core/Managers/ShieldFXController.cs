// Provides a simple component to allow for shields to be damaged and destroyed.
using System;
using UnityEngine;

namespace Endsley
{
    public class ShieldFXController : MonoBehaviour
    {
        private HealthManager healthManager;
        // Debounce
        private bool shieldsUp = true;
        // Shield Renderer
        [SerializeField] private Animator shieldAnimator;

        // This is fired any time the health manager takes damage
        public void UpdateShields((int health, int shields) hs)
        {
            Debug.Log($"ShieldController - HP: {hs.health} Shields: {hs.shields}");
            // Shields hit but not break
            if (hs.shields > 0 && hs.shields < healthManager.healthConfig.maxShields)
            {
                ShieldDamage();
            }
            // Shields break
            else if (hs.shields == 0 && shieldsUp)
            {
                ShieldBreak();
            }
            // Shields increase from 0 
            else if (hs.shields > 0 && !shieldsUp)
            {
                ShieldRecover();
            }
            // Shields regen
            else if (hs.shields == healthManager.healthConfig.maxShields)
            {
                ShieldRegen();
            }
        }

        void ShieldBreak()
        {
            shieldsUp = false;
            // Play shatter animation
            shieldAnimator.Play("Base Layer.Shatter");
        }
        void ShieldDamage()
        {
            shieldAnimator.Play("Base Layer.Damage");
        }
        void ShieldRecover()
        {
            shieldsUp = true;
        }
        void ShieldRegen()
        {

        }

        // Start is called before the first frame update
        void Start()
        {
            // Subscribe to the health manager's OnShieldsDamageTaken event
            if (TryGetComponent(out healthManager))
            {
                healthManager.OnHitStatsChanged += UpdateShields;
            }
            else
            {
                Debug.LogError("No health manager found! Are you creating an orphan HDM or a health manager?");
            }
        }
    }
}