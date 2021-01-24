using UnityEngine;

namespace Interactivity
{
    public class TransitionBox : MonoBehaviour
    {
        public Transform teleportPos;
        public float minSizeToTeleport = 5f;

        public void TeleportToNewArea(Collider col)
        {
            col.transform.position = teleportPos.position;
        }
    }
}