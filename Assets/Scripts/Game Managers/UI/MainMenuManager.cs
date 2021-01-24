using System;
using System.Collections;
using Cinemachine;
using Interactivity;
using NUnit.Framework.Constraints;
using Player;
using UnityEngine;
using UnityEngine.Playables;
using Input = Player.Input;

namespace Game_Managers.UI
{
    public class MainMenuManager : MonoBehaviour
    {
        public Transform spawnPos;
        private BallController m_BallController;
        private PlayableDirector m_MenuDirector;
        private Input m_PlayerInput;
        private Rigidbody m_Rigidbody;
        private CameraController m_CameraController;


        private void Start()
        {
            m_BallController = FindObjectOfType<BallController>();
            
            m_CameraController = m_BallController.GetComponent<CameraController>();
            m_Rigidbody = m_BallController.GetComponent<Rigidbody>();
            m_MenuDirector = GetComponent<PlayableDirector>();
            m_PlayerInput = m_BallController.GetComponent<Input>();
            
            m_BallController.enabled = false;
            m_PlayerInput.enabled = false;
            
            m_CameraController.SetCursorState(true);

            Cursor.visible = true;
            m_Rigidbody.isKinematic = true;
        }

        public void StartGame()
        {
            StartCoroutine(OnStartGame());
        }

        public void ResetPlayer()
        {
           
            m_BallController.transform.parent.SetParent(null);
            m_BallController.enabled = true;
            GameManager.SingletonAccess.ResetToCheckpoint();
        }

        private IEnumerator OnStartGame()
        {
         
            GameManager.SingletonAccess.ResetData();
            GameManager.SingletonAccess.SetCheckpoint(spawnPos.gameObject);
            m_MenuDirector.Play();
            yield return new WaitForSeconds((float)m_MenuDirector.duration);
            GameManager.SingletonAccess.ResetToCheckpoint();
            m_PlayerInput.enabled = true;
            m_Rigidbody.isKinematic = false;
            
            m_CameraController.SetCursorState(false);
          
        }

        public void QuitGame()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
        }
    }
}