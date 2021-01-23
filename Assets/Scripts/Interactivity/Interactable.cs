using UnityEngine;
using UnityEngine.Events;

namespace Interactivity
{
    
    public class Interactable : MonoBehaviour
    {
        public UnityEvent<Collider> onInteractCallback;

        public void OnInteract(Collider col)
        {
            onInteractCallback?.Invoke(col);
        }
    }
}
