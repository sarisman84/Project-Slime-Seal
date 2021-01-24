using System;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UIElements;

namespace Interactivity
{
    public class BallAffector : MonoBehaviour
    {
        public BallAffectorInformation information;
        public bool IsPickedUpByPlayer { get; set; } = false;
        public bool debugPrintThisObjectsState;
        
        
        
        private void OnValidate()
        {
            if (GetComponent<Collider>() == null && GetComponent<MeshFilter>() != null && GetComponent<MeshRenderer>() != null)
                gameObject.AddComponent<MeshCollider>();
            else if (GetComponent<Collider>() == null || GetComponent<MeshCollider>() != null)
                gameObject.AddComponent<BoxCollider>();
            if (gameObject.layer != 6)
                gameObject.layer = 6;

            if (ContainsMoreThanOneCollider() && Application.isPlaying)
                RemoveAllButOneCollider();
        }

        private void RemoveAllButOneCollider()
        {
            for (int i = 0; i < GetComponents<Collider>().Length - 1; i++)
            {
                Destroy(GetComponent<Collider>());

            }
        }

        private bool ContainsMoreThanOneCollider()
        {
            return GetComponents<Collider>().Length != 1;
        }

        private void Update()
        {
            if (debugPrintThisObjectsState)
                Debug.Log(
                    $"{gameObject.name} {(IsPickedUpByPlayer ? "is currently picked up (or has been picked up) by the player" : "hasn't been picked up yet")}");
        }
    }
}