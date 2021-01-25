using UnityEditor;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Editor
{
    public class ObjectReplacer : ScriptableWizard
    {
        public GameObject prefab;

        [MenuItem("Tools/Extra/Replace Tool")]
        public static void CreateWizard()
        {
            DisplayWizard<ObjectReplacer>("Replace Tool", "Replace");
        }

        public void OnWizardCreate()
        {
            Object[] foundObjects = Selection.objects;

            foreach (var o in foundObjects)
            {
                var obj = (GameObject) o;
                if (DoesObjectContainPrefabName(obj))
                {
                    GameObject newObj = Instantiate(prefab, obj.transform.parent);
                    newObj.transform.position = obj.transform.position;
                    newObj.transform.rotation = obj.transform.rotation;
                    Debug.Log($"Replacing {obj.name} with {newObj.name}");
                    obj.SetActive(false);
                }
            }
        }

        private bool DoesObjectContainPrefabName(GameObject obj)
        {
            return obj.name.ToLower().Contains(prefab.name.ToLower());
        }
    }
}