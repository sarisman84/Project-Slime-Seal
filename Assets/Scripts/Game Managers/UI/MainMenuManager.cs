using System.Collections;
using Cinemachine;
using UnityEngine;
using UnityEngine.Playables;
using Input = Player.Input;

namespace Game_Managers.UI
{
    public class MainMenuManager : MonoBehaviour
    {
        public PlayableDirector menuDirector;
        public Input playerInput;

        
        public void StartGame()
        {
            StartCoroutine(OnStartGame());
        }

        private IEnumerator OnStartGame()
        {
            GameManager.SingletonAccess.ResetData();
            menuDirector.Play();
            yield return new WaitForSeconds((float)menuDirector.duration);
            playerInput.enabled = true;
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