// using Interactivity;
// using UnityEditor;
// using UnityEngine;
//
// namespace Editor
// {
//     [CustomPropertyDrawer(typeof(BridgeController))]
//     public class BridgeDrawer : PropertyDrawer
//     {
//         
//         
//         private bool m_UseCustomAnim;
//
//         public override void OnGUI(Rect position, SerializedProperty property,
//             GUIContent label)
//         {
//             Rect r = position;
//
//            
//         }
//
//         public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
//         {
//             float height = GetPropertyHeight(property, label);
//             SerializedProperty it = property.serializedObject.GetIterator();
//             while (it.NextVisible(true))
//             {
//                 switch (it.propertyPath)
//                 {
//                     
//                     case "onBuildBridgeAnimation":
//                     case "onResetBridgeAnimation":
//                         if (m_UseCustomAnim)
//                             height += EditorGUI.GetPropertyHeight(it, it.isExpanded);
//                         break;
//                     default:
//                         height += EditorGUI.GetPropertyHeight(it, it.isExpanded);
//                         break;
//                 }
//             }
//
//             return height;
//         }
//     }
// }