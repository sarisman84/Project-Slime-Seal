using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
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
            public float displayDuration = 3f;

            private GameObject m_CurrentIns;

            public TimelineAsset transitionIn;
            public TimelineAsset transitionOut;

            private PlayableDirector m_Director;

            public void CreateBubbleElement(Transform owner)
            {
                m_CurrentIns = Instantiate(uiPrefab, owner);
                m_Director = m_CurrentIns.GetComponent<PlayableDirector>() ??
                             m_CurrentIns.AddComponent<PlayableDirector>();
            }


            public IEnumerator DisplayUIForSecondsAmount()
            {
                m_Director.Play(transitionIn);
                yield return new WaitForSeconds((float) (transitionIn.duration + displayDuration));
                m_Director.Play(transitionOut);
            }

            public void DisplayUIText(string text)
            {
                if (!m_CurrentIns.activeSelf)
                    m_CurrentIns.SetActive(true);
                m_CurrentIns.GetComponentInChildren<TextMeshProUGUI>().text = text;
            }

            public void ResetUI()
            {
                // m_CurrentIns.
            }
        }


        public List<BubbleUI> bubbleElements = new List<BubbleUI>();


        private void Awake()
        {
            foreach (var bubbleElement in bubbleElements)
            {
                bubbleElement.CreateBubbleElement(transform);
            }
        }

        public void TemporaryShowUI(string uiToDisplay)
        {
            StartCoroutine(bubbleElements.Find(b => b.name.Contains(uiToDisplay))?.DisplayUIForSecondsAmount());
        }

        public void ShowUI(string uiToDisplay)
        {
        }
    }
}