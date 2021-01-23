using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Player
{
    public class AnimationController : MonoBehaviour
    {
        [Header("Tentacle Grab Variables")] public List<GameObject> tentaclePrefabs;
        private GameObject m_StartingPos;
        private GameObject m_ArmPos;
        private GameObject m_EndingPos;

        private void Awake()
        {
            SetupModels();
        }

        public IEnumerator PlayGrabAnimation(Vector3 startPos, Vector3 targetGrabPosition)
        {
            m_StartingPos.SetActive(true);

            m_StartingPos.transform.localScale = Vector3.one * 0.01f;
            m_StartingPos.transform.position = startPos;

            yield return m_StartingPos.transform.DOScale(Vector3.one, 0.5f).OnComplete(() =>
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
                    new Vector3(1, 1, DirectionVector(startPos, targetGrabPosition).magnitude), 0.5f);
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
            yield return m_ArmPos.transform.DOScale(Vector3.one *0.01f, 0.2f).OnStart(() =>
            {
                m_StartingPos.transform.DOScale(Vector3.one *0.01f, 0.3f).OnComplete(() =>
                {
                    m_StartingPos.SetActive(false);
                });
              
                m_EndingPos.transform.DOScale(Vector3.one *0.01f, 0.4f).OnComplete(() =>
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
        }
    }
}