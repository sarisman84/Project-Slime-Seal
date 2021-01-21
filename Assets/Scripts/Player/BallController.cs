using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace Player
{
    [RequireComponent(typeof(Input))]
    public class BallController : MonoBehaviour
    {
        // Start is called before the first frame update

        private Input m_InputComponent;
        private Rigidbody m_Rigidbody;
        private Camera _mainCamera;
        private SphereCollider _collider;
        [SerializeField] private BallEnlarger ballEnlarger;

        [SerializeField] float accelerationSpeed = 4f;
        [SerializeField] private float defaultMaxMovementSpeed;
        [Space] [SerializeField] private float grabRadius;
        [SerializeField] private LayerMask grabMask;
        [SerializeField] private float scaleRate = 0.5f;

        private float m_CurrentMaxMovementSpeed;
        private Vector3 m_Input;


        private float TrueSpeed => accelerationSpeed * 100f;
        private float MaxSpeed => m_CurrentMaxMovementSpeed * 100f;
        public Vector3 MovementDirection => m_Rigidbody.velocity.normalized;

        private void SetMaxMovementSpeed(float limit = 0)
        {
            if (limit == 0)
                limit = defaultMaxMovementSpeed;
            m_CurrentMaxMovementSpeed = limit;
        }


        void Awake()
        {
            _mainCamera = Camera.main;
            _collider = GetComponent<SphereCollider>();
            ballEnlarger = new BallEnlarger(_collider, grabRadius, grabMask);
            m_InputComponent = GetComponent<Input>();
            m_Rigidbody = GetComponent<Rigidbody>();

            SetMaxMovementSpeed(defaultMaxMovementSpeed);
        }

        private void Update()
        {
            m_Input = m_InputComponent.GetInputMovementRaw(AxisType.Axis3D);
            ballEnlarger.PickupNearbyObjectsAndEnlarge(scaleRate);
        }

        private Vector3 RelativeDirection =>
            _mainCamera.transform.right * m_Input.x + _mainCamera.transform.forward * m_Input.z;

        // Update is called once per frame
        void FixedUpdate()
        {
            m_Rigidbody.AddForce(ClampSpeed(RelativeDirection *
                                            (TrueSpeed * Time.fixedDeltaTime)) + _mainCamera.transform.right,
                ForceMode.Force);
            m_Rigidbody.velocity = ClampSpeed(m_Rigidbody.velocity);

            Debug.DrawRay(transform.position, m_Rigidbody.velocity, Color.green);
        }

        private Vector3 ClampSpeed(Vector3 direction)
        {
            return Vector3.ClampMagnitude(direction, MaxSpeed);
        }

        private void OnDrawGizmos()
        {
            if (_collider == null) return;
            Gizmos.color = Color.red - new Color(0, 0, 0, 0.5f);
            Gizmos.DrawSphere(transform.position, _collider.radius);
            Gizmos.color = Color.green - new Color(0, 0, 0, 0.5f);
            Gizmos.DrawSphere(transform.position, _collider.radius + grabRadius);
        }
    }

    [Serializable]
    public class BallEnlarger
    {
        private SphereCollider m_SphereCollider;
        private LayerMask m_grabMask;
        private Dictionary<int, GameObject> m_CapturedObjects;
        private float m_radius;
        private float currentSize;

        public Dictionary<int, GameObject> CurrentlyGrabbedObjects => m_CapturedObjects;

        public BallEnlarger(SphereCollider collider, float radius, LayerMask layerMask)
        {
            m_SphereCollider = collider;
            m_grabMask = layerMask;
            m_radius = radius;
            m_CapturedObjects = new Dictionary<int, GameObject>();
        }


        public void PickupNearbyObjectsAndEnlarge(float sizeIncrement)
        {
            currentSize = 0;
            Collider[] foundObjects = Physics.OverlapSphere(m_SphereCollider.transform.position, m_SphereCollider.radius +m_radius, m_grabMask);
            Debug.Log($"{foundObjects.Length} were found in {m_radius} radius.");
            for (int i = 0; i < foundObjects.Length; i++)
            {
                foundObjects[i].gameObject.SetActive(false);
                currentSize += sizeIncrement;
            }

            SetCollisionSize();
        }

        public void SetCollisionSize()
        {
            m_SphereCollider.radius += currentSize;
        }
    }
}