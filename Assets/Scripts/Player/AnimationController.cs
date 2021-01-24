using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Player
{
    [RequireComponent(typeof(Animator))]
    public class AnimationController : MonoBehaviour
    {
        [Header("Tentacle Grab Variables")] public List<GameObject> tentaclePrefabs;
        public float grabDuration = 1f;
        
       
       
        private GameObject m_StartingPos;
        private GameObject m_ArmPos;
        private GameObject m_EndingPos;
        private Collider m_Collider;

        private Vector3 m_CurrentPos;

        private Animator m_Animator;
        private Vector3 m_ShoulderSize, m_HandSize, m_ArmSize;

        private void Awake()
        {
            m_Collider = GetComponent<Collider>();
            m_Animator = GetComponent<Animator>();
            SetupModels();
        }

        public void SetAnimatorBooleanStateToTrue(string state)
        {
            m_Animator.SetBool(state, true);
        }
        public void SetAnimatorBooleanStateToFalse(string state)
        {
            m_Animator.SetBool(state, false);
        }

        public void TriggerAnimatorTrigger(string state)
        {
            m_Animator.SetTrigger(state);
        }
        
        
        private void Update()
        {
            m_CurrentPos = transform.position;
        }

        public void GrabOntoObject(GameObject targetPos)
        {
            StartCoroutine(Grab(targetPos.transform.position));
        }

        private IEnumerator Grab(Vector3 waypointB)
        {
            Vector3 startPos = Vector3.zero;
            float dot = -1;
            int attempts = 0;
            while (Mathf.Sign(dot) == -1 && attempts <= 500)
            {
                startPos = m_CurrentPos + Random.insideUnitSphere;
                dot = Vector3.Dot((
                        m_CurrentPos - waypointB).normalized,
                    startPos * m_Collider.bounds.size.x);
                attempts++;
            }

            yield return PlayGrabAnimation(startPos, waypointB);
            yield return new WaitForSeconds(grabDuration);
            yield return DisableGrabModels();
        }

        public IEnumerator PlayGrabAnimation(Vector3 startPos, Vector3 targetGrabPosition)
        {
            m_StartingPos.SetActive(true);

            m_StartingPos.transform.localScale = Vector3.one * 0.01f;
            m_StartingPos.transform.position = startPos;

            yield return m_StartingPos.transform.DOScale(m_ShoulderSize, 0.5f).OnComplete(() =>
            {
                m_EndingPos.SetActive(true);
                m_EndingPos.transform.position = startPos;
            }).WaitForCompletion();
            yield return m_EndingPos.transform.DOMove(targetGrabPosition, 0.5f).OnStart(() =>
            {
                m_ArmPos.SetActive(true);
                m_ArmPos.transform.rotation =
                    Quaternion.LookRotation(DirectionVector(startPos, targetGrabPosition).normalized, Vector3.up);
                m_ArmPos.transform.DOScale(
                    new Vector3(m_ArmSize.x, m_ArmSize.y, m_ArmSize.z + DirectionVector(startPos, targetGrabPosition).magnitude), 0.5f);
                m_ArmPos.transform.position = startPos;
                m_ArmPos.transform.DOMove(targetGrabPosition - (DirectionVector(startPos, targetGrabPosition) / 2f),
                    0.5f);
            }).WaitForCompletion();
        }

        private Vector3 DirectionVector(Vector3 startPos, Vector3 targetGrabPosition)
        {
            return (targetGrabPosition - startPos);
        }

        public IEnumerator DisableGrabModels()
        {
            yield return m_ArmPos.transform.DOScale(Vector3.one * 0.01f, 0.2f).OnStart(() =>
            {
                m_StartingPos.transform.DOScale(Vector3.one * 0.01f, 0.3f).OnComplete(() =>
                {
                    m_StartingPos.SetActive(false);
                });

                m_EndingPos.transform.DOScale(Vector3.one * 0.01f, 0.4f).OnComplete(() =>
                {
                    m_EndingPos.SetActive(false);
                });

                m_ArmPos.SetActive(false);
            }).WaitForCompletion();
        }

        private void SetupModels()
        {
            m_StartingPos = m_StartingPos ? m_StartingPos : Instantiate(tentaclePrefabs[0]);
            m_ArmPos = m_ArmPos ? m_ArmPos : Instantiate(tentaclePrefabs[1]);
            m_EndingPos = m_EndingPos
                ? m_EndingPos
                : Instantiate(tentaclePrefabs[tentaclePrefabs.Count - 1]);

            m_StartingPos.SetActive(false);
            m_ArmPos.SetActive(false);
            m_EndingPos.SetActive(false);

            m_ShoulderSize = m_StartingPos.transform.localScale;
            m_ArmSize = m_ArmPos.transform.localScale;
            m_HandSize = m_EndingPos.transform.localScale;
        }
    }
}