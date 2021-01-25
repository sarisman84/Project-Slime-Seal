using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;


public static class DeleteDupBox
{
    [MenuItem("Tools/Extra/Delete Duplicate Colliders on selected GameObjects")]
    public static void DeleteDuplicateSelectedColliders()
    {
        List<Collider> confirmedDupes =
            Selection.gameObjects.Where(g => g.GetComponents<Collider>().Length > 1).ToList().ConvertAll(
                g => g.GetComponent<Collider>());

        DeleteDuplicates(confirmedDupes);
    }

    private static void DeleteDuplicates<TList>(TList confirmedDupes) where TList : IEnumerable<Component>
    {
        int dupCount = 0;
        foreach (var dupe in confirmedDupes)
        {
            var colliders = dupe.GetComponents<Collider>();
            if (colliders != null)
                for (int i = 0; i < colliders.Length - 1; i++)
                {
                    GameObject.DestroyImmediate(colliders[i]);
                    dupCount++;
                }
        }
        Debug.Log($"Deleted {dupCount} duplicate colliders");
    }

    [MenuItem("Tools/Extra/Delete Duplicate Colliders on all GameObjects in the current Scene.")]
    public static void DeleteAllDuplicateColliders()
    {
        DeleteDuplicates(Object.FindObjectsOfType<Collider>());
    }
}