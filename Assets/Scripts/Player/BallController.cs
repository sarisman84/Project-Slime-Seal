using System;
using UnityEngine;

namespace Player
{
    [RequireComponent(typeof(Input))]
    public class BallController : MonoBehaviour
    {
        // Start is called before the first frame update

        private Input m_InputComponent;
        private Rigidbody m_Rigidbody;
        private Camera _mainCamera;

        public float accelerationSpeed = 4f;
        [SerializeField] private float defaultMaxMovementSpeed;

        private float _currentMaxMovementSpeed;
        private Vector3 m_Input;
        private Vector3 _localDirection;


        private float TrueSpeed => accelerationSpeed * 100f;
        private float MaxSpeed => _currentMaxMovementSpeed * 100f;
        public Vector3 MovementDirection => m_Rigidbody.velocity.normalized;

        private void SetMaxMovementSpeed(float limit = 0)
        {
            if (limit == 0)
                limit = defaultMaxMovementSpeed;
            _currentMaxMovementSpeed = limit;
        }


        void Awake()
        {
            _mainCamera = Camera.main;
            m_InputComponent = GetComponent<Input>();
            m_Rigidbody = GetComponent<Rigidbody>();

            SetMaxMovementSpeed(defaultMaxMovementSpeed);
        }

        private void Update()
        {
            m_Input = m_InputComponent.GetInputMovementRaw(AxisType.Axis3D);
            _localDirection = _mainCamera.transform.right * m_Input.x + _mainCamera.transform.forward * m_Input.z;
        }

        // Update is called once per frame
        void FixedUpdate()
        {
            m_Rigidbody.AddForce(ClampSpeed(_localDirection *
                                            (TrueSpeed * Time.fixedDeltaTime)) + _mainCamera.transform.right, ForceMode.Force);
            m_Rigidbody.velocity = ClampSpeed(m_Rigidbody.velocity);
            Debug.DrawRay(transform.position, m_Rigidbody.velocity, Color.green);
        }

        private Vector3 ClampSpeed(Vector3 direction)
        {
            return Vector3.ClampMagnitude(direction, MaxSpeed);
        }
    }
}