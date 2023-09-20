using UnityEngine;
using System.Collections;

namespace Endsley
{
    public class Missile : MonoBehaviour
    {
        [SerializeField] private BulletAllegiance bulletAllegiance;
        private GameObject target;
        [SerializeField] private float speed = 20f;
        [SerializeField] private float arcHeight = 10f;
        [SerializeField] private float fudgeFactor = 0.3f;

        public void Initialize(GameObject target, BulletAllegiance bulletAllegiance)
        {
            this.bulletAllegiance = bulletAllegiance;
            this.target = target;
            StartCoroutine(ArcToTarget());
        }

        private IEnumerator ArcToTarget()
        {
            Vector3 startPoint = transform.position;
            Vector3 endPoint = target.transform.position;
            Vector3 controlPoint = startPoint + (endPoint - startPoint) / 2 + Vector3.up * arcHeight;

            float t = 0f;
            while (t < 1f)
            {
                t += Time.deltaTime * speed;
                Vector3 m1 = Vector3.Lerp(startPoint, controlPoint, t);
                Vector3 m2 = Vector3.Lerp(controlPoint, endPoint, t);
                transform.position = Vector3.Lerp(m1, m2, t);

                // Update rotation to face target
                Vector3 direction = (target.transform.position - transform.position).normalized;
                if (direction != Vector3.zero)
                {
                    transform.rotation = Quaternion.LookRotation(direction);
                }

                if (Vector3.Distance(transform.position, endPoint) <= fudgeFactor)
                {
                    // Destroy the missile or trigger some explosion effect
                    Destroy(gameObject);
                    break;
                }

                yield return null;
            }
        }
    }
}
