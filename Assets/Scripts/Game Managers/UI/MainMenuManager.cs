using UnityEngine;

namespace Game_Managers.UI
{
    public class MainMenuManager : MonoBehaviour
    {
        public void StartGame()
        {
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