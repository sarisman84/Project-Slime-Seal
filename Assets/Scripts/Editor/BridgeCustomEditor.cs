using System;
using Interactivity;
using UnityEditor;
using UnityEditor.SearchService;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(BridgeController))]
    public class BridgeCustomEditor : UnityEditor.Editor
    {
        private BridgeController m_Controller;
        private Vector3 m_TempPos1, m_TempPos2;

        private void OnEnable()
        {
            m_Controller = target as BridgeController;

            if (m_Controller is { })
            {
                var transform = m_Controller.transform;
                var position = transform.position;
                m_Controller.waypointA = m_Controller.waypointA == Vector3.zero
                    ? position + transform.forward.normalized
                    : m_Controller.waypointA;

                m_Controller.waypointB = m_Controller.waypointB == Vector3.zero
                    ? position - transform.forward.normalized
                    : m_Controller.waypointB;
            }


            SceneView.duringSceneGui += SceneViewOnduringSceneGui;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("ResetWaypoints"))
            {
                m_Controller.waypointA =
                    m_Controller.transform.position + m_Controller.transform.forward.normalized * 5f;
                m_Controller.waypointB =
                    m_Controller.transform.position - m_Controller.transform.forward.normalized * 5f;
            }
        }

        private void OnDisable()
        {
            SceneView.duringSceneGui -= SceneViewOnduringSceneGui;
        }

        private void SceneViewOnduringSceneGui(SceneView obj)
        {
            var rotation = m_Controller.transform.rotation;

            m_Controller.waypointA = Handles.PositionHandle( m_Controller.waypointA, rotation);
            m_Controller.waypointB = Handles.PositionHandle(m_Controller.waypointB, rotation);

            Handles.color = Color.magenta;
            Handles.DrawLine(m_Controller.waypointA, m_Controller.waypointB);
        }
    }
}