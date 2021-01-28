// using Interactivity;
// using TPUModelerEditor;
// using UnityEditor;
// using UnityEngine;
//
// namespace Editor
// {
//     [CustomPropertyDrawer(typeof(BubbleRenderer.BubbleUI))]
//     public class BubbleTextDrawer : PropertyDrawer
//     {
//         private bool m_ValDur, m_ValTrans, m_ValEvent;
//
//         public override void OnGUI(Rect position, SerializedProperty property,
//             GUIContent label)
//         {
//             property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, property.displayName);
//             if (property.isExpanded)
//             {
//                 Rect r = position;
//                 SerializedProperty it = property.serializedObject.GetIterator();
//                 it.Next(true);
//                 bool disableFirstElement = true;
//                 while (it.NextVisible(false))
//                 {
//                     if (disableFirstElement)
//                     {
//                         disableFirstElement = false;
//                         continue;
//                     }
//
//                     r.height = EditorGUI.GetPropertyHeight(it.Copy(), it.Copy().isExpanded);
//                     EditorGUI.PropertyField(r, it.Copy(), new GUIContent(it.Copy().displayName), it.Copy().isExpanded);
//                     r.y += r.height;
//                 }
//             }
//
//             property.serializedObject.ApplyModifiedProperties();
//         }
//
//         private void DrawProperty(Rect r, SerializedProperty it)
//         {
//         }
//
//         private bool GetBoolean(SerializedProperty it)
//         {
//             switch (it.propertyPath)
//             {
//                 case "hasDuration":
//                     return m_ValDur;
//                 case "hasTransition":
//                     return m_ValTrans;
//                 case "useCustomEvents":
//                     return m_ValEvent;
//             }
//
//             return false;
//         }
//
//         private void StoreBoolean(SerializedProperty it)
//         {
//             switch (it.propertyPath)
//             {
//                 case "hasDuration":
//                     m_ValDur = it.boolValue;
//                     return;
//                 case "hasTransition":
//                     m_ValTrans = it.boolValue;
//                     return;
//                 case "useCustomEvents":
//                     m_ValEvent = it.boolValue;
//                     break;
//             }
//         }
//
//         private float totalHeight = 0;
//
//         public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
//         {
//             totalHeight = 0;
//             if (property.isExpanded)
//             {
//                 SerializedProperty it = property.serializedObject.GetIterator();
//                 it.Next(true);
//                 while (it.NextVisible(true))
//                 {
//                     totalHeight += EditorGUI.GetPropertyHeight(it.Copy(), it.Copy().isExpanded);
//                 }
//             }
//
//             return totalHeight + base.GetPropertyHeight(property, label);
//         }
//     }
// }