using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;
using UnityEngine.UI;
using DG.Tweening;

namespace Game_Managers.UI
{
    [RequireComponent(typeof(PlayableDirector))]
    public class PauseMenuManager : MonoBehaviour
    {
        public CanvasGroup menuUI;
        public InputActionReference pauseMenuToggle;


        internal bool ToggleMenu;
        internal float OriginalTimeScale;
        private GraphicRaycaster m_GraphicRaycaster;
        public float timeScaleDuringPause = 0.01f;

        private Coroutine m_TimeTrans;

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
                ToggleMenu = !ToggleMenu;
            }

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
            m_GraphicRaycaster.enabled = ToggleMenu;
        }

        private IEnumerator FadeTime(Fade fade)
        {
            switch (fade)
            {
                case Fade.In:
                    while (Time.timeScale != timeScaleDuringPause)
                    {
                        Time.timeScale = Mathf.Lerp(Time.timeScale, timeScaleDuringPause, 0.05f);
                        yield return new WaitForEndOfFrame();
                    }

                    break;

                case Fade.Out:
                    while (Time.timeScale != OriginalTimeScale)
                    {
                        Time.timeScale = Mathf.Lerp(Time.timeScale, OriginalTimeScale, 0.05f);
                        yield return new WaitForEndOfFrame();
                    }

                    break;
            }

            yield return null;
        }

        public void CloseMenu()
        {
            ToggleMenu = false;
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