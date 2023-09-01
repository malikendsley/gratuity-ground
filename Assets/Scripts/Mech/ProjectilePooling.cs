using System.Collections.Generic;
using UnityEngine;
namespace Endsley
{
    using UnityEngine;
    using System.Collections.Generic;
    public enum BulletAllegiance
    {
        Ally,
        Enemy
    }
    public class ProjectilePooling : MonoBehaviour
    {
        public static ProjectilePooling Instance;
        public GameObject bulletPrefab;
        public int poolSize = 50;
        private readonly Queue<Bullet> availableBullets = new();

        private void Awake()
        {
            Instance = this;
            for (int i = 0; i < poolSize; i++)
            {
                Bullet newBullet = Instantiate(bulletPrefab, transform).GetComponent<Bullet>();
                newBullet.gameObject.SetActive(false);
                availableBullets.Enqueue(newBullet);
            }
        }

        public Bullet GetBullet(BulletAllegiance allegiance)
        {
            if (availableBullets.Count == 0)
            {
                // Either grow your pool here or handle this case
                Debug.LogWarning("No bullets available in pool. Consider increasing pool size.");
                return null;
            }

            Bullet bullet = availableBullets.Dequeue();
            bullet.allegiance = allegiance; // You can define Allegiance in your Bullet script
            bullet.gameObject.SetActive(true);
            return bullet;
        }

        public void ReturnBullet(Bullet bullet)
        {
            bullet.gameObject.SetActive(false);
            availableBullets.Enqueue(bullet);
        }
    }

}