using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;
using UnityEngine.UI;
using DG.Tweening;
using Player;
using Input = Player.Input;

namespace Game_Managers.UI
{
    [RequireComponent(typeof(PlayableDirector))]
    public class PauseMenuManager : MonoBehaviour
    {
        public CanvasGroup menuUI;
        public InputActionReference pauseMenuToggle;
        BallController m_Player;


        internal bool ToggleMenu;
        internal float OriginalTimeScale;
        private GraphicRaycaster m_GraphicRaycaster;
        public float timeScaleDuringPause = 0.01f;

        private Coroutine m_TimeTrans;
        public float timeTransitionRate;
        private CameraController m_CameraController;
        private Input m_Input;

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
            m_Player = FindObjectOfType<BallController>();
            m_Input = m_Player.GetComponent<Input>();
            m_CameraController = m_Player.GetComponent<CameraController>();
            m_GraphicRaycaster = menuUI.GetComponent<GraphicRaycaster>();
            OriginalTimeScale = Time.timeScale;

       
        }

        private void Update()
        {
            if (pauseMenuToggle.action.ReadValue<float>() > 0 && pauseMenuToggle.action.triggered)
            {
                ToggleMenu = !ToggleMenu;
                Transition();
                m_GraphicRaycaster.enabled = ToggleMenu;
                m_Input.enabled = !ToggleMenu;
                m_CameraController.SetCursorState(ToggleMenu);
            }
        }

        private void Transition()
        {
            if (ToggleMenu)
                menuUI.DOFade(1, 0.5f).OnStart(() =>
                {
                    if (m_TimeTrans != null)
                        StopCoroutine(m_TimeTrans);
                    m_TimeTrans = StartCoroutine(FadeTime(Fade.In));
                });
            else
                menuUI.DOFade(0, 0.5f).OnStart(() =>
                {
                    if (m_TimeTrans != null)
                        StopCoroutine(m_TimeTrans);
                    m_TimeTrans = StartCoroutine(FadeTime(Fade.Out));
                });
        }

        private IEnumerator FadeTime(Fade fade)
        {
            timeTransitionRate = 0.05f;
            switch (fade)
            {
                case Fade.In:
                    while (Time.timeScale > timeScaleDuringPause)
                    {
                        Debug.Log($"Slowing time down: {Time.timeScale}");
                        Time.timeScale = Mathf.Lerp(Time.timeScale, timeScaleDuringPause, timeTransitionRate);
                        Time.fixedDeltaTime = Time.timeScale * 0.02f;
                        yield return new WaitForEndOfFrame();
                    }

                    m_CameraController.SetCursorState(true);
                    m_Input.enabled = false;
                    break;

                case Fade.Out:
                    while (Time.timeScale < OriginalTimeScale - 0.1)
                    {
                        Debug.Log($"Accelerating time: {Time.timeScale}");
                        Time.timeScale = Mathf.Lerp(Time.timeScale, OriginalTimeScale, timeTransitionRate);
                        Time.fixedDeltaTime = Time.timeScale * 0.02f;
                        yield return new WaitForEndOfFrame();
                    }

                    m_CameraController.SetCursorState(false);
                    m_Input.enabled = true;
                    break;
            }

            yield return null;
        }

        public void CloseMenu()
        {
            ToggleMenu = false;
            Transition();
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

    internal enum Fade
    {
        In,
        Out
    }
}