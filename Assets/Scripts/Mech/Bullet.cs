using UnityEngine;
using System;

namespace Endsley
{
    public class Bullet : MonoBehaviour
    {
        public float speed = 50f;
        public int damage = 1;
        public BulletAllegiance allegiance;
        public LayerMask interactableLayers;  // Layers that this bullet can interact with
        [Tooltip("Seconds before returning to the pool")]
        public float lifetime = 5f;

        private Transform target;  // Set this if aim-assisted

        private float timeToLive;  // Timer to track bullet's lifetime
        private bool isAimAssisted = false;
        private Vector3 direction = Vector3.zero;
        void Start()
        {
            timeToLive = 0f;  // Initialize the lifetime timer
        }

        public void InitNoAimAssist(Vector3 position, Vector3 direction, BulletAllegiance allegiance)
        {
            this.allegiance = allegiance;
            transform.position = position;
            isAimAssisted = false;
            timeToLive = 0f;  // Reset the lifetime timer
            this.direction = direction.normalized;
        }

        public void InitAimAssist(Vector3 position, BulletAllegiance allegiance, Transform target)
        {
            Debug.Log("Init aim assist");
            if (target == null)
            {
                Debug.LogWarning("Target is null");
            }
            this.allegiance = allegiance;
            transform.position = position;
            isAimAssisted = true;
            this.target = target;
        }

        void Update()
        {
            timeToLive += Time.deltaTime;
            // If aim assist is true, track the target
            if (isAimAssisted)
            {
                // Move speed units towards the target
                transform.position = Vector3.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
            }
            else
            {
                // Move speed units in the direction of the bullet
                transform.position += speed * Time.deltaTime * direction;
            }
            // Check if the bullet should be returned to the pool
            if (timeToLive >= lifetime)
            {
                ProjectilePooling.Instance.ReturnBullet(this);
            }
        }

        void OnTriggerEnter(Collider other)
        {
            // Check if the other object is on an interactable layer
            if (((1 << other.gameObject.layer) & interactableLayers) != 0)
            {
                // try to retrieve the HitDetectionManager from the other object
                if (other.TryGetComponent(out HitDetectionManager hitDetectionManager))
                {
                    if (hitDetectionManager.GetBulletAllegiance() == allegiance)
                    {
                        Debug.Log("Prevented friendly fire");
                        // If the other object is on the same team, don't hit it
                        return;
                    }
                    // TODO: use allegiance to make only enemies hit the player and vice versa
                    // if the other object has a HitDetectionManager, tell it that it was hit
                    hitDetectionManager.TakeDamage(damage);
                    Debug.Log("Bullet hit " + other.gameObject.name);

                }
                ProjectilePooling.Instance.ReturnBullet(this);
            }
        }
    }
}
