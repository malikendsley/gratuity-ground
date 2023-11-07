using UnityEngine;
using UnityEngine.UI;


// Allows updating UI in response to any object with a HealthManager component
namespace Endsley
{
    public class HealthShieldsUIController : MonoBehaviour
    {
        [SerializeField] private HealthManager healthManager;
        // These have sliders, this script will manage their values
        [SerializeField] private Image healthBar;
        [SerializeField] private Image shieldBar;

        float maxHealth;
        float maxShields;
        // Start is called before the first frame update
        void Start()
        {
            healthManager.OnHitStatsChanged += UpdateHealth;
            // Retrieve the max health and shields from the health manager
            var healthConfig = healthManager.GetHealthManagerConfig();
            maxHealth = healthConfig.maxHealth;
            maxShields = healthConfig.maxShields;
            // Set the initial values, unless they are 0
            healthBar.fillAmount = maxHealth > 0 ? 1 : 0;
            shieldBar.fillAmount = maxShields > 0 ? 1 : 0;

        }

        private void UpdateHealth((int, int) tuple)
        {
            healthBar.fillAmount = tuple.Item1 / maxHealth;
            shieldBar.fillAmount = tuple.Item2 / maxShields;
        }

        private void OnDisable()
        {
            healthManager.OnHitStatsChanged -= UpdateHealth;
            // Set health and shields to 0 (Has the effect of hiding the UI)
            // TODO: May also hide the container, in which case would need a reference to it
            healthBar.fillAmount = 0;
            shieldBar.fillAmount = 0;
        }
    }
}
