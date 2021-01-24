using Game_Managers;
using UnityEngine;

namespace Interactivity
{
    public class Checkpoint : MonoBehaviour
    {
        public Transform checkpointPos;
        public void SetCheckpoint(Collider col)
        {
            GameManager.SingletonAccess.SetCheckpoint(checkpointPos.gameObject);
        }
    }
}
