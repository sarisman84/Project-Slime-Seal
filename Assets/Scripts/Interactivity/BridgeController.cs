using System;
using Player;
using Unity.Mathematics;
using UnityEngine;

namespace Interactivity
{
    public class BridgeController : MonoBehaviour
    {
        public float minSizeRequirement = 2.5f;
        public float bridgeWidthSize = 3f;
        public GameObject bridgeModelPrefab;

        public Vector3 waypointA, waypointB;

        public void BuildBridge(Collider col)
        {
            Debug.Log("Attempting to build bridge!");
            BallController ballController = col.GetComponent<BallController>();
            if (ballController == null) return;

            Debug.Log($"Current Ball Size: {ballController.CurrentSize}");
            if (ballController.CurrentSize >= minSizeRequirement)
            {
                AddBridgeModel(MidPoint, bridgeWidthSize);
                GetComponent<Collider>().enabled = false;
            }
        }

        private void AddBridgeModel(Vector3 midPoint, float width)
        {
            var transform2 = transform;
            GameObject obj = Instantiate(bridgeModelPrefab, transform);
            obj.transform.position = waypointA + (midPoint);
            obj.transform.localRotation = quaternion.LookRotation(midPoint.normalized, Vector3.up);
            obj.transform.localScale = obj.transform.localScale + (transform2.forward * midPoint.magnitude * 2f) +
                                       transform2.right * width;
        }


        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawCube(waypointA + MidPoint, Vector3.one / 2f);

            Gizmos.color = Color.green - new Color(0, 0, 0, 0.7f);
            Gizmos.DrawSphere(transform.position - transform.forward.normalized * 5f, minSizeRequirement);

            Gizmos.color = Color.cyan - new Color(0, 0, 0, 0.7f);
            Gizmos.DrawCube(transform.position, GetComponent<Collider>().bounds.size);
        }

        private Vector3 MidPoint => (waypointB - waypointA) / 2f;
    }
}