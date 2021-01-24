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
        private Camera m_MainCamera;
        private SphereCollider m_Collider;


        [SerializeField] private Transform sphereModel;
        [SerializeField] private CinemachineFreeLook cameraBehaivour;
        [SerializeField] private ParticleSystem sphereParticles;
        internal BallEnlarger m_BallEnlarger;

        [SerializeField] float accelerationSpeed = 4f;
        [SerializeField] private float defaultMaxMovementSpeed;
        [SerializeField] private float jumpForce;
        [Space] [SerializeField] private float grabRadius;

        [SerializeField] private LayerMask grabMask;
        [SerializeField] private LayerMask hazardMask;
        [SerializeField] private LayerMask collisionMask;

        private float m_CurrentMaxMovementSpeed;
        private Vector3 m_Input;


        public CinemachineFreeLook PlayerCam => cameraBehaivour;

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

        private void SetMaxMovementSpeed(float limit = 0)
        {
            if (limit == 0)
                limit = defaultMaxMovementSpeed;
            m_CurrentMaxMovementSpeed = limit;
        }


        void Awake()
        {
            m_MainCamera = Camera.main;
            m_Collider = GetComponent<SphereCollider>();
            m_BallEnlarger = new BallEnlarger(m_Collider, grabRadius, grabMask, hazardMask, cameraBehaivour,
                sphereModel,
                this);
            m_InputComponent = GetComponent<Input>();
            m_Rigidbody = GetComponent<Rigidbody>();

            SetMaxMovementSpeed(defaultMaxMovementSpeed);
        }

        private void Update()
        {
            m_Input = m_InputComponent.GetInputMovementRaw(AxisType.Axis3D);
            m_BallEnlarger.PickupNearbyObjectsAndEnlarge();
            m_BallEnlarger.DecreaseInSizeOnCondition();

            UpdateParticleEmitter();
        }

        private void UpdateParticleEmitter()
        {
            var sphereParticlesShape = sphereParticles.shape;
            sphereParticlesShape.radius = m_BallEnlarger.CurrentSize - 0.5f;

            var sphereParticlesMain = sphereParticles.main;
            sphereParticlesMain.startLifetime =
                new ParticleSystem.MinMaxCurve(0.1f + (m_BallEnlarger.CurrentSize - 0.5f),
                    1.5f + (m_BallEnlarger.CurrentSize - 0.5f));

            var sphereParticlesEmission = sphereParticles.emission;
            sphereParticlesEmission.rateOverTime = new ParticleSystem.MinMaxCurve(15f + m_BallEnlarger.CurrentSize,
                20f + m_BallEnlarger.CurrentSize);
        }

        private Vector3 RelativeDirection =>
            m_MainCamera.transform.right * m_Input.x + m_MainCamera.transform.forward * m_Input.z;

        public float CurrentSize => m_BallEnlarger.CurSize;

        // Update is called once per frame
        void FixedUpdate()
        {
            m_Rigidbody.AddForce((RelativeDirection *
                                  (TrueSpeed * Time.fixedDeltaTime)) + m_MainCamera.transform.right,
                ForceMode.Force);
            // m_Rigidbody.velocity = ClampSpeed(m_Rigidbody.velocity);

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
            return foundObjects.FindAll(c => c != m_Collider).Count != 0;
        }


        // private Vector3 ClampSpeed(Vector3 direction)
        // {
        //     return Vector3.ClampMagnitude(direction, MaxSpeed);
        // }

        private void OnDrawGizmos()
        {
            if (m_Collider == null) return;
            Gizmos.color = Color.magenta;
            Gizmos.DrawCube(BottomPositionOfCollider, GroundCollisionSize);
            Gizmos.color = Color.red - new Color(0, 0, 0, 0.5f);
            Gizmos.DrawSphere(transform.position, m_Collider.radius);
            Gizmos.color = Color.green - new Color(0, 0, 0, 0.5f);
            Gizmos.DrawSphere(transform.position, m_Collider.radius + grabRadius);
        }


        public void ChangeBallSize(float value)
        {
            Debug.Log("Changing Ball's size!");
            m_BallEnlarger.ChangeBallSize(value);
        }

        public void SetBallSize(float size)
        {
            m_BallEnlarger.SetBallSize(size);
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
        private float m_PreviousSize;
        private CinemachineFreeLook m_CinemachineFreeLook;
        private MonoBehaviour m_BehaviourRef;
        private Transform m_SphereModel;

        private MeshRenderer m_SphereModelMeshRenderer;
        private Material m_SphereModelMaterial;

        private float m_TopOrbitHeight, m_MidOrbitRadius;

        private float m_SphereModelDefaultVdSpeed, m_SphereModelDefaultVdSize, m_SphereModelDefaultVdStrength;


        private Coroutine m_DroppingObjects;

        #region StaticDefinitions

        private static readonly int SlimeColor = Shader.PropertyToID("Slime_Color");
        private static readonly int VdSpeed = Shader.PropertyToID("VD_Speed");
        private static readonly int VdSize = Shader.PropertyToID("VD_Size");
        private static readonly int VdStrength = Shader.PropertyToID("VD_Strength");

        #endregion

        public float CurrentSize => m_SphereCollider.radius;

        public BallEnlarger(SphereCollider collider, float radius, LayerMask layerMask, LayerMask hazardMask,
            CinemachineFreeLook cinemachineFreeLook, Transform sphereModel, MonoBehaviour behaviourRef)
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
            m_SphereModelMeshRenderer = m_SphereModel.GetComponent<MeshRenderer>();
            m_SphereModelMaterial = m_SphereModelMeshRenderer.material;

            m_SphereModelDefaultVdSpeed = SphereModelVertexDisplacementSpeed;
            m_SphereModelDefaultVdSize = SphereModelVertexDisplacementSize;
            m_SphereModelDefaultVdStrength = SphereModelVertexDisplacementStrength;

            m_BehaviourRef = behaviourRef;
        }

        private Color PSphereColor
        {
            get => m_SphereModelMaterial.GetColor(SlimeColor);
            set => m_SphereModelMaterial.SetColor(SlimeColor, value);
        }

        private float SphereModelVertexDisplacementSpeed
        {
            get => m_SphereModelMaterial.GetFloat(VdSpeed);
            set => m_SphereModelMaterial.SetFloat(VdSpeed, value);
        }

        private float SphereModelVertexDisplacementSize
        {
            get => m_SphereModelMaterial.GetFloat(VdSize);
            set => m_SphereModelMaterial.SetFloat(VdSize, value);
        }

        private float SphereModelVertexDisplacementStrength
        {
            get => m_SphereModelMaterial.GetFloat(VdStrength);
            set => m_SphereModelMaterial.SetFloat(VdStrength, value);
        }

        public float CurSize => m_SphereCollider.radius;


        public void PickupNearbyObjectsAndEnlarge()
        {
            Collider[] foundObjects = Physics.OverlapSphere(m_SphereCollider.transform.position,
                m_SphereCollider.radius + m_Radius, m_GrabMask);
            Debug.Log($"{foundObjects.Length} were found in {m_Radius} radius.");
            for (int i = 0; i < foundObjects.Length; i++)
            {
                BallAffector affector = foundObjects[i].GetComponent<BallAffector>();
                if (affector != null && affector.information.scaleType == BallAffectorInformation.ScaleType.ScaleUp &&
                    affector.information.minSizeToGrab <= m_SphereCollider.radius && !affector.IsPickedUpByPlayer)
                {
                    GameObject obj = foundObjects[i].gameObject;
                    PickupObject(obj, affector, true);
                    m_CurrentSize += affector.information.scaleRate;
                }
            }

            OnPickupObject(true);

            ChangeCollisionSize(m_CinemachineFreeLook, m_SphereModel);
        }

        private void PickupObject(GameObject obj, BallAffector affector, bool isPickedUp)
        {
            m_CaughtObjects.Add(obj);
            if (obj.GetComponent<Rigidbody>() is { } rigidbody && rigidbody != null)
                rigidbody.isKinematic = true;
            obj.GetComponent<Collider>().enabled = false;
            affector.IsPickedUpByPlayer = isPickedUp;
        }

        public void OnPickupObject(bool useAnimation)
        {
            foreach (GameObject obj in m_CaughtObjects)
            {
                if (obj.transform.parent != m_SphereCollider.transform)
                {
                    if (useAnimation)
                        obj.transform.DOMove(
                            m_SphereCollider.transform.position + (Random.insideUnitSphere *
                                                                   Mathf.Clamp(m_SphereCollider.radius, 2,
                                                                       float.MaxValue)), 0.15f);
                    else
                        obj.transform.position = m_SphereCollider.transform.position + (Random.insideUnitSphere *
                            Mathf.Clamp(m_SphereCollider.radius, 2,
                                float.MaxValue));


                    obj.transform.SetParent(m_SphereCollider.transform);
                    // obj.transform.localScale = Vector3.one * Random.Range(0.25f, 1f);
                }
            }
        }

        public void DecreaseInSizeOnCondition()
        {
            Collider[] foundObjects = Physics.OverlapSphere(m_SphereCollider.transform.position,
                m_SphereCollider.radius + 0.25f, m_HazardMask);

            for (int i = 0; i < foundObjects.Length; i++)
            {
                BallAffector affector = foundObjects[i].GetComponent<BallAffector>();
                if (foundObjects[i] != null && affector != null &&
                    affector.information.scaleType == BallAffectorInformation.ScaleType.ScaleDown)
                {
                    m_CurrentSize -= affector.information.scaleRate;
                    UpdateCaughtObjectsList(m_BehaviourRef);
                }
            }

            ChangeCollisionSize(m_CinemachineFreeLook, m_SphereModel);
        }

        public void UpdateCaughtObjectsList(MonoBehaviour monoBehaviour)
        {
            if (m_DroppingObjects != null)
                monoBehaviour.StopCoroutine(m_DroppingObjects);
            m_DroppingObjects = monoBehaviour.StartCoroutine(DropCaughtObjects());
        }

        private IEnumerator DropCaughtObjects()
        {
            List<GameObject> outofRangeObjects = m_CaughtObjects.FindAll(c =>
                (m_SphereCollider.transform.position - c.transform.position).magnitude > m_SphereCollider.radius);
            int index = 0;
            while (m_CaughtObjects.Count != 0 &&
                   outofRangeObjects.Count != 0)
            {
                GameObject obj = outofRangeObjects[index];

                ResetObject(obj);

                if (obj.gameObject.activeSelf)
                {
                    obj.gameObject.transform
                        .DOMove(obj.gameObject.transform.position, Random.Range(0.2f, 0.5f)).OnComplete(
                            () => { DropObject(obj, true); });
                }

                index++;
                index = Mathf.Clamp(index, 0, outofRangeObjects.Count - 1);

                yield return new WaitForEndOfFrame();
            }
        }

        private void ResetObject(GameObject obj)
        {
            obj.transform.SetParent(null);
            if (m_CaughtObjects.Contains(obj))
                m_CaughtObjects.Remove(obj);
        }

        private void DropObject(GameObject obj, bool enableRigidbody)
        {
            Rigidbody body = default;
            if (obj.GetComponent<Rigidbody>() == null)
            {
                body = obj.AddComponent<Rigidbody>();
            }
            obj.GetComponent<Collider>().enabled = true;
            if (body is { }) body.isKinematic = !enableRigidbody;
        }

        public void ChangeBallSize(float value)
        {
            m_CurrentSize = value;
            ChangeCollisionSize(m_CinemachineFreeLook, m_SphereModel);
            UpdateCaughtObjectsList(m_BehaviourRef);
        }

        private void ChangeCollisionSize(CinemachineFreeLook cinemachineFreeLook, Transform sphereModel)
        {
            var radius = m_SphereCollider.radius;

            radius += m_CurrentSize;
            m_SphereCollider.radius = radius;
            m_SphereCollider.radius = Mathf.Clamp(radius, 1, float.MaxValue);

            cinemachineFreeLook.m_Orbits[0].m_Height += m_CurrentSize * 4f;
            cinemachineFreeLook.m_Orbits[0].m_Height = Mathf.Clamp(cinemachineFreeLook.m_Orbits[0].m_Height,
                m_TopOrbitHeight, float.MaxValue);

            cinemachineFreeLook.m_Orbits[1].m_Radius += m_CurrentSize * 4f;
            cinemachineFreeLook.m_Orbits[1].m_Radius = Mathf.Clamp(cinemachineFreeLook.m_Orbits[1].m_Radius,
                m_MidOrbitRadius, float.MaxValue);


            sphereModel.localScale = Vector3.one * (m_SphereCollider.radius * 2f);
            //sphereModel.localScale = Vector3.Min(Vector3.Max(sphereModel.localScale, Vector3.zero), sphereModel.localScale);
            SphereModelVertexDisplacementSize += m_CurrentSize;
            SphereModelVertexDisplacementSize = Mathf.Clamp(SphereModelVertexDisplacementSize,
                m_SphereModelDefaultVdSize, float.MaxValue);

            m_PreviousSize = m_CurrentSize;
            m_CurrentSize = 0;
        }

        private void SetCollisionSize(float size, CinemachineFreeLook cinemachineFreeLook, Transform sphereModel)
        {
            var radius = m_SphereCollider.radius;

            radius = size;
            m_SphereCollider.radius = radius;
            m_SphereCollider.radius = Mathf.Clamp(radius, 1, float.MaxValue);

            cinemachineFreeLook.m_Orbits[0].m_Height = size * 4f;
            cinemachineFreeLook.m_Orbits[0].m_Height = Mathf.Clamp(cinemachineFreeLook.m_Orbits[0].m_Height,
                m_TopOrbitHeight, float.MaxValue);

            cinemachineFreeLook.m_Orbits[1].m_Radius = size * 4f;
            cinemachineFreeLook.m_Orbits[1].m_Radius = Mathf.Clamp(cinemachineFreeLook.m_Orbits[1].m_Radius,
                m_MidOrbitRadius, float.MaxValue);


            sphereModel.localScale = Vector3.one * (m_SphereCollider.radius * 2f);
            //sphereModel.localScale = Vector3.Min(Vector3.Max(sphereModel.localScale, Vector3.zero), sphereModel.localScale);
            SphereModelVertexDisplacementSize = size;
            SphereModelVertexDisplacementSize = Mathf.Clamp(SphereModelVertexDisplacementSize,
                m_SphereModelDefaultVdSize, float.MaxValue);
        }

        public void SetBallSize(float size)
        {
            SetCollisionSize(size, m_CinemachineFreeLook, m_SphereModel);
            UpdateCaughtObjectsList(m_BehaviourRef);
        }

        public void ForceDropObject(BallAffector affectorValue, Vector3 keyAffectorPosition, bool keyAffectorState,
            Quaternion keyAffectorRotation, Transform keyAffectorParent, bool keyObjectCollisionState)
        {
            DropObject(affectorValue.gameObject, false);
            ResetObject(affectorValue.gameObject);
            affectorValue.IsPickedUpByPlayer = keyAffectorState;
            affectorValue.transform.position = keyAffectorPosition;
            affectorValue.transform.rotation = keyAffectorRotation;
            affectorValue.transform.SetParent(keyAffectorParent);
            affectorValue.GetComponent<Collider>().enabled = keyObjectCollisionState;
        }

        public void ForcePickupObject(BallAffector affectorValue, bool keyObjectState)
        {
            PickupObject(affectorValue.gameObject, affectorValue, keyObjectState);
            OnPickupObject(false);
        }

        public void ForceDropAllObjects()
        {
            for (var index = 0; index < m_CaughtObjects.Count; index++)
            {
                var obj = m_CaughtObjects[index];
                DropObject(obj, false);
                ResetObject(obj.gameObject);
            }
        }
    }
}