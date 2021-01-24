using Game_Managers;
using UnityEngine;

namespace Interactivity
{
    public class DeathBox : MonoBehaviour
    {
        public void KillPlayer()
        {
            GameManager.SingletonAccess.ResetToCheckpoint();
        }
    }
}
