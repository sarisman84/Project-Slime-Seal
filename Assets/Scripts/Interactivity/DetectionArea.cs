using System;
using UnityEngine;
using UnityEngine.Events;

namespace Interactivity
{
    public class DetectionArea : MonoBehaviour
    {
        public BoxCollider m_BoxCollider;

        public UnityEvent<Collider> onTriggerEnter, onTriggerStay, onTriggerExit;
        public Color triggerBoxColor;

        private void OnTriggerEnter(Collider other)
        {
            onTriggerEnter?.Invoke(other);
        }

        private void OnTriggerExit(Collider other)
        {
            onTriggerExit?.Invoke(other);
        }

        private void OnTriggerStay(Collider other)
        {
            onTriggerStay?.Invoke(other);
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = triggerBoxColor;
            Gizmos.DrawCube(transform.position, Vector3.Max(m_BoxCollider.bounds.size, transform.localScale));
        }

        private void OnValidate()
        {
            if (m_BoxCollider == null && GetComponent<BoxCollider>() == null)
            {
                m_BoxCollider = gameObject.AddComponent<BoxCollider>();
                m_BoxCollider.isTrigger = true;
            }
               
        }
    }
}