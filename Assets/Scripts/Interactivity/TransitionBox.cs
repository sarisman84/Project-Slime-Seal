using Player;
using UnityEngine;

namespace Interactivity
{
    public class TransitionBox : MonoBehaviour
    {
        public Transform teleportPos;
        public float minSizeToTeleport = 5f;

        public void TeleportToNewArea(BallController col)
        {
            if (col.CurrentSize >= minSizeToTeleport)
                col.transform.position = teleportPos.position;
        }
    }
}