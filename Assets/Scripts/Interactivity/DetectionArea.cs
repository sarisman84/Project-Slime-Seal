using System;
using UnityEngine;
using UnityEngine.Events;

namespace Interactivity
{
    public partial class DetectionArea : MonoBehaviour
    {
        public BoxCollider m_BoxCollider;

        public UnityEvent<Collider> onTriggerEnter, onTriggerStay, onTriggerExit;
        public Color onSelectTriggerBoxColor;
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
            DrawCube(onSelectTriggerBoxColor);
        }

        private void DrawCube(Color boxColor)
        {
            DrawBoxCollider(boxColor, m_BoxCollider);
        }

        //Source: Paalo: https://forum.unity.com/threads/gizmo-rotation.4817/#post-5299893

        private void OnDrawGizmos()
        {
            DrawCube(triggerBoxColor);
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