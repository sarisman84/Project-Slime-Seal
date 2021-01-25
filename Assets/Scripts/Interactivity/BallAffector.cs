using System;
using System.Security.Cryptography;
using UnityEngine;
using UnityEngine.UIElements;

namespace Interactivity
{
    [RequireComponent(typeof(MeshCollider))]
    [DisallowMultipleComponent]
    public class BallAffector : MonoBehaviour
    {
        public BallAffectorInformation information;
        public bool IsPickedUpByPlayer { get; set; } = false;
        public bool debugPrintThisObjectsState;


        #region Editor Stuff

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (GetComponent<Collider>() == null && GetComponent<MeshFilter>() != null &&
                GetComponent<MeshRenderer>() == null)
                gameObject.AddComponent<MeshCollider>();
            else if ((GetComponent<Collider>() == null || GetComponent<MeshCollider>() != null) &&
                     Application.isPlaying)
            {
                gameObject.AddComponent<BoxCollider>();
                Destroy(GetComponent<MeshCollider>());
            }

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
#endif

        #endregion


        private void Update()
        {
            if(information == null)
            {
                var o = gameObject;
                Debug.LogError($"Information variable on {o.name} is missing", o);
            }

            if (debugPrintThisObjectsState)
                Debug.Log(
                    $"{gameObject.name} {(IsPickedUpByPlayer ? "is currently picked up (or has been picked up) by the player" : "hasn't been picked up yet")}");
        }
    }
}