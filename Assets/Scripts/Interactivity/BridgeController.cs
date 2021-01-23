using System;
using Player;
using UnityEngine;

namespace Interactivity
{
    public class BridgeController : MonoBehaviour
    {
        public float minSizeRequirement = 2.5f;
        public float bridgeWidthSize = 3f;
        public GameObject bridgeModelPrefab;

        [HideInInspector] public Vector3 waypointA, waypointB;

        public void BuildBridge(Collider col)
        {
            Debug.Log("Attempting to build bridge!");
            BallController ballController = col.GetComponent<BallController>();
            if (ballController == null) return;

            Debug.Log($"Current Ball Size: {ballController.CurrentSize}");
            if (ballController.CurrentSize >= minSizeRequirement)
            {
                AddBridgeModel((waypointA - waypointB) / 2f, bridgeWidthSize);
                GetComponent<Collider>().enabled = false;
            }
        }

        private void AddBridgeModel(Vector3 midPoint, float width)
        {
            var transform2 = transform;
            GameObject obj = Instantiate(bridgeModelPrefab, transform2.position - midPoint, transform2.rotation,
                transform2);
            obj.transform.localScale = obj.transform.localScale + (transform2.forward * midPoint.sqrMagnitude / 2f) +
                                       transform2.right * width;
        }


        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green - new Color(0, 0, 0, 0.7f);
            Gizmos.DrawSphere(transform.position - transform.forward.normalized * 5f, minSizeRequirement);
        }
    }
}