using UnityEngine;
using System;

namespace Endsley
{
    public class Bullet : MonoBehaviour
    {
        public float speed = 50f;  // Adjust based on your needs
        public BulletAllegiance allegiance;
        public float timeBeforePhysics = 1f;  // Time in seconds before physics take over

        public event Action OnReturnToPool;
        public Transform target;  // Set this if aim-assisted

        private Vector3 direction;
        private bool isAimAssisted;
        private Rigidbody rb;
        private float timeElapsed = 0f;
        void Start()
        {
            rb = GetComponent<Rigidbody>();
            rb.isKinematic = true;  // Initialize with kinematic on
        }

        public void Initialize(Vector3 position, Vector3 direction, BulletAllegiance allegiance, Transform target = null)
        {
            this.allegiance = allegiance;
            transform.position = position;
            this.direction = direction.normalized;
            this.target = target;
            isAimAssisted = target != null;
            timeElapsed = 0f;
        }

        void Update()
        {
            timeElapsed += Time.deltaTime;

            if (timeElapsed < timeBeforePhysics)
            {
                if (isAimAssisted)
                {
                    // Aim-assisted movement
                    direction = (target.position - transform.position).normalized;
                }
                // General movement, aim-assisted or not
                transform.position += speed * Time.deltaTime * direction;
            }
            else
            {
                if (rb.isKinematic)
                {
                    // Switch to physics-based movement for the first time
                    rb.isKinematic = false;
                    rb.velocity = speed * direction;
                }
                // Now physics will naturally take over
            }
        }

        public void ReturnToPool()
        {
            OnReturnToPool?.Invoke();
            // Return to pool logic
        }
    }
}
