using System;
using System.Collections;
using UnityEngine;

// Works in tandem with AmmoUIController
// Attached to each bullet icon
// When Fire()'d, the bullet icon will bounce up in an arc and then fall off screen
// once it is off screen, it will be destroyed
namespace Endsley
{
    public class BulletUIController : MonoBehaviour
    {

        public float xBounceForce = 10f;
        public float yBounceForce = 10f;
        public float offScreenDelay = 1f;
        public float simulatedGravity = 9.8f;
        public float duckDistance = 100f;
        public float slideUpTime = 0.1f;

        private float curXVelocity = 0f;
        private float curYVelocity = 0f;


        private RectTransform rectTransform;
        bool fired = false;
        public enum BulletDirection
        {
            Left,
            Right
        }

        void Awake()
        {
            rectTransform = GetComponent<RectTransform>();
        }

        public void Fire(BulletDirection direction)
        {
            // Pop the bullet up in an arc facing left or right based on parameter
            curXVelocity = xBounceForce * (direction == BulletDirection.Left ? -1 : 1);
            curYVelocity = yBounceForce;
            fired = true;
        }

        void Update()
        {
            // Accelerate downwards and update position
            if (fired)
            {
                curYVelocity -= simulatedGravity * Time.deltaTime;
                rectTransform.position += new Vector3(curXVelocity, curYVelocity, 0) * Time.deltaTime;
            }


            // If bullet is totally off screen, destroy it after delay
            if (fired && rectTransform.position.y < -Screen.height)
            {
                Destroy(gameObject, offScreenDelay);
            }
        }

        internal void Eject()
        {
            // Drop this bullet straight down and then destroy it
            curXVelocity = 0;
            curYVelocity = -yBounceForce;
            fired = true;
        }

        internal void Inject()
        {
            // Save original position
            Vector3 originalPosition = rectTransform.position;
            // Move bullet 100 px down
            rectTransform.position -= new Vector3(0, duckDistance, 0);
            // Start coroutine to slide bullet up over 0.1 seconds
            StartCoroutine(SlideBulletUp(originalPosition));
        }

        private IEnumerator SlideBulletUp(Vector3 targetPosition)
        {
            float elapsedTime = 0f;
            Vector3 startPosition = rectTransform.position;

            while (elapsedTime < slideUpTime)
            {
                // Calculate the lerp value
                float lerpValue = elapsedTime / slideUpTime;
                // Interpolate position from start to target
                rectTransform.position = Vector3.Lerp(startPosition, targetPosition, lerpValue);
                // Increment the time elapsed
                elapsedTime += Time.deltaTime;
                // Wait for the next frame
                yield return null;
            }

            // Ensure the final position is set accurately
            rectTransform.position = targetPosition;
        }
    }
}