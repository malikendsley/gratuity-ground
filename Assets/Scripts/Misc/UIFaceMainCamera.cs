using UnityEngine;

namespace Endsley
{

    public class UIFaceMainCamera : MonoBehaviour
    {
        public Camera mainCamera;

        void Awake()
        {
            if (mainCamera == null)
            {
                mainCamera = Camera.main;
            }
        }

        void Update()
        {
            transform.LookAt(transform.position + mainCamera.transform.rotation * Vector3.forward, mainCamera.transform.rotation * Vector3.up);
        }
    }
}