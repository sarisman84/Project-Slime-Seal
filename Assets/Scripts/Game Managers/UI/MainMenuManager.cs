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
        public Transform mainMenuSpawnPosition;
        public Transform gameplaySpawnPosition;

        public CinemachineClearShot menuCamera;
        private Vector3 m_DefaultCameraPosition;
        
        private BallController m_BallController;
        private PlayableDirector m_MenuDirector;
        private Input m_PlayerInput;
        private Rigidbody m_Rigidbody;
        private CameraController m_CameraController;


        private void Start()
        {
            if (mainMenuSpawnPosition == null)
                throw new NullReferenceException(
                    "No Initial Position for Player has been set (check the mainMenuSpawnPosition Variable)");

            if (gameplaySpawnPosition == null)
                throw new NullReferenceException(
                    "No Spawning location has been set for the Player (check the gameplaySpawnPosition");
            
            
            m_BallController = FindObjectOfType<BallController>();
            m_CameraController = m_BallController.GetComponent<CameraController>();
            m_Rigidbody = m_BallController.GetComponent<Rigidbody>();
            m_MenuDirector = GetComponent<PlayableDirector>();
            m_PlayerInput = m_BallController.GetComponent<Input>();
            m_DefaultCameraPosition = menuCamera.transform.position;
            SetupPlayer();


       



        }

        public void StartGame()
        {
            StartCoroutine(OnStartGame());
        }

        public void ResetPlayer()
        {
            // var parent = m_BallController.transform.parent;
            // parent.SetParent(null);
            // Debug.Break();
            // m_BallController.transform.position = gameplaySpawnPosition.position;
            m_BallController.enabled = true;
            m_PlayerInput.enabled = true;
            m_Rigidbody.isKinematic = false;
            m_CameraController.SetCursorState(false);
            // GameManager.SingletonAccess.ResetToCheckpoint();
        }

        public void SetupPlayer()
        {
            menuCamera.transform.position = m_DefaultCameraPosition;
            //
            //
            var parent = m_BallController.transform.parent;
            // parent.SetParent(mainMenuSpawnPosition);
            // parent.localPosition = Vector3.zero;
            parent.position = mainMenuSpawnPosition.position;
            
            m_Rigidbody.isKinematic = true;
            m_BallController.enabled = false;
            m_PlayerInput.enabled = false;
            m_CameraController.SetCursorState(true);
        }

        public void ToMainMenu(PauseMenuManager manager)
        {
            Debug.Log("Opening Main Menu!");
            GameManager.SingletonAccess.ResetData();
            Time.timeScale = manager.OriginalTimeScale;

            SetupPlayer();
            
            manager.ToggleMenu = false;
            m_MenuDirector.Pause();
            m_MenuDirector.time = 0;
        }

        private IEnumerator OnStartGame()
        {
            GameManager.SingletonAccess.SetCheckpoint(gameplaySpawnPosition.gameObject);
            m_MenuDirector.Play();
            yield return new WaitForSeconds((float) m_MenuDirector.duration);
            GameManager.SingletonAccess.ResetToCheckpoint();
            ResetPlayer();
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