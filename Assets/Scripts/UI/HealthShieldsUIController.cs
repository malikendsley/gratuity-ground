using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Endsley
{
    public class HealthShieldsUIController : MonoBehaviour
    {
        [SerializeField] private HealthManager healthManager;
        // These have sliders, this script will manage their values
        [SerializeField] private Slider healthBarSlider;
        [SerializeField] private Slider shieldBarSlider;
        // Start is called before the first frame update
        void Start()
        {
            healthManager.OnHitStatsChanged += UpdateHealth;
            // Retrieve the max health and shields from the health manager
            var healthConfig = healthManager.GetHealthManagerConfig();
            healthBarSlider.maxValue = healthConfig.maxHealth;
            shieldBarSlider.maxValue = healthConfig.maxShields;
            // Set the initial values
            healthBarSlider.value = healthConfig.maxHealth;
            shieldBarSlider.value = healthConfig.maxShields;

        }

        private void UpdateHealth((int, int) tuple)
        {
            healthBarSlider.value = tuple.Item1;
            shieldBarSlider.value = tuple.Item2;
        }
    }
}
