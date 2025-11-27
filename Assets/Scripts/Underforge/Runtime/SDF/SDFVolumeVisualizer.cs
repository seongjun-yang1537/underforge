using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace Underforge
{
    public static class SDFVolumeVisualizer
    {
        public static void DrawGizmos(in SDFVolume volume, float isoLevel = 0f, float cellSize = 1f)
        {
            Color previousColor = Gizmos.color;
            Vector3 cubeSize = Vector3.one * cellSize;

            for (int z = 0; z < volume.size.z; z++)
            {
                for (int y = 0; y < volume.size.y; y++)
                {
                    for (int x = 0; x < volume.size.x; x++)
                    {
                        int3 index = new int3(x, y, z);
                        float density = volume[index];

                        if (density > isoLevel)
                        {
                            continue;
                        }

                        float weight = math.saturate((isoLevel - density) / (math.abs(isoLevel) + 1f));
                        Gizmos.color = Color.Lerp(Color.cyan, Color.blue, weight);

                        float3 center = volume.origin + new float3(x + 0.5f, y + 0.5f, z + 0.5f);
                        Gizmos.DrawCube((Vector3)(center * cellSize), cubeSize);
                    }
                }
            }

            Gizmos.color = previousColor;
        }

        public static Mesh CreateMesh(in SDFVolume volume, float isoLevel = 0f, float cellSize = 1f)
        {
            List<Vector3> vertices = new List<Vector3>();
            List<int> triangles = new List<int>();

            for (int z = 0; z < volume.size.z; z++)
            {
                for (int y = 0; y < volume.size.y; y++)
                {
                    for (int x = 0; x < volume.size.x; x++)
                    {
                        int3 index = new int3(x, y, z);
                        float density = volume[index];

                        if (density > isoLevel)
                        {
                            continue;
                        }

                        Vector3 center = (Vector3)(volume.origin + new float3(x + 0.5f, y + 0.5f, z + 0.5f));
                        AppendCube(vertices, triangles, center, cellSize * 0.5f);
                    }
                }
            }

            Mesh mesh = new Mesh
            {
                vertices = vertices.ToArray(),
                triangles = triangles.ToArray()
            };

            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            return mesh;
        }

        private static void AppendCube(List<Vector3> vertices, List<int> triangles, Vector3 center, float halfSize)
        {
            Vector3[] offsets = new Vector3[8]
            {
                new Vector3(-halfSize, -halfSize, -halfSize),
                new Vector3(halfSize, -halfSize, -halfSize),
                new Vector3(halfSize, halfSize, -halfSize),
                new Vector3(-halfSize, halfSize, -halfSize),
                new Vector3(-halfSize, -halfSize, halfSize),
                new Vector3(halfSize, -halfSize, halfSize),
                new Vector3(halfSize, halfSize, halfSize),
                new Vector3(-halfSize, halfSize, halfSize)
            };

            int startIndex = vertices.Count;

            for (int i = 0; i < offsets.Length; i++)
            {
                vertices.Add(center + offsets[i]);
            }

            int[] faceIndices = new int[36]
            {
                0, 2, 1, 0, 3, 2,
                1, 6, 5, 1, 2, 6,
                5, 7, 4, 5, 6, 7,
                4, 3, 0, 4, 7, 3,
                3, 6, 2, 3, 7, 6,
                4, 1, 5, 4, 0, 1
            };

            for (int i = 0; i < faceIndices.Length; i++)
            {
                triangles.Add(startIndex + faceIndices[i]);
            }
        }
    }
}
