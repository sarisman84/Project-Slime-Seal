using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Interactivity;
using Player;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace UI
{
    public class BallSizeDisplay : MonoBehaviour
    {
        public BallController playerRef;
        private DisplayGraphic m_CurrentElement;

        public Image ballSizeHolder;
        public List<DisplayGraphic> sizeGraphics = new List<DisplayGraphic>();
      
        private void Awake()
        {
            if (playerRef == null)
                throw new NullReferenceException("Player is not assigned to BallSizeDisplay");


            if (ballSizeHolder == null)
                throw new NullReferenceException("Image holder for display is missing!");
            
            DisableDisplay();
        }

        private void Update()
        {
            DisplayGraphic element = sizeGraphics.Find(g =>
                g.minSizeToDisplay < playerRef.CurrentSize && g.maxSizeToDisplay > playerRef.CurrentSize);

            if (!element.spriteElement.Equals(m_CurrentElement.spriteElement))
            {
                m_CurrentElement = element;
                m_CurrentElement.DisplayUI(ballSizeHolder);
            }
                
        }

        public void EnableDisplay()
        {
            ballSizeHolder.gameObject.SetActive(true);
        }

        public void DisableDisplay()
        {
            ballSizeHolder.gameObject.SetActive(false);
        }
    } 

    [Serializable]
    public struct DisplayGraphic
    {
        public float minSizeToDisplay, maxSizeToDisplay;
        public  Sprite spriteElement;
        
        public void DisplayUI(Image imageElement)
        {
            imageElement.sprite = spriteElement;
        }

       
    }
}