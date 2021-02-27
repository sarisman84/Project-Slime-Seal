using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Cinemachine;
using DG.Tweening;
using Extensions;
using Interactivity;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Player
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(SphereCollider))]
    public class SizeController : MonoBehaviour
    {
        public float pickupRadius;
        [Space] public CinemachineFreeLook playerCamera;
        public ParticleSystem sphereParticles;
        public Transform sphereModel;

        #region Private Variables

        private List<GameObject> _mCaughtObjects;
        private SphereCollider _mSphereCollider;

        private float _mRadius;
        private float _mAccumilatedSizeResult;
        private float _mPreviousSize;
        private CinemachineFreeLook _mCinemachineFreeLook;
        private Transform _mSphereModel;

        private MeshRenderer _mSphereModelMeshRenderer;
        private Material _mSphereModelMaterial;

        private float _mTopOrbitHeight, _mMidOrbitRadius;

        private float _mSphereModelDefaultVdSpeed, _mSphereModelDefaultVdSize, _mSphereModelDefaultVdStrength;


        private Coroutine _mDroppingObjects;

        #region StaticDefinitions

        private static readonly int SlimeColor = Shader.PropertyToID("Slime_Color");
        private static readonly int VdSpeed = Shader.PropertyToID("VD_Speed");
        private static readonly int VdSize = Shader.PropertyToID("VD_Size");
        private static readonly int VdStrength = Shader.PropertyToID("VD_Strength");

        #endregion

        #endregion


        private void Awake()
        {
            _mSphereCollider = GetComponent<SphereCollider>();
            _mSphereModel = sphereModel;
            _mCaughtObjects = new List<GameObject>();
            _mSphereModelMeshRenderer = _mSphereModel.GetComponent<MeshRenderer>();
            _mSphereModelMaterial = _mSphereModelMeshRenderer.material;

            _mCinemachineFreeLook = playerCamera;
            _mRadius = pickupRadius;

            _mTopOrbitHeight = _mCinemachineFreeLook.m_Orbits[0].m_Height;
            _mMidOrbitRadius = _mCinemachineFreeLook.m_Orbits[1].m_Radius;

            _mSphereModelDefaultVdSpeed = sphereModelVertexDisplacementSpeed;
            _mSphereModelDefaultVdSize = sphereModelVertexDisplacementSize;
            _mSphereModelDefaultVdStrength = sphereModelVertexDisplacementStrength;

            ballSize = _mSphereCollider.radius;
        }

        #region Public Properties

        #region Vertex Shader Properties

        private Color pSphereColor
        {
            get => _mSphereModelMaterial.GetColor(SlimeColor);
            set => _mSphereModelMaterial.SetColor(SlimeColor, value);
        }

        private float sphereModelVertexDisplacementSpeed
        {
            get => _mSphereModelMaterial.GetFloat(VdSpeed);
            set => _mSphereModelMaterial.SetFloat(VdSpeed, value);
        }

        private float sphereModelVertexDisplacementSize
        {
            get => _mSphereModelMaterial.GetFloat(VdSize);
            set => _mSphereModelMaterial.SetFloat(VdSize, value);
        }

        private float sphereModelVertexDisplacementStrength
        {
            get => _mSphereModelMaterial.GetFloat(VdStrength);
            set => _mSphereModelMaterial.SetFloat(VdStrength, value);
        }

        #endregion

        public float ballSize { get; private set; }
        public float pickupRange => _mRadius + (ballSize / 4f);

        #endregion

        private void Update()
        {
            DetectAndChangeSize();
        }


        public void DetectAndChangeSize()
        {
            Collider[] foundObjects = Physics.OverlapSphere(_mSphereCollider.transform.position,
                    _mSphereCollider.radius + pickupRange).Where(c => c.gameObject.GetComponent<BallAffector>() != null)
                .ToArray();
            for (int i = 0; i < foundObjects.Length; i++)
            {
                BallAffector affector = foundObjects[i].GetComponent<BallAffector>();
                if (affector == null) continue;
                if (affector.information.scaleType == BallAffectorInformation.ScaleType.ScaleUp &&
                    affector.information.minSizeToGrab <= _mSphereCollider.radius && !affector.IsPickedUpByPlayer)
                {
                    PickupObject(affector.gameObject, affector, UpdateParticleEmitter, true);
                    _mAccumilatedSizeResult += affector.information.ScaleRate(_mSphereCollider);
                }
                else if (Vector3.Distance(transform.position, affector.transform.position) <=
                         _mSphereCollider.radius + 0.25f &&
                         affector.information.scaleType == BallAffectorInformation.ScaleType.ScaleDown)
                {
                    _mAccumilatedSizeResult -= affector.information.ScaleRate(_mSphereCollider, true);
                }

                if (_mAccumilatedSizeResult != 0)
                {
                    ApplyObjectVisualsOnListChange(affector.gameObject, true);
                    ChangeCollisionSize(_mCinemachineFreeLook, _mSphereModel);
                }
            }
        }

        private void UpdateParticleEmitter()
        {
            var sphereParticlesShape = sphereParticles.shape;
            sphereParticlesShape.radius = ballSize - 0.5f;

            var sphereParticlesMain = sphereParticles.main;
            sphereParticlesMain.startLifetime =
                new ParticleSystem.MinMaxCurve(0.1f + (ballSize - 0.5f),
                    1.5f + (ballSize - 0.5f));

            var sphereParticlesEmission = sphereParticles.emission;
            sphereParticlesEmission.rateOverTime = new ParticleSystem.MinMaxCurve(
                15f + ballSize,
                20f + ballSize);
        }


        private void PickupObject(GameObject obj, BallAffector affector, Action onPickupDelegate, bool isPickedUp)
        {
            _mCaughtObjects.Add(obj);
            if (obj.GetComponent<Rigidbody>() is { } rigidbody && rigidbody != null)
                rigidbody.isKinematic = true;
            obj.GetComponent<Collider>().enabled = false;
            affector.IsPickedUpByPlayer = isPickedUp;

            onPickupDelegate?.Invoke();
        }

        public void ApplyObjectVisualsOnListChange(GameObject caughtGameObject, bool useAnimation)
        {
            if (caughtGameObject == null)
            {
                foreach (var obj in _mCaughtObjects)
                {
                    if (obj.transform.parent != _mSphereCollider.transform)
                    {
                        if (useAnimation)
                            obj.transform.DOMove(
                                _mSphereCollider.transform.position + (Random.insideUnitSphere *
                                                                       Mathf.Clamp(_mSphereCollider.radius, 2,
                                                                           float.MaxValue)), 0.15f);
                        else
                            obj.transform.position = _mSphereCollider.transform.position +
                                                     (Random.insideUnitSphere *
                                                      Mathf.Clamp(_mSphereCollider.radius, 2,
                                                          float.MaxValue));
                        obj.transform.SetParent(_mSphereCollider.transform);
                    }
                }

                return;
            }

            if (caughtGameObject.transform.parent != _mSphereCollider.transform)
            {
                if (useAnimation)
                    caughtGameObject.transform.DOMove(
                        _mSphereCollider.transform.position + (Random.insideUnitSphere *
                                                               Mathf.Clamp(_mSphereCollider.radius, 2,
                                                                   float.MaxValue)), 0.15f);
                else
                    caughtGameObject.transform.position = _mSphereCollider.transform.position +
                                                          (Random.insideUnitSphere *
                                                           Mathf.Clamp(_mSphereCollider.radius, 2,
                                                               float.MaxValue));

                caughtGameObject.transform.SetParent(_mSphereCollider.transform);
            }
        }

        public void ApplyObjectVisualsOnListChange(bool useAnimation)
        {
            ApplyObjectVisualsOnListChange(null, useAnimation);
        }


        public void UpdateCaughtObjectsList(MonoBehaviour monoBehaviour, Action onDropDelegate)
        {
            if (_mDroppingObjects != null)
                monoBehaviour.StopCoroutine(_mDroppingObjects);
            _mDroppingObjects = monoBehaviour.StartCoroutine(DropCaughtObjects(onDropDelegate));
        }

        private IEnumerator DropCaughtObjects(Action onDropDelegate)
        {
            List<GameObject> outofRangeObjects = _mCaughtObjects.FindAll(c =>
                (_mSphereCollider.transform.position - c.transform.position).magnitude > _mSphereCollider.radius);
            int index = 0;
            while (_mCaughtObjects.Count != 0 &&
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
                onDropDelegate?.Invoke();
                yield return new WaitForEndOfFrame();
            }
        }

        private void ResetObject(GameObject obj)
        {
            obj.transform.SetParent(null);
            if (_mCaughtObjects.Contains(obj))
                _mCaughtObjects.Remove(obj);
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
            _mAccumilatedSizeResult = value;
            ChangeCollisionSize(_mCinemachineFreeLook, _mSphereModel);
            UpdateCaughtObjectsList(this, UpdateParticleEmitter);
        }

        private void ChangeCollisionSize(CinemachineFreeLook cinemachineFreeLook, Transform sphereModel)
        {
            ballSize += _mAccumilatedSizeResult;
            ballSize = Mathf.Clamp(ballSize, 1, float.MaxValue);

            Debug.Log($"Size Changed to: {ballSize}");

            SetCollisionSize(ballSize, cinemachineFreeLook, sphereModel);

            _mPreviousSize = _mAccumilatedSizeResult;
            _mAccumilatedSizeResult = 0;
        }

        private void SetCollisionSize(float size, CinemachineFreeLook cinemachineFreeLook, Transform sphereModel)
        {
            _mSphereCollider.radius = size.ClampedTo(1);

            cinemachineFreeLook.m_Orbits[0].m_Height =
                (size * 4f).ClampedTo(_mTopOrbitHeight);
            cinemachineFreeLook.m_Orbits[1].m_Radius =
                (size * 4f).ClampedTo(_mMidOrbitRadius);

            sphereModel.localScale = Vector3.one * (_mSphereCollider.radius * 2f);
            sphereModelVertexDisplacementSize = size.ClampedTo(_mSphereModelDefaultVdSize);
        }

        public void SetBallSize(float size)
        {
            ballSize = size.ClampedTo(1);
            SetCollisionSize(ballSize, _mCinemachineFreeLook, _mSphereModel);
            UpdateCaughtObjectsList(this, UpdateParticleEmitter);
        }

        public void ForceDropObject(BallAffector affectorValue, Vector3 keyAffectorPosition, bool keyAffectorState,
            Quaternion keyAffectorRotation, Transform keyAffectorParent, bool keyObjectCollisionState)
        {
            DropObject(affectorValue.gameObject, false);
            ResetObject(affectorValue.gameObject);
            affectorValue.IsPickedUpByPlayer = keyAffectorState;
            var transform = affectorValue.transform;
            transform.position = keyAffectorPosition;
            transform.rotation = keyAffectorRotation;
            affectorValue.transform.SetParent(keyAffectorParent);
            affectorValue.GetComponent<Collider>().enabled = keyObjectCollisionState;
        }

        public void ForcePickupObject(BallAffector affectorValue, bool keyObjectState)
        {
            PickupObject(affectorValue.gameObject, affectorValue, UpdateParticleEmitter, keyObjectState);
            ApplyObjectVisualsOnListChange(false);
        }

        public void ForceDropAllObjects()
        {
            for (var index = 0; index < _mCaughtObjects.Count; index++)
            {
                var obj = _mCaughtObjects[index];
                DropObject(obj, false);
                ResetObject(obj.gameObject);
            }
        }
        
        
    }
}