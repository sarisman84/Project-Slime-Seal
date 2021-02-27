using System;
using Interactivity;
using TPUModelerEditor;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.XR;

namespace Editor
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(AffectRigidbodies))]
    public class AffectRigidbodiesEditor : UnityEditor.Editor
    {
        private AffectRigidbodies m_Arb;

        private void OnEnable()
        {
            m_Arb = target as AffectRigidbodies;
            SceneView.duringSceneGui += DrawPositionHandle;
        }

        private void OnDisable()
        {
            SceneView.duringSceneGui -= DrawPositionHandle;
        }

        private void DrawPositionHandle(SceneView obj)
        {
            var transform = m_Arb.transform;
            var position = transform.position;
            var rotation = transform.rotation;

            m_Arb.direction =
                Handles.PositionHandle(m_Arb.direction + position, rotation) -
                position;
            Handles.color = Color.yellow;
            Handles.SphereHandleCap(GUIUtility.GetControlID(FocusType.Passive), position,
                rotation, 0.5f, EventType.Repaint);
            Handles.color = Color.cyan;
            Handles.DrawDottedLine(position, m_Arb.direction + position, 4f);

            if (m_Arb.direction != Vector3.zero)
                Handles.ConeHandleCap(GUIUtility.GetControlID(FocusType.Passive),
                    (m_Arb.direction + position + m_Arb.direction.normalized * (1f + m_Arb.forceAmm)),
                    Quaternion.LookRotation(m_Arb.direction.normalized, m_Arb.transform.up), 1f + m_Arb.forceAmm,
                    EventType.Repaint);
        }
    }
}