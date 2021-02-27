using System;
using System.Timers;
using UnityEngine;

namespace Interactivity
{
    public class AffectRigidbodies : MonoBehaviour
    {
        public Vector3 direction;
        public float forceAmm;
        public float pushRatePerSecond;
        public bool showDebugLogs;


        private float m_CurrentTimer;


        public void PushRigidbody(Collider other)
        {
            float trueForce = forceAmm * 100f;
            if (showDebugLogs)
                Debug.Log(
                    $"{(other.attachedRigidbody != null ? $"{gameObject.name} has found a rigidbody" : $"{gameObject.name} couldn't find a rigidbody")}");
            if (other.attachedRigidbody != null && m_CurrentTimer > pushRatePerSecond)
            {
                if (showDebugLogs)
                    Debug.Log($"{gameObject.name} is pushing {other.name} towards its designated direction with {trueForce} amounts of force");
                other.attachedRigidbody.AddForce(direction.normalized * trueForce);

                if (pushRatePerSecond == 0) return;
                m_CurrentTimer = 0;
            }
        }

        private void Update()
        {
            m_CurrentTimer += Time.deltaTime;
        }
    }
}