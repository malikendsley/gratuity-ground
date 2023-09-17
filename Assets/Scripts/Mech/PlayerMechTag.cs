using UnityEngine;

namespace Endsley
{
    public class PlayerMechTag : MonoBehaviour
    {
        public static PlayerMechTag Instance { get; private set; }
        // Easily allow other scripts to get the player mech
        public GameObject PlayerMech { get; set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                PlayerMech = gameObject;
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}