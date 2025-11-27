using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace Underforge
{
    public class DualContouringDebugData
    {
        public DualContouringDebugData(List<DualContouringHermiteSample> hermiteSamples, List<DualContouringVertexSample> vertexSamples, List<Vector3> meshPositions, List<int> meshIndices)
        {
            HermiteSamples = hermiteSamples;
            VertexSamples = vertexSamples;
            MeshPositions = meshPositions;
            MeshIndices = meshIndices;
        }

        public List<DualContouringHermiteSample> HermiteSamples { get; }

        public List<DualContouringVertexSample> VertexSamples { get; }

        public List<Vector3> MeshPositions { get; }

        public List<int> MeshIndices { get; }
    }

    public struct DualContouringHermiteSample
    {
        public DualContouringHermiteSample(int3 coordinate, int axis, Vector3 position, Vector3 normal)
        {
            Coordinate = coordinate;
            Axis = axis;
            Position = position;
            Normal = normal;
        }

        public int3 Coordinate { get; }

        public int Axis { get; }

        public Vector3 Position { get; }

        public Vector3 Normal { get; }
    }

    public struct DualContouringVertexSample
    {
        public DualContouringVertexSample(int3 coordinate, Vector3 position, Vector3 normal)
        {
            Coordinate = coordinate;
            Position = position;
            Normal = normal;
        }

        public int3 Coordinate { get; }

        public Vector3 Position { get; }

        public Vector3 Normal { get; }
    }
}
