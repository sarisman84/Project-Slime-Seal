using UnityEditor;
using UnityEngine;

namespace Editor
{
    public class DeleteDupBox : ScriptableWizard
    {
        [MenuItem("Tools/Remove dupe box colliders")]
        public static void CreateWizard()
        {
            DisplayWizard<DeleteDupBox>("Box Erazer 2000", "Delete Selected BoxColliders", "Delete All BoxColliders");
        }

        public void OnWizardCreate()
        {
            foreach (var obj in Selection.gameObjects)
            {
                if (obj.GetComponent<BoxCollider>() is { } boxCollider && boxCollider != null)
                {
                    DestroyImmediate(boxCollider);
                }
            }
        }

        public void OnWizardUpdate()
        {
            // EditorGUILayout.LabelField("Select a set amount of objects or just press Delete All.");
        }

        public void OnWizardOtherButton()
        {
            foreach (var box in FindObjectsOfType<BoxCollider>())
            {
                DestroyImmediate(box);
            }
           
        }
    }
}