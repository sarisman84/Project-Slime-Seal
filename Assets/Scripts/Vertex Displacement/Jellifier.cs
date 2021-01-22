using System;
using UnityEngine;
namespace Vertex_Displacement
{
    public class Jellifier : MonoBehaviour
    {
        public float bounceSpeed;
        public float fallForce;
        public float stiffness;

        private MeshFilter _meshFilter;
        private Mesh mesh;

        private JellyVertex[] _jellyVertices;
        private Vector3[] _currentMeshVertices;

        private void Start()
        {
            _meshFilter = GetComponent<MeshFilter>();
            mesh = _meshFilter.mesh;

            GetVertices();
        }

        private void GetVertices()
        {
            _jellyVertices = new JellyVertex[mesh.vertices.Length];
            _currentMeshVertices = new Vector3[mesh.vertices.Length];
            for (int i = 0; i < mesh.vertices.Length; i++)
            {
                _jellyVertices[i] = new JellyVertex(i, mesh.vertices[i], mesh.vertices[i], Vector3.zero);
                _currentMeshVertices[i] = mesh.vertices[i];
            }
        }

        private void Update()
        {
            UpdateVertices();
        }

        private void UpdateVertices()
        {
            for (int i = 0; i < _jellyVertices.Length; i++)
            {
                _jellyVertices[i].UpdateVelocity(bounceSpeed);
                _jellyVertices[i].Settle(stiffness);

                _jellyVertices[i].currentVertexPosition += _jellyVertices[i].currentVelocity * Time.deltaTime;
                _currentMeshVertices[i] = _jellyVertices[i].currentVertexPosition;
            }

            mesh.vertices = _currentMeshVertices;
            mesh.RecalculateBounds();
            mesh.RecalculateNormals();
            mesh.RecalculateTangents();
        }

        public void OnCollision(Collision other)
        {
            ContactPoint[] collisonPoints = other.contacts;
            for (int i = 0; i < collisonPoints.Length; i++)
            {
                Vector3 inputPoint = collisonPoints[i].point + (collisonPoints[i].point * 2f);
                ApplyPressureToPoint(inputPoint, fallForce);
            }
        }

        private void ApplyPressureToPoint(Vector3 inputPoint, float f)
        {
            for (int i = 0; i < _jellyVertices.Length; i++)
            {
                _jellyVertices[i].ApplyPressureToVertex(transform, inputPoint, f);
            }
        }
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

        public void UpdateVelocity(float _bounceSpeed)
        {
            currentVelocity = currentVelocity - GetCurrentDisplacement() * (_bounceSpeed * Time.deltaTime);
        }

        public void Settle(float _stiffness)
        {
            currentVelocity *= 1f - _stiffness * Time.deltaTime;
        }

        public void ApplyPressureToVertex(Transform _transform, Vector3 _position, float _pressure)
        {
            Vector3 distanceVerticePoint = currentVertexPosition - _transform.InverseTransformPoint(-_position);
            float adaptedPressure = _pressure / (1f + distanceVerticePoint.sqrMagnitude);
            float velocity = adaptedPressure * Time.deltaTime;
            currentVelocity += distanceVerticePoint.normalized * velocity;
        }
        
    }
}