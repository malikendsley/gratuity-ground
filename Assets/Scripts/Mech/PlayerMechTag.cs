using UnityEngine;

namespace Endsley
{
    public class PlayerMechTag : MonoBehaviour
    {
        public static PlayerMechTag Instance { get; private set; }
        // Easily allow other scripts to get the player mech
        public GameObject PlayerMech { get; set; }
        public GameObject PlayerCoM { get; set; }
        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                PlayerMech = gameObject;
            }
            else
            {
                Debug.LogWarning("Multiple instances of PlayerMechTag detected. Destroying the new one.");
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            PlayerCoM = transform.Find("CoM").gameObject;
        }
    }
}