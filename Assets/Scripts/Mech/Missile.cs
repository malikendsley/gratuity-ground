using UnityEngine;
using System.Collections;

namespace Endsley
{


    public class Missile : MonoBehaviour
    {
        [SerializeField] private BulletAllegiance bulletAllegiance;
        private GameObject target;
        //TODO: Make this a scriptable object
        [SerializeField] private float popUpHeight = 10f;
        [SerializeField] private float popUpSpeed = 20f;
        [SerializeField] private float cruiseSpeed = 20f;
        [Tooltip("The missile uses a few cutoffs to move between phases. Increasing this makes the missile feel 'snappier'")]
        [SerializeField] private float fudgeFactor = 0.3f;

        public void Initialize(GameObject target, BulletAllegiance bulletAllegiance)
        {
            this.bulletAllegiance = bulletAllegiance;
            this.target = target;
            StartCoroutine(MissileSequence());
        }

        private IEnumerator MissileSequence()
        {
            yield return StartCoroutine(PopUp());
            yield return StartCoroutine(Turn());
            yield return StartCoroutine(Cruise());
        }

        private IEnumerator PopUp()
        {
            Vector3 targetPosition = transform.position + Vector3.up * popUpHeight;
            while (Vector3.Distance(transform.position, targetPosition) > fudgeFactor)
            {
                // Parameterize this with a speed variable
                transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * popUpSpeed);
                yield return null;
            }
        }

        private IEnumerator Turn()
        {
            Vector3 direction = (target.transform.position - transform.position).normalized;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            // Have a generous cutoff for the angle, then just point the missile at the target
            while (Quaternion.Angle(transform.rotation, targetRotation) > fudgeFactor * 10)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime);
                yield return null;
            }
        }

        private IEnumerator Cruise()
        {
            while (Vector3.Distance(transform.position, target.transform.position) > fudgeFactor)
            {
                transform.position = Vector3.MoveTowards(transform.position, target.transform.position, Time.deltaTime * cruiseSpeed);
                yield return null;
            }
            // Here, you can destroy the missile or trigger some explosion effect
            Destroy(gameObject);
        }
    }
}
