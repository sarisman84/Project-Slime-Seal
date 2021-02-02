using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TPUModelerEditor;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

namespace Editor
{
    public class DeleteDupBox : EditorWindow
    {
        // public static void DeleteDuplicateSelectedColliders()
        // {
        //     List<Collider> confirmedDupes =
        //         Selection.gameObjects.Where(g => g.GetComponents<Collider>().Length > 1).ToList().ConvertAll(
        //             g => g.GetComponent<Collider>());
        //
        //     DeleteDuplicates(confirmedDupes);
        // }

        private void DeleteObjects(List<GameObject> confirmedDupes, int amountRemaining)
        {
            int dupCount = 0;
            foreach (var dupe in confirmedDupes)
            {
                var colliders = GetDupeCollider(dupe, m_MemberNames[m_CurIndex]);
                if (colliders != null)
                    for (int i = 0; i < colliders.Length - amountRemaining; i++)
                    {
                        DestroyImmediate(colliders[i]);
                        dupCount++;
                    }
            }

            Debug.Log($"Deleted {dupCount} duplicate colliders");
        }

        private Collider[] GetDupeCollider(GameObject dupe, string memberName)
        {
            switch (memberName)
            {
                case "Box Collider":
                    return dupe.GetComponents<BoxCollider>();
                case "Sphere Collider":
                    return dupe.GetComponents<SphereCollider>();
                case "Mesh Collider":
                    return dupe.GetComponents<MeshCollider>();
                default:
                    return default;
            }
        }


        public bool keepAtleastOneObjectInScene;

        private List<string> m_MemberNames = new List<string>();
        private int m_CurIndex;

        private void OnEnable()
        {
            m_MemberNames = new List<string>()
            {
                "Box Collider", "Sphere Collider", "Mesh Collider", "GameObject"
            };
        }


        private void OnGUI()
        {
            keepAtleastOneObjectInScene = EditorGUILayout.Toggle(new GUIContent("Keep one object per selection?"),
                keepAtleastOneObjectInScene);
            
            
            List<GameObject> gmObjs;
            m_CurIndex = EditorGUILayout.Popup(new GUIContent("Object Type"), m_CurIndex, m_MemberNames.ToArray());

            if (GUILayout.Button(
                $"Select all objects containing {m_MemberNames[m_CurIndex]} in current Scene and delete the {m_MemberNames[m_CurIndex]}. ")
            )
            {
                gmObjs = FindObjectsOfType<GameObject>().Where(g =>
                {
                    switch (m_MemberNames[m_CurIndex])
                    {
                        case "Box Collider":
                            return true;
                        case "Sphere Collider":
                            return true;
                        case "Mesh Collider":
                            return true;
                        default:
                            return false;
                    }
                }).ToList();

                DeleteObjects(gmObjs.ToList(), keepAtleastOneObjectInScene ? 1 : 0);
            }

            if (Selection.gameObjects.FirstOrDefault(g => g.scene is { } scene && scene == SceneManager.GetActiveScene()) ==
                null) return;

            gmObjs = Array.FindAll(Selection.gameObjects,
                g => g.scene == SceneManager.GetActiveScene()).ToList();
            if (GUILayout.Button($"Delete selected {m_MemberNames[m_CurIndex]} objects (Currently Selected Amount : {gmObjs.Count})"))
            {
               

                gmObjs = gmObjs.Where(g =>
                {
                    switch (m_MemberNames[m_CurIndex])
                    {
                        case "Box Collider":
                            return true;
                        case "Sphere Collider":
                            return true;
                        case "Mesh Collider":
                            return true;
                        default:
                            return false;
                    }
                }).ToList();

                DeleteObjects(gmObjs.ToList(), keepAtleastOneObjectInScene ? 1 : 0);
            }
        }


        [MenuItem("Tools/Extra/Mass Delete Tool")]
        public static void OpenWindow()
        {
            GetWindow<DeleteDupBox>("Mass Delete Tool");
        }
    }
}