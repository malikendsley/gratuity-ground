using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Endsley
{
    public class UIWorldSpaceConstantSize : MonoBehaviour
    {
        public float tuningFactor = .02f;
        private Camera mainCamera;
        void Awake()
        {
            if (mainCamera == null)
            {
                mainCamera = Camera.main;
            }
        }
        void Update()
        {
            transform.localScale = tuningFactor * Vector3.Distance(transform.position, mainCamera.transform.position) * Vector3.one;
        }
    }
}
