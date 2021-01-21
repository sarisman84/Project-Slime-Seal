using UnityEngine;
namespace Vertex_Displacement
{
    public class Jellifier : MonoBehaviour
    {
        
    }

    public class JellyVertex
    {
        public int verticeIndex;
        public Vector3 initialVertexPosition;
        public Vector3 currentVertexPosition;

        public Vector3 currentVelocity;

        public JellyVertex(int _verticeIndex, Vector3 _initialVertexPosition, Vector3 _currentVertexPosition, Vector3 _currentVelocity)
        {
            verticeIndex = _verticeIndex;
            initialVertexPosition = _initialVertexPosition;
            currentVertexPosition = _currentVertexPosition;
            currentVelocity = _currentVelocity;
        }

        public Vector3 GetCurrentDisplacement()
        {
            return currentVertexPosition - initialVertexPosition;
        }
    }
}