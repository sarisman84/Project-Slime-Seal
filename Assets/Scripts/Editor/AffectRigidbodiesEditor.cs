using System;
using Interactivity;
using TPUModelerEditor;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(AffectRigidbodies))]
    public class AffectRigidbodiesEditor : UnityEditor.Editor
    {
        private AffectRigidbodies m_AffectRigidbodies;

        public enum DefaultForwardDirection
        {
            Left,
            Right,
            Forward,
            Backwards,
            Up,
            Down
        }

        public DefaultForwardDirection defaultForwardDirection;

        private void OnEnable()
        {
            m_AffectRigidbodies = target as AffectRigidbodies;
            SceneView.duringSceneGui += RenderForce;
        }

        private float Distance
        {
            get
            {
                switch (defaultForwardDirection)
                {
                    case DefaultForwardDirection.Left:
                    case DefaultForwardDirection.Right:
                        return m_AffectRigidbodies.GetComponent<Collider>() is { } colNx && colNx != null
                            ? Mathf.Max(m_AffectRigidbodies.transform.localScale.x, colNx.bounds.size.x)
                            : -m_AffectRigidbodies.transform.localScale.x;
                    case DefaultForwardDirection.Backwards:
                    case DefaultForwardDirection.Forward:
                        return m_AffectRigidbodies.GetComponent<Collider>() is { } colz && colz != null
                            ? Mathf.Max(m_AffectRigidbodies.transform.localScale.z, colz.bounds.size.z)
                            : m_AffectRigidbodies.transform.localScale.z;
                    
                    case DefaultForwardDirection.Up:
                    case DefaultForwardDirection.Down:
                        return m_AffectRigidbodies.GetComponent<Collider>() is { } colNy && colNy != null
                            ? Mathf.Max(m_AffectRigidbodies.transform.localScale.y, colNy.bounds.size.y)
                            : m_AffectRigidbodies.transform.localScale.y;
                }

                return 0;
            }
        }

        private Vector3 ForceDirection => (m_AffectRigidbodies.force - m_AffectRigidbodies.transform.position);

        private void OnDisable()
        {
            SceneView.duringSceneGui -= RenderForce;
        }

        private void RenderForce(SceneView obj)
        {
            m_AffectRigidbodies.force =
                Handles.PositionHandle(m_AffectRigidbodies.force, m_AffectRigidbodies.transform.rotation);

            Handles.color = Color.cyan;
            Handles.DrawDottedLine(m_AffectRigidbodies.force, m_AffectRigidbodies.transform.position, 4);
            Handles.ConeHandleCap(HandleUtility.nearestControl, m_AffectRigidbodies.force - (ForceDirection / 2f),
                Quaternion.LookRotation(ForceDirection.normalized, Vector3.up), (ForceDirection.magnitude + 1f) / 2f,
                EventType.Repaint);

            Handles.CubeHandleCap(HandleUtility.nearestControl, m_AffectRigidbodies.force,
                quaternion.LookRotation(ForceDirection.normalized, Vector3.up), 0.5f, EventType.Repaint);
        }


        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            defaultForwardDirection =
                (DefaultForwardDirection) EditorGUILayout.EnumPopup(defaultForwardDirection,
                    GUIStyles.defaultButtonStyle);
            if (GUILayout.Button("Reset Waypoint"))
            {
                switch (defaultForwardDirection)
                {
                    case DefaultForwardDirection.Left:
                        m_AffectRigidbodies.force = m_AffectRigidbodies.transform.position -
                                                    m_AffectRigidbodies.transform.right.normalized * (2f + Distance);
                        break;
                    case DefaultForwardDirection.Right:
                        m_AffectRigidbodies.force = m_AffectRigidbodies.transform.position +
                                                    m_AffectRigidbodies.transform.right.normalized * (2f + Distance);
                        break;
                    case DefaultForwardDirection.Forward:
                        m_AffectRigidbodies.force = m_AffectRigidbodies.transform.position +
                                                    m_AffectRigidbodies.transform.forward.normalized * (2f + Distance);
                        break;
                    case DefaultForwardDirection.Backwards:
                        m_AffectRigidbodies.force = m_AffectRigidbodies.transform.position -
                                                    m_AffectRigidbodies.transform.forward.normalized * (2f + Distance);
                        break;
                    case DefaultForwardDirection.Up:
                        m_AffectRigidbodies.force = m_AffectRigidbodies.transform.position +
                                                    m_AffectRigidbodies.transform.up.normalized * (2f + Distance);
                        break;
                    case DefaultForwardDirection.Down:
                        m_AffectRigidbodies.force = m_AffectRigidbodies.transform.position -
                                                    m_AffectRigidbodies.transform.up.normalized * (2f + Distance);
                        break;
                }
            }
        }
    }
}