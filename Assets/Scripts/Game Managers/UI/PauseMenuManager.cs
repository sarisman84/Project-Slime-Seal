using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;
using UnityEngine.UI;

namespace Game_Managers.UI
{
    [RequireComponent(typeof(PlayableDirector))]
    public class PauseMenuManager : MonoBehaviour
    {
        public CanvasGroup menuUI;
        public InputActionReference pauseMenuToggle;


        internal bool m_ToggleMenu;
        internal float OriginalTimeScale;
        private GraphicRaycaster m_GraphicRaycaster;

        private void OnEnable()
        {
            pauseMenuToggle.action.Enable();
        }

        private void OnDisable()
        {
            pauseMenuToggle.action.Disable();
        }

        private void Awake()
        {
            m_GraphicRaycaster = menuUI.GetComponent<GraphicRaycaster>();
            OriginalTimeScale = Time.timeScale;
        }

        private void Update()
        {
            if (pauseMenuToggle.action.ReadValue<float>() > 0 && pauseMenuToggle.action.triggered)
            {
                m_ToggleMenu = !m_ToggleMenu;
            }

            menuUI.alpha = m_ToggleMenu ? Mathf.Lerp(menuUI.alpha, 1, 0.5f) : Mathf.Lerp(menuUI.alpha, 0, 0.5f);
            m_GraphicRaycaster.enabled = m_ToggleMenu;
            Time.timeScale = m_ToggleMenu ? 0 : OriginalTimeScale;
        }

        public void CloseMenu()
        {
            m_ToggleMenu = false;
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