using UnityEngine;
using System.Collections;

namespace Endsley
{
    public class Missile : MonoBehaviour
    {
        [SerializeField] private BulletAllegiance bulletAllegiance;
        private GameObject target;
        private Vector3 targetPosition;
        private bool shouldTrackTarget = true;
        [SerializeField] private float speed = 20f;
        [SerializeField] private float arcHeight = 10f;
        [SerializeField] private bool missileActive = true;
        [SerializeField] private GameObject explosionPrefab;
        // Collide mask
        //TODO: If there is hit reg issues re-enable and implement this
        // [SerializeField] private LayerMask collideMask;
        // Time that missile was launched
        private float launchTime;
        // Collisions will not detonate the missile before this time
        [SerializeField] private float IFFtimer = 0.25f;

        public void Initialize(GameObject target, BulletAllegiance bulletAllegiance)
        {
            launchTime = Time.time;
            this.bulletAllegiance = bulletAllegiance;
            this.target = target;
            shouldTrackTarget = true;
            StartCoroutine(ArcToTarget());
        }

        public void Initialize(Vector3 target, BulletAllegiance bulletAllegiance)
        {
            launchTime = Time.time;
            this.bulletAllegiance = bulletAllegiance;
            targetPosition = target;
            shouldTrackTarget = false;
            StartCoroutine(ArcToTarget());
        }

        // If target, update targetposition to target's position
        // arc to targetposition
        private IEnumerator ArcToTarget()
        {
            Vector3 startPoint = transform.position;
            Vector3 endPoint;

            float t = 0f;
            while (t < 1f)
            {
                endPoint = shouldTrackTarget ? target.transform.position : targetPosition;
                Vector3 controlPoint = startPoint + (endPoint - startPoint) / 2 + Vector3.up * arcHeight;
                t += Time.deltaTime * speed;
                Vector3 m1 = Vector3.Lerp(startPoint, controlPoint, t);
                Vector3 m2 = Vector3.Lerp(controlPoint, endPoint, t);
                transform.position = Vector3.Lerp(m1, m2, t);

                // Update rotation to face target
                Vector3 direction = endPoint - transform.position;
                if (direction != Vector3.zero)
                {
                    transform.rotation = Quaternion.LookRotation(direction);
                }

                yield return null;
            }
            // Dumb fire missiles should continue to fly forward
            if (!shouldTrackTarget)
            {
                while (missileActive)
                {
                    transform.position += speed * 100 * Time.deltaTime * transform.forward;
                    yield return null;
                }
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!missileActive) return;
            Debug.Log("Missile collide: " + other.gameObject.name);
            // if other object has a hit detection manager, and the allegiance is not the same as this missile, apply damage
            if (other.gameObject.TryGetComponent(out HitDetectionManager otherHDM) && otherHDM.GetBulletAllegiance() != bulletAllegiance)
            {
                Debug.Log("Missile hit " + other.gameObject.name + " with allegiance " + otherHDM.GetBulletAllegiance());
                //TODO: Actual damage
                otherHDM.TakeDamage(1);
                Detonate();
            }
            // if IFFtimer has elapsed, detonate
            else if (Time.time - launchTime > IFFtimer)
            {
                Debug.Log("Missile hit " + other.gameObject.name + " with layer " + other.gameObject.layer);
                Detonate();
            }
        }

        void Detonate()
        {
            // Disable renderer and destroy after 2 seconds (in case any cleanup needs to happen)
            missileActive = false;
            GetComponent<Renderer>().enabled = false;
            var explosion = Instantiate(explosionPrefab, transform.position, transform.rotation);
            Destroy(explosion, 2f);
            Destroy(gameObject, 1f);
        }

    }

}

