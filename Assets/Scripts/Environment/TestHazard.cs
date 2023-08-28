using System.Collections;
using UnityEngine;

namespace Endsley
{
    public class TestHazard : MonoBehaviour, IDamageSource
    {

        [SerializeField] int cooldown = 5;
        [SerializeField] int damage = 10;

        private bool canDealDamage = true;

        public void DealDamageTo(IDamageable target, int amount)
        {
            Debug.Log("Damage Dealt");
            target.TakeDamage(amount);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.TryGetComponent(out IDamageable damageable) && canDealDamage)
            {
                DealDamageTo(damageable, damage);
                StartCoroutine(Cooldown());
            }
        }

        private IEnumerator Cooldown()
        {
            Renderer rend = GetComponent<Renderer>();

            canDealDamage = false;
            rend.material.color = Color.red; // Turns red when can't deal damage

            yield return new WaitForSeconds(cooldown);

            canDealDamage = true;
            rend.material.color = Color.green; // Turns green when can deal damage again
        }
    }
}
