using System;
using System.Collections;
using DG.Tweening;
using Player;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace Interactivity
{
    public class BridgeController : MonoBehaviour
    {
        public float minSizeRequirement = 2.5f;
        public float bridgeWidthSize = 3f;
        public float bridgeSpawnSpeed = 2f;
        public GameObject bridgeModelPrefab;
        [SerializeField] private Ease bridgeEaseType;
        public Vector3 waypointA, waypointB;
        public UnityEvent onBridgeBuilding;

        public bool useCustomAnimation;
        public UnityEvent onBuildBridgeAnimation;
        public UnityEvent onResetBridgeAnimation;

        public bool showDebugMessages;


        private GameObject m_currentBridge;
        private Collider m_Collider;


        private void Awake()
        {
            m_Collider = GetComponent<Collider>();
        }

        public void BuildBridge(Collider col)
        {
          
            BallController ballController = col.GetComponent<BallController>();
            if (showDebugMessages)
            {
                Debug.Log("Attempting to build bridge!");
                Debug.Log(
                    $"{(ballController == null ? "Cant find player" : $"Player ({ballController.name}) has been found! Building bridge!")}");
            }

            if (ballController == null) return;

            if (showDebugMessages)
                Debug.Log($"Current Ball Size: {ballController.CurrentSize}");
            if (ballController.CurrentSize >= minSizeRequirement)
            {
                
                StartCoroutine(BeginApplyingBridge(col));
                m_Collider.enabled = false;
                IsBridgeBuilt = true;
            }
            
            onBridgeBuilding?.Invoke();
        }

        public void ResetBridge()
        {
            IsBridgeBuilt = false;
            m_Collider.enabled = true;
            if (!useCustomAnimation)
            {
                Destroy(m_currentBridge);
                return;
            }

            onResetBridgeAnimation?.Invoke();
        }


        private IEnumerator BeginApplyingBridge(Collider col)
        {
            AnimationController animationController = col.GetComponent<AnimationController>();
            Vector3 startPos = Vector3.zero;
            float dot = -1;
            int attempts = 0;
            while (Mathf.Sign(dot) == -1 && attempts <= 500)
            {
                startPos = col.transform.position + Random.insideUnitSphere;
                dot = Vector3.Dot((animationController.transform.position - waypointB).normalized,
                    startPos * col.bounds.size.x);
                attempts++;
            }

            yield return animationController.PlayGrabAnimation(startPos, waypointA);
            if (!useCustomAnimation)
                AddBridgeModel(MidPoint, bridgeWidthSize);
            else
                onBuildBridgeAnimation?.Invoke();
            m_Collider.enabled = false;
            yield return new WaitForSeconds(1f);
            yield return animationController.DisableGrabModels();
        }


        private void AddBridgeModel(Vector3 midPoint, float width)
        {
            var transform2 = transform;
            GameObject obj = Instantiate(bridgeModelPrefab, transform);
            obj.transform.position = waypointA;
            obj.transform.localScale = Vector3.one;
            obj.transform.DOMove(waypointA + (midPoint), bridgeSpawnSpeed).OnStart(() =>
            {
                obj.transform.localRotation = quaternion.LookRotation(midPoint.normalized, Vector3.up);
                obj.transform.localScale += transform2.right.normalized * width;
                obj.transform.DOScale(
                    obj.transform.localScale + (transform2.forward.normalized * midPoint.magnitude * 2f),
                    bridgeSpawnSpeed);
            }).SetEase(bridgeEaseType);

            m_currentBridge = obj;
            IsBridgeBuilt = true;
        }


        private void OnDrawGizmosSelected()
        {
            if (m_Collider == null) m_Collider = GetComponent<Collider>();
            Color yellow = Color.yellow;
            Color sphereSizeColor = Color.green - new Color(0, 0, 0, 0.7f);
            Color collisionColor = Color.cyan - new Color(0, 0, 0, 0.7f);
            DrawBridgeGizmos(yellow, sphereSizeColor, collisionColor);
        }

        private void OnDrawGizmos()
        {
            if (m_Collider == null) m_Collider = GetComponent<Collider>();
            Color yellow = Color.yellow - new Color(0.2f, 0.2f, 0.2f, 0);
            Color sphereSizeColor = Color.green - new Color(0.2f, 0.2f, 0.2f, 0.7f);
            Color collisionColor = Color.cyan - new Color(0.2f, 0.2f, 0.2f, 0.7f);
            DrawBridgeGizmos(yellow, sphereSizeColor, collisionColor);
        }

        private void DrawBridgeGizmos(Color midPointColor, Color sphereSizeColor, Color collisionColor)
        {
            Gizmos.color = midPointColor;
            Gizmos.DrawCube(waypointA + MidPoint, Vector3.one / 2f);

            Gizmos.color = sphereSizeColor;
            Gizmos.DrawSphere(transform.position - transform.forward.normalized * 5f, minSizeRequirement);
            
            DetectionArea.DrawBoxCollider(collisionColor, m_Collider);
        }

        private Vector3 MidPoint => (waypointB - waypointA) / 2f;
        public bool IsBridgeBuilt { get; private set; }
    }
}