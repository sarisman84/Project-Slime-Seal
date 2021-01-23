using System;
using Interactivity;
using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(Input))]
    public class InteractionController : MonoBehaviour
    {
        public LayerMask interactionMask;
        public float rerticleRadius, rerticleLength;
        private Input m_Input;

        private Camera m_Camera;
        private Collider m_Collider;
        private Vector3 m_hitPoint;

        private Ray AimRay => new Ray(transform.position, m_Camera.transform.forward.normalized);
        private float TotalLength => (transform.position - m_Camera.transform.position).sqrMagnitude + rerticleLength;

        // Start is called before the first frame update
        private void Start()
        {
            m_Collider = GetComponent<Collider>();
        }

        void Awake()
        {
            m_Input = GetComponent<Input>();
            m_Camera = Camera.main;
        }

        // Update is called once per frame
        void Update()
        {
            if (m_Input.GetButtonDown(Input.InputType.Interact) && m_Collider != null)
            {
                GetNearestObjectFromRerticle()?.OnInteract(m_Collider);
            }
        }

        
        public Interactable GetNearestObjectFromRerticle()
        {
            Collider[] foundObjects = default;
            Color rayColor = Color.red;
            if (Physics.Raycast(AimRay, out var hitInfo, TotalLength))
            {
                m_hitPoint = hitInfo.point;
                rayColor = Color.green;
                foundObjects = Array.FindAll(Physics.OverlapSphere(hitInfo.point, rerticleRadius, interactionMask),
                    c => c.gameObject.GetInstanceID() != gameObject.GetInstanceID());

                float dist = float.MaxValue;
                Interactable interactable = default;

                foreach (Collider col in foundObjects)
                {
                    Vector3 vect = (m_Camera.transform.forward.normalized * TotalLength) - col.transform.position;

                    Interactable potentialResult = col.GetComponent<Interactable>();
                    if (vect.sqrMagnitude < dist && potentialResult != null)
                    {
                        dist = vect.sqrMagnitude;
                        interactable = potentialResult;
                    }
                }
                
                Debug.DrawRay(AimRay.origin, AimRay.direction * TotalLength,rayColor);
                return interactable;
            }
            Debug.DrawRay(AimRay.origin, AimRay.direction * TotalLength,rayColor);
            return default;
        }


        private void OnDrawGizmos()
        {
            // Gizmos.color = Color.green;
            // Gizmos.DrawRay(AimRay.origin, AimRay.direction * rerticleLength);
            Gizmos.color = Color.green - new Color(0,0,0,0.5f);
            Gizmos.DrawSphere(m_hitPoint, rerticleRadius);
        }
    }
}