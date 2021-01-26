using UnityEngine;

namespace Interactivity
{
    public partial class DetectionArea
    {
        public static void DrawBoxCollider(Color gizmoColor, Collider boxCollider, float alphaForInsides = 0.3f)
        {
            //Save the color in a temporary variable to not overwrite changes in the inspector (if the sent-in color is a serialized variable).
            var color = gizmoColor;

            //Change the gizmo matrix to the relative space of the boxCollider.
            //This makes offsets with rotation work
            //Source: https://forum.unity.com/threads/gizmo-rotation.4817/#post-3242447
            Gizmos.matrix = Matrix4x4.TRS(boxCollider.bounds.center, boxCollider.transform.rotation,
                boxCollider.transform.lossyScale);

            //Draws the edges of the BoxCollider
            //Center is Vector3.zero, since we've transformed the calculation space in the previous step.
            Gizmos.color = color;
            Gizmos.DrawWireCube(Vector3.zero,
                boxCollider is BoxCollider collider1 ? collider1.size : boxCollider.bounds.size);

            //Draws the sides/insides of the BoxCollider, with a tint to the original color.
            color.a *= alphaForInsides;
            Gizmos.color = color;
            Gizmos.DrawCube(Vector3.zero,
                boxCollider is BoxCollider collider2 ? collider2.size : boxCollider.bounds.size);
        }
    }
}