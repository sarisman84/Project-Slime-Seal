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

        private void OnDisable()
        {
            SceneView.duringSceneGui -= SceneViewOnduringSceneGui;
        }

        private void SceneViewOnduringSceneGui(SceneView obj)
        {
            var rotation = m_Controller.transform.rotation;
           m_Controller.waypointA = Handles.PositionHandle(m_Controller.waypointA, rotation);
           m_Controller.waypointB = Handles.PositionHandle(m_Controller.waypointB, rotation);

            Handles.color = Color.magenta;
            Handles.DrawLine(m_Controller.waypointA, m_Controller.waypointB);
        }

    }
}