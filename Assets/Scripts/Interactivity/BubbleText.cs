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

         

            public void CreateBubbleElement(Transform owner)
            {
                m_CurrentIns = Instantiate(uiPrefab, owner);
                m_CurrentIns.SetActive(false);
            }


            public IEnumerator DisplayUIForSecondsAmount()
            {
                m_CurrentIns.SetActive(true);
                yield return new WaitForSeconds(displayDuration);
                m_CurrentIns.SetActive(false);
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