using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;

namespace Game_Managers.UI
{
    [RequireComponent(typeof(PlayableDirector))]
    public class PauseMenuManager : MonoBehaviour
    {
        public Canvas menuUI;
        public InputActionReference pauseMenuToggle;
        private PlayableDirector m_PlayableDirector;


        private bool m_ToggleMenu;
        private float m_OriginalTimeScale;

        private void Awake()
        {
            m_PlayableDirector = GetComponent<PlayableDirector>();
            m_OriginalTimeScale = Time.timeScale;
        }

        private void Update()
        {
            if (pauseMenuToggle.action.ReadValue<float>() > 0 && pauseMenuToggle.action.triggered)
            {
                m_ToggleMenu = !m_ToggleMenu;
            }
            
            menuUI.gameObject.SetActive(m_ToggleMenu);
            Time.timeScale = m_ToggleMenu ? 0 : m_OriginalTimeScale;
        }

        public void ToMainMenu()
        {
            m_PlayableDirector.Play();
        }

        public void Quit()
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
         Application.Quit();
#endif
        }
    }
}