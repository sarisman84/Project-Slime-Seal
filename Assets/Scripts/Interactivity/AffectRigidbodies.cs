using UnityEngine;

namespace Interactivity
{
    public class AffectRigidbodies : MonoBehaviour
    {
        public Vector3 force;
        
        public void PushRigidbody(Collider other)
        {
            if (other.attachedRigidbody != null)
            {
                other.attachedRigidbody.AddForce((force - transform.position).normalized * force.magnitude);
            }
        }
    }
}
