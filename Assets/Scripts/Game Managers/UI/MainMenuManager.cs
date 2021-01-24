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

        private Transform m_Parent;


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
            m_Parent = m_BallController.transform.parent.parent;
            m_BallController.transform.parent.SetParent(null);
            // m_BallController.transform.position = spawnPos.position;
            m_BallController.enabled = true;
            // GameManager.SingletonAccess.ResetToCheckpoint();
        }

        public void ToMainMenu(PauseMenuManager manager)
        {
            GameManager.SingletonAccess.ResetData();
            Time.timeScale = manager.OriginalTimeScale;
            m_BallController.transform.parent.SetParent(m_Parent);
            m_BallController.transform.parent.localPosition = Vector3.zero;
            m_BallController.transform.localPosition = Vector3.zero;
            m_BallController.enabled = false;
            m_Rigidbody.isKinematic = true;
            m_PlayerInput.enabled = false;
            manager.m_ToggleMenu = false;
            m_MenuDirector.Pause();
            m_MenuDirector.time = 0;
        }

        private IEnumerator OnStartGame()
        {
            GameManager.SingletonAccess.SetCheckpoint(spawnPos.gameObject);
            m_MenuDirector.Play();
            yield return new WaitForSeconds((float) m_MenuDirector.duration);
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