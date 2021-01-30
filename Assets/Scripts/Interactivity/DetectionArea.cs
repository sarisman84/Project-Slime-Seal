using System;
using UnityEngine;
using UnityEngine.Events;

namespace Interactivity
{
    public partial class DetectionArea : MonoBehaviour
    {
        public BoxCollider m_BoxCollider;

        public string targetTag;
        public UnityEvent<Collider> onTriggerEnter, onTriggerStay, onTriggerExit;
        public Color onSelectTriggerBoxColor;
        public Color triggerBoxColor;

        private void OnTriggerEnter(Collider other)
        {
            if (IsConditionMet(other)) onTriggerEnter?.Invoke(other);
        }

        private bool IsConditionMet(Collider other)
        {
            return other.gameObject.CompareTag(targetTag);
        }

        private void OnTriggerExit(Collider other)
        {
            if (IsConditionMet(other))
                onTriggerExit?.Invoke(other);
        }

        private void OnTriggerStay(Collider other)
        {
            if (IsConditionMet(other))
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