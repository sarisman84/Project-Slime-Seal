using System;
using UnityEngine;

namespace Player
{
    public class CameraController : MonoBehaviour
    {
        public Transform focalPoint;

        public void SetCursorState(bool state)
        {
            Cursor.lockState = state ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = state;
        }
        
        private void Awake()
        {
            SetCursorState(false);
        }

        public void Update()
        {
            focalPoint.position = transform.position;
        }
    }
}