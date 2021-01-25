// using Interactivity;
// using TPUModelerEditor;
// using UnityEditor;
// using UnityEngine;
//
// namespace Editor
// {
//     [CustomPropertyDrawer(typeof(BubbleText.BubbleUI))]
//     public class BubbleTextDrawer : PropertyDrawer
//     {
//         private bool m_ValDur, m_ValTrans, m_ValEvent;
//
//         public override void OnGUI(Rect position, SerializedProperty property,
//             GUIContent label)
//         {
//             Rect r = position;
//             SerializedProperty it = property.serializedObject.GetIterator();
//             it.Next(true);
//             bool disableFirstElement = true;
//             while (it.NextVisible(false))
//             {
//                 EditorGUI.BeginDisabledGroup(disableFirstElement);
//                 r.height = EditorGUI.GetPropertyHeight(it, it.isExpanded);
//                 switch (it.propertyPath)
//                 {
//                     case "hasDuration":
//                     case "hasTransition":
//                     case "useCustomEvents":
//                         it.boolValue = EditorGUI.Toggle(r, new GUIContent(it.displayName), it.boolValue,
//                             GUIStyles.defaultButtonStyle);
//                         StoreBoolean(it.Copy());
//                         continue;
//
//                     case "displayDuration":
//                     case "transitionTime":
//                     case "onEnableUIElement":
//                     case "onDisableUIElement":
//                         if (GetBoolean(it.Copy()))
//                             EditorGUI.PropertyField(r, it, new GUIContent(it.displayName), false);
//                         continue;
//                 }
//
//                 EditorGUI.PropertyField(r, it, new GUIContent(it.displayName), false);
//                 r.y += r.height;
//                 EditorGUI.EndDisabledGroup();
//                 disableFirstElement = false;
//             }
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
//
//         public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
//         {
//             float totalHeight = base.GetPropertyHeight(property, label);
//
//             SerializedProperty it = property.serializedObject.GetIterator();
//             it.Next(true);
//             while (it.NextVisible(false))
//             {
//                 totalHeight += EditorGUI.GetPropertyHeight(it, it.isExpanded);
//             }
//
//             return totalHeight;
//         }
//     }
// }