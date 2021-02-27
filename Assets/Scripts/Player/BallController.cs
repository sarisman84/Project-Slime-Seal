using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using DG.Tweening;
using Extensions;
using Interactivity;
using UnityEngine;
using UnityEngine.PlayerLoop;
using Vertex_Displacement;
using static UnityEngine.Object;
using Random = UnityEngine.Random;

namespace Player
{
    [RequireComponent(typeof(Input), typeof(SizeController))]
    public class BallController : MonoBehaviour
    {
        // Start is called before the first frame update

        private Input m_InputComponent;
        private Rigidbody m_Rigidbody;
        private Camera m_MainCamera;
        private SphereCollider m_Collider;
        private bool m_HasAlreadyJumped;


        [SerializeField] float accelerationSpeed = 4f;
        [SerializeField] private float maxMovementSpeed;
        [SerializeField] private float jumpForce;


        [SerializeField] private LayerMask collisionMask;

        private float _mCurrentVelocity;
        private Vector3 m_Input;
        internal SizeController SizeController;


        public CinemachineFreeLook PlayerCam => SizeController.playerCamera;

        private float TrueSpeed => accelerationSpeed * 100f;


        // private float MaxSpeed => m_CurrentMaxMovementSpeed * 100f;
        private float TotalJumpForce => jumpForce;
        private Vector3 GroundCollisionSize => new Vector3(m_Collider.radius, 0.1f, m_Collider.radius);

        private Vector3 BottomPositionOfCollider
        {
            get
            {
                var position = transform.position;
                return new Vector3(position.x, position.y - (m_Collider.bounds.size.y / 2f), position.z);
            }
        }

        private Vector3 RelativeDirection =>
            new Vector3(m_MainCamera.transform.right.x, 0, m_MainCamera.transform.right.z) * m_Input.x +
            new Vector3(m_MainCamera.transform.forward.x, 0, m_MainCamera.transform.forward.z) * m_Input.z;

        public float CurrentSize => SizeController.ballSize;
        public float PhysicsBias { get; set; } = 1f;
        public ForceMode PhysicsMode { get; set; } = ForceMode.Acceleration;
        public float InputBias { get; set; } = 1;

        private float CurrentMovementSpeed => maxMovementSpeed + (SizeController.ballSize);


        void Awake()
        {
            m_MainCamera = Camera.main;
            m_Collider = GetComponent<SphereCollider>();
            m_InputComponent = GetComponent<Input>();
            m_Rigidbody = GetComponent<Rigidbody>();
            SizeController = GetComponent<SizeController>();
        }

        private void Update()
        {
            m_Input = m_InputComponent.GetInputMovementRaw(AxisType.Axis3D);
        }

        void FixedUpdate()
        {
            m_Rigidbody.AddClampedForce(
                RelativeDirection * (TrueSpeed * Time.fixedDeltaTime * InputBias) + m_MainCamera.transform.right,
                ref _mCurrentVelocity,
                CurrentMovementSpeed, 0.15f,
                PhysicsMode, 30f, 0.2f, 0.05f);


            if (m_InputComponent.GetButton(Input.InputType.Jump) && IsTouchingTheGround() && !m_HasAlreadyJumped)
            {
                m_Rigidbody.AddForce(Vector3.up * TotalJumpForce, ForceMode.VelocityChange);
                m_HasAlreadyJumped = true;
            }
            else if (!IsTouchingTheGround())
            {
                m_HasAlreadyJumped = false;
            }

            Debug.DrawRay(transform.position, m_Rigidbody.velocity, Color.green);
        }


        private bool IsTouchingTheGround()
        {
            List<Collider> foundObjects = Physics.OverlapBox(BottomPositionOfCollider,
                GroundCollisionSize, transform.rotation, collisionMask).ToList();
            return foundObjects.Count != 0;
        }


        private void OnDrawGizmos()
        {
            if (m_Collider == null) return;
            Gizmos.color = Color.magenta;
            Gizmos.DrawCube(BottomPositionOfCollider, GroundCollisionSize);
            Gizmos.color = Color.red - new Color(0, 0, 0, 0.5f);
            var position = transform.position;
            Gizmos.DrawSphere(position, m_Collider.radius);
            Gizmos.color = Color.green - new Color(0, 0, 0, 0.5f);
            Gizmos.DrawSphere(position, SizeController.ballSize);
        }
    }
}