using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using DG.Tweening;
using Interactivity;
using UnityEngine;
using UnityEngine.PlayerLoop;
using Vertex_Displacement;
using static UnityEngine.Object;
using Random = UnityEngine.Random;

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


        [SerializeField] private Transform sphereModel;
        [SerializeField] private CinemachineFreeLook _cameraBehaivour;
        [SerializeField] private BallEnlarger ballEnlarger;

        [SerializeField] float accelerationSpeed = 4f;
        [SerializeField] private float defaultMaxMovementSpeed;
        [SerializeField] private float jumpForce;
        [Space] [SerializeField] private float grabRadius;

        [SerializeField] private LayerMask grabMask;
        [SerializeField] private LayerMask hazardMask;
        [SerializeField] private LayerMask collisionMask;
        [SerializeField] private float scaleUpRate = 0.5f;
        [SerializeField] private float scaleDownRate = 0.01f;

        private float m_CurrentMaxMovementSpeed;
        private Vector3 m_Input;


        private float TrueSpeed => accelerationSpeed * 100f;
        private float MaxSpeed => m_CurrentMaxMovementSpeed * 100f;
        private float TotalJumpForce => jumpForce;
        private Vector3 GroundCollisionSize => new Vector3(_collider.radius, 0.1f, _collider.radius);

        private Vector3 BottomPositionOfCollider
        {
            get
            {
                var position = transform.position;
                return new Vector3(position.x, position.y - (_collider.bounds.size.y / 2f), position.z);
            }
        }

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
            ballEnlarger = new BallEnlarger(_collider, grabRadius, grabMask, hazardMask, _cameraBehaivour, sphereModel);
            m_InputComponent = GetComponent<Input>();
            m_Rigidbody = GetComponent<Rigidbody>();

            SetMaxMovementSpeed(defaultMaxMovementSpeed);
        }

        private void Update()
        {
            m_Input = m_InputComponent.GetInputMovementRaw(AxisType.Axis3D);
            ballEnlarger.PickupNearbyObjectsAndEnlarge();
            ballEnlarger.DecreaseInSizeOnCondition();
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

            if (m_InputComponent.GetButton(Input.InputType.Jump) && IsTouchingTheGround())
            {
                m_Rigidbody.AddForce(Vector3.up * TotalJumpForce, ForceMode.Impulse);
            }

            Debug.DrawRay(transform.position, m_Rigidbody.velocity, Color.green);
        }


        private bool IsTouchingTheGround()
        {
            List<Collider> foundObjects = Physics.OverlapBox(BottomPositionOfCollider,
                GroundCollisionSize, transform.rotation, collisionMask).ToList();
            return foundObjects.FindAll(c => c != _collider).Count != 0;
        }


        private Vector3 ClampSpeed(Vector3 direction)
        {
            return Vector3.ClampMagnitude(direction, MaxSpeed);
        }

        private void OnDrawGizmos()
        {
            if (_collider == null) return;
            Gizmos.color = Color.magenta;
            Gizmos.DrawCube(BottomPositionOfCollider, GroundCollisionSize);
            Gizmos.color = Color.red - new Color(0, 0, 0, 0.5f);
            Gizmos.DrawSphere(transform.position, _collider.radius);
            Gizmos.color = Color.green - new Color(0, 0, 0, 0.5f);
            Gizmos.DrawSphere(transform.position, _collider.radius + grabRadius);
        }
    }

    [Serializable]
    public class BallEnlarger
    {
        private List<GameObject> m_CaughtObjects;
        private SphereCollider m_SphereCollider;
        private LayerMask m_GrabMask;
        private LayerMask m_HazardMask;

        private float m_Radius;
        private float m_CurrentSize;
        private CinemachineFreeLook m_CinemachineFreeLook;
        private Transform m_SphereModel;

        private MeshRenderer m_SphereModel_meshRenderer;
        private Material m_SphereModel_material;

        private float m_TopOrbitHeight, m_MidOrbitRadius;

        private float m_SphereModel_defaultVDSpeed, m_SphereModel_defaultVDSize, m_SphereModel_defaultVDStrength;

        #region StaticDefinitions

        private static readonly int SlimeColor = Shader.PropertyToID("Slime_Color");
        private static readonly int VdSpeed = Shader.PropertyToID("VD_Speed");
        private static readonly int VdSize = Shader.PropertyToID("VD_Size");
        private static readonly int VdStrength = Shader.PropertyToID("VD_Strength");

        #endregion

        public BallEnlarger(SphereCollider collider, float radius, LayerMask layerMask, LayerMask hazardMask,
            CinemachineFreeLook cinemachineFreeLook, Transform sphereModel)
        {
            m_SphereCollider = collider;
            m_GrabMask = layerMask;
            m_HazardMask = hazardMask;
            m_Radius = radius;
            m_CinemachineFreeLook = cinemachineFreeLook;

            m_TopOrbitHeight = cinemachineFreeLook.m_Orbits[0].m_Height;
            m_MidOrbitRadius = cinemachineFreeLook.m_Orbits[1].m_Radius;

            m_SphereModel = sphereModel;

            m_CaughtObjects = new List<GameObject>();
            m_SphereModel_meshRenderer = m_SphereModel.GetComponent<MeshRenderer>();
            m_SphereModel_material = m_SphereModel_meshRenderer.material;

            m_SphereModel_defaultVDSpeed = SphereModel_VertexDisplacement_Speed;
            m_SphereModel_defaultVDSize = SphereModel_VertexDisplacement_Size;
            m_SphereModel_defaultVDStrength = SphereModel_VertexDisplacement_Strength;
        }

        private Color PSphereColor
        {
            get => m_SphereModel_material.GetColor(SlimeColor);
            set => m_SphereModel_material.SetColor(SlimeColor, value);
        }

        private float SphereModel_VertexDisplacement_Speed
        {
            get => m_SphereModel_material.GetFloat(VdSpeed);
            set => m_SphereModel_material.SetFloat(VdSpeed, value);
        }

        private float SphereModel_VertexDisplacement_Size
        {
            get => m_SphereModel_material.GetFloat(VdSize);
            set => m_SphereModel_material.SetFloat(VdSize, value);
        }

        private float SphereModel_VertexDisplacement_Strength
        {
            get => m_SphereModel_material.GetFloat(VdStrength);
            set => m_SphereModel_material.SetFloat(VdStrength, value);
        }


        public void PickupNearbyObjectsAndEnlarge()
        {
            m_CurrentSize = 0;
            Collider[] foundObjects = Physics.OverlapSphere(m_SphereCollider.transform.position,
                m_SphereCollider.radius + m_Radius, m_GrabMask);
            Debug.Log($"{foundObjects.Length} were found in {m_Radius} radius.");
            for (int i = 0; i < foundObjects.Length; i++)
            {
                BallAffector affector = foundObjects[i].GetComponent<BallAffector>();
                if (affector != null && affector.information.scaleType == BallAffectorInformation.ScaleType.ScaleUp)
                {
                    GameObject obj = foundObjects[i].gameObject;
                    m_CaughtObjects.Add(obj);
                    Destroy(obj.GetComponent<Collider>());
                    m_CurrentSize += affector.information.scaleRate;
                }
            }

            OnPickupObject();

            SetCollisionSize(m_CinemachineFreeLook, m_SphereModel);
        }

        public void OnPickupObject()
        {
            foreach (GameObject obj in m_CaughtObjects)
            {
                if (obj.transform.parent != m_SphereCollider.transform)
                {
                    obj.transform.DOMove(
                        m_SphereCollider.transform.position + (Random.insideUnitSphere *
                                                               Mathf.Clamp(m_SphereCollider.radius, 3f,
                                                                   float.MaxValue)), 0.15f);
                    obj.transform.SetParent(m_SphereCollider.transform);
                    obj.transform.localScale = Vector3.one * Random.Range(0.25f, 1f);
                }
            }
        }

        public void DecreaseInSizeOnCondition()
        {
            m_CurrentSize = 0;
            Collider[] foundObjects = Physics.OverlapSphere(m_SphereCollider.transform.position,
                m_SphereCollider.radius + 0.25f, m_HazardMask);

            for (int i = 0; i < foundObjects.Length; i++)
            {
                BallAffector affector = foundObjects[i].GetComponent<BallAffector>();
                if (foundObjects[i] != null && affector != null &&
                    affector.information.scaleType == BallAffectorInformation.ScaleType.ScaleDown)
                {
                    m_CurrentSize -= affector.information.scaleRate;
                    if (m_CaughtObjects.Count != 0)
                    {
                        GameObject obj = m_CaughtObjects[m_CaughtObjects.Count - 1];
                        if ((m_SphereCollider.transform.position - obj.transform.position).magnitude >
                            m_SphereCollider.radius)
                        {
                            obj.transform.SetParent(null);
                            m_CaughtObjects.Remove(obj);

                            if (obj.gameObject.activeSelf)
                            {
                                obj.gameObject.transform
                                    .DOMove(obj.gameObject.transform.position, Random.Range(0.2f, 0.5f)).OnComplete(
                                        () =>
                                        {
                                            obj.gameObject.AddComponent<Rigidbody>();
                                            obj.gameObject.AddComponent<BoxCollider>();
                                            obj.gameObject.layer = 0;
                                        });
                            }
                        }
                    }
                }
            }

            SetCollisionSize(m_CinemachineFreeLook, m_SphereModel);
        }

        private void SetCollisionSize(CinemachineFreeLook cinemachineFreeLook, Transform sphereModel)
        {
            m_SphereCollider.radius += m_CurrentSize;
            m_SphereCollider.radius = Mathf.Clamp(m_SphereCollider.radius, 1, float.MaxValue);

            cinemachineFreeLook.m_Orbits[0].m_Height += m_CurrentSize * 4f;
            cinemachineFreeLook.m_Orbits[0].m_Height = Mathf.Clamp(cinemachineFreeLook.m_Orbits[0].m_Height,
                m_TopOrbitHeight, float.MaxValue);

            cinemachineFreeLook.m_Orbits[1].m_Radius += m_CurrentSize * 4f;
            cinemachineFreeLook.m_Orbits[1].m_Radius = Mathf.Clamp(cinemachineFreeLook.m_Orbits[1].m_Radius,
                m_MidOrbitRadius, float.MaxValue);


            sphereModel.localScale = Vector3.one * (m_SphereCollider.radius * 2f);
            //sphereModel.localScale = Vector3.Min(Vector3.Max(sphereModel.localScale, Vector3.zero), sphereModel.localScale);
            SphereModel_VertexDisplacement_Size += m_CurrentSize;
            SphereModel_VertexDisplacement_Size = Mathf.Clamp(SphereModel_VertexDisplacement_Size,
                m_SphereModel_defaultVDSize, float.MaxValue);
        }
    }
}