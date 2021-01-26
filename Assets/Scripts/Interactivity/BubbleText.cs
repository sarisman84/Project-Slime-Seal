using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace Interactivity
{
    public class BubbleText : MonoBehaviour
    {
        [Serializable]
        public class BubbleUI
        {
            public string name;
            public GameObject uiPrefab;
            public bool hasDuration;
            public float displayDuration = 3f;
            public float displayDelayDuration = 0f;

            public bool hasTransition;
            public float transitionTime;

            public bool useCustomEvents;
            public UnityEvent onEnableUIElement;
            public UnityEvent onDisableUIElement;
            
            private GameObject m_CurrentIns;
            private MonoBehaviour m_MonoBehaviourRef;
            private Coroutine m_DisplayingTextOnDuration;
            private CanvasGroup m_CanvasGroup;

            public bool IsThisActive { get; private set; }

          
            public void CreateBubbleElement(MonoBehaviour owner)
            {
                m_CurrentIns = Instantiate(uiPrefab, owner.transform);
                m_CanvasGroup = m_CurrentIns.GetComponentInChildren<CanvasGroup>();
                m_CanvasGroup.alpha = 0;
                m_CurrentIns.SetActive(false);
                m_MonoBehaviourRef = owner;
            }


            private IEnumerator DisplayUIForSecondsAmount()
            {
                float duration = hasTransition ? transitionTime + displayDuration : displayDuration;
                yield return new WaitForSeconds(displayDelayDuration);
                SetUIActive(true);
                yield return new WaitForSeconds(duration);
                SetUIActive(false);
            }

            private void SetUIActive(bool b)
            {
              
                if (hasTransition)
                {
                    if (m_CanvasGroup == null)
                        throw new NullReferenceException(
                            $"Couldn't find a Canvas Group in {m_CurrentIns.name} and it's children");
                    if (b)
                        m_CanvasGroup.DOFade(1, transitionTime).OnComplete(() =>
                        {
                           
                            IsThisActive = true;
                        }).OnStart(() =>
                        {
                            m_CurrentIns.SetActive(true);
                            if (useCustomEvents)
                                onEnableUIElement?.Invoke();
                        });
                    else
                        m_CanvasGroup.DOFade(0, transitionTime).OnStart(() =>
                        {
                         
                            IsThisActive = false;
                        }).OnComplete(() =>
                        {
                            m_CurrentIns.SetActive(false);
                            if(useCustomEvents)
                                onDisableUIElement?.Invoke();
                        });
                    return;
                }
                m_CurrentIns.SetActive(b);
                m_CanvasGroup.alpha = b ? 1 : 0;


            }

            public void DisplayUIText()
            {
                if (hasDuration)
                {
                    if (m_DisplayingTextOnDuration != null)
                        m_MonoBehaviourRef.StopCoroutine(m_DisplayingTextOnDuration);
                    m_DisplayingTextOnDuration = m_MonoBehaviourRef.StartCoroutine(DisplayUIForSecondsAmount());
                    return;
                }


                SetUIActive(true);
            }

            public void ResetUI()
            {
                SetUIActive(false);
            }
        }


        public List<BubbleUI> bubbleElements = new List<BubbleUI>();


        private void Awake()
        {
            foreach (var bubbleElement in bubbleElements)
            {
                bubbleElement.CreateBubbleElement(this);
            }
        }

        public void ShowUI(string uiToDisplay)
        {
            bubbleElements.Find(b => b.name.Contains(uiToDisplay))?.DisplayUIText();
        }

        public void CloseCurrentActiveUI()
        {
            bubbleElements.Find(b => b.IsThisActive)?.ResetUI();
        }
    }
}