using System;
using UnityEngine;

namespace Player
{
    public class CameraController : MonoBehaviour
    {
        public Transform focalPoint;
        public void Update()
        {
            focalPoint.position = transform.position;
        }
    }
}