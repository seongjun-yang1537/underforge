using System.Collections.Generic;
using UnityEngine;

namespace Underforge
{
    public static class DualContoruingVisualizer
    {
        private static readonly List<Color> AxisColors = new List<Color> { Color.red, Color.green, Color.blue };

        public static void DrawHermiteEdges(DualContouringDebugData data, float normalScale)
        {
            if (data == null) return;

            foreach (DualContouringHermiteSample sample in data.HermiteSamples)
            {
                int colorIndex = Mathf.Clamp(sample.Axis, 0, AxisColors.Count - 1);
                Gizmos.color = AxisColors[colorIndex];
                Gizmos.DrawSphere(sample.Position, 0.05f);
                Gizmos.DrawLine(sample.Position, sample.Position + sample.Normal * normalScale);
            }
        }

        public static void DrawVertices(DualContouringDebugData data, float vertexRadius)
        {
            if (data == null) return;

            Gizmos.color = Color.yellow;
            foreach (DualContouringVertexSample sample in data.VertexSamples)
            {
                Gizmos.DrawSphere(sample.Position, vertexRadius);
            }
        }

        public static void DrawMesh(DualContouringDebugData data)
        {
            if (data == null) return;

            List<Vector3> positions = data.MeshPositions;
            List<int> indices = data.MeshIndices;
            Gizmos.color = Color.white;
            for (int i = 0; i + 2 < indices.Count; i += 3)
            {
                Vector3 a = positions[indices[i]];
                Vector3 b = positions[indices[i + 1]];
                Vector3 c = positions[indices[i + 2]];
                Gizmos.DrawLine(a, b);
                Gizmos.DrawLine(b, c);
                Gizmos.DrawLine(c, a);
            }
        }

        public static void DrawAll(DualContouringDebugData data, float normalScale, float vertexRadius)
        {
            DrawHermiteEdges(data, normalScale);
            DrawVertices(data, vertexRadius);
            DrawMesh(data);
        }

        public static void DrawLatest(float normalScale, float vertexRadius)
        {
            DrawAll(DualContouring.LastDebugData, normalScale, vertexRadius);
        }
    }
}
