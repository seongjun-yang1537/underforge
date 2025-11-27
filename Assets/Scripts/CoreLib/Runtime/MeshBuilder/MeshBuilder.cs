using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;

namespace Corelib.Utils
{
    public class MeshBuilder
    {
        public CompareFunction ZTest { get; set; } = CompareFunction.LessEqual;
        private UnityAction<BuildState> meshBuildAction;

        private MeshBuilder(UnityAction<BuildState> meshBuildAction)
        {
            this.meshBuildAction = meshBuildAction;
        }

        public Mesh Build()
        {
            BuildState buildState = new BuildState();
            meshBuildAction?.Invoke(buildState);
            return buildState.ToMesh();
        }

        public async UniTask<Mesh> BuildAsync()
        {
            BuildState buildState = await UniTask.RunOnThreadPool(() =>
            {
                BuildState s = new BuildState();
                meshBuildAction?.Invoke(s);
                return s;
            });

            await UniTask.SwitchToMainThread();

            Mesh mesh = new Mesh();
            mesh.SetVertices(buildState.vertices);
            mesh.SetColors(buildState.colors);
            mesh.SetIndices(buildState.triangleIndices.ToArray(), MeshTopology.Triangles, 0);

            if (buildState.lineIndices.Count > 0)
                mesh.SetIndices(buildState.lineIndices.ToArray(), MeshTopology.Lines, 1);

            return mesh;
        }

        public static MeshBuilder operator +(MeshBuilder first, MeshBuilder second)
        {
            UnityAction<BuildState> combinedAction = null;
            if (first?.meshBuildAction != null)
            {
                combinedAction = (UnityAction<BuildState>)Delegate.Combine(combinedAction, first.meshBuildAction);
            }
            if (second?.meshBuildAction != null)
            {
                combinedAction = (UnityAction<BuildState>)Delegate.Combine(combinedAction, second.meshBuildAction);
            }

            MeshBuilder builder = new MeshBuilder(combinedAction);
            if (first != null)
            {
                builder.ZTest = first.ZTest;
            }
            else if (second != null)
            {
                builder.ZTest = second.ZTest;
            }

            return builder;
        }

        public static MeshBuilder operator |(MeshBuilder builder, UnityAction<MeshBuilder> action)
        {
            action?.Invoke(builder);
            return builder;
        }

        public static MeshBuilder Empty()
        {
            return new MeshBuilder(null);
        }

        public static MeshBuilder Box(Vector3 center, Vector3 size, Color fillColor, Color wireColor, bool fill, bool wire)
        {
            return new MeshBuilder(state => AddBox(state, center, size, fillColor, wireColor, fill, wire));
        }

        public static MeshBuilder Sphere(Vector3 center, float radius, Color fillColor, Color wireColor, bool fill, bool wire, int segments = 12)
        {
            return new MeshBuilder(state => AddSphere(state, center, radius, fillColor, wireColor, fill, wire, segments));
        }

        public static MeshBuilder Line(Vector3 start, Vector3 end, Color wireColor)
        {
            return new MeshBuilder(state => AddLine(state, start, end, wireColor));
        }

        public MeshBuilder AddTriangle(Vector3 vertex0, Vector3 vertex1, Vector3 vertex2, Color fillColor)
        {
            meshBuildAction += state => AddTriangle(state, vertex0, vertex1, vertex2, fillColor);
            return this;
        }

        public MeshBuilder AddLine(Vector3 start, Vector3 end, Color wireColor)
        {
            meshBuildAction += state => AddLine(state, start, end, wireColor);
            return this;
        }

        private class BuildState
        {
            public List<Vector3> vertices = new List<Vector3>();
            public List<Color> colors = new List<Color>();
            public List<int> triangleIndices = new List<int>();
            public List<int> lineIndices = new List<int>();

            public Mesh ToMesh()
            {
                Mesh mesh = new Mesh();
                mesh.SetVertices(vertices);
                mesh.SetColors(colors);
                mesh.SetIndices(triangleIndices.ToArray(), MeshTopology.Triangles, 0);
                mesh.SetIndices(lineIndices.ToArray(), MeshTopology.Lines, 1);
                return mesh;
            }
        }

        private static void AddBox(BuildState state, Vector3 center, Vector3 size, Color fillColor, Color wireColor, bool fill, bool wire)
        {
            Vector3 halfSize = size * 0.5f;
            Vector3 vertex0 = center + new Vector3(-halfSize.x, -halfSize.y, -halfSize.z);
            Vector3 vertex1 = center + new Vector3(halfSize.x, -halfSize.y, -halfSize.z);
            Vector3 vertex2 = center + new Vector3(halfSize.x, halfSize.y, -halfSize.z);
            Vector3 vertex3 = center + new Vector3(-halfSize.x, halfSize.y, -halfSize.z);
            Vector3 vertex4 = center + new Vector3(-halfSize.x, -halfSize.y, halfSize.z);
            Vector3 vertex5 = center + new Vector3(halfSize.x, -halfSize.y, halfSize.z);
            Vector3 vertex6 = center + new Vector3(halfSize.x, halfSize.y, halfSize.z);
            Vector3 vertex7 = center + new Vector3(-halfSize.x, halfSize.y, halfSize.z);

            if (fill)
            {
                int startIndex = state.vertices.Count;
                state.vertices.AddRange(new[] { vertex0, vertex1, vertex2, vertex3, vertex4, vertex5, vertex6, vertex7 });
                for (int vertexIndex = 0; vertexIndex < 8; vertexIndex++) state.colors.Add(fillColor);
                state.triangleIndices.AddRange(new int[]
                {
                    startIndex + 4, startIndex + 5, startIndex + 6, startIndex + 6, startIndex + 7, startIndex + 4,
                    startIndex + 1, startIndex + 0, startIndex + 3, startIndex + 3, startIndex + 2, startIndex + 1,
                    startIndex + 0, startIndex + 4, startIndex + 7, startIndex + 7, startIndex + 3, startIndex + 0,
                    startIndex + 5, startIndex + 1, startIndex + 2, startIndex + 2, startIndex + 6, startIndex + 5,
                    startIndex + 3, startIndex + 7, startIndex + 6, startIndex + 6, startIndex + 2, startIndex + 3,
                    startIndex + 0, startIndex + 1, startIndex + 5, startIndex + 5, startIndex + 4, startIndex + 0
                });
            }
            if (wire)
            {
                int startIndex = state.vertices.Count;
                state.vertices.AddRange(new[] { vertex0, vertex1, vertex2, vertex3, vertex4, vertex5, vertex6, vertex7 });
                for (int vertexIndex = 0; vertexIndex < 8; vertexIndex++) state.colors.Add(wireColor);
                state.lineIndices.AddRange(new int[]
                {
                    startIndex + 0, startIndex + 1, startIndex + 1, startIndex + 2, startIndex + 2, startIndex + 3, startIndex + 3, startIndex + 0,
                    startIndex + 4, startIndex + 5, startIndex + 5, startIndex + 6, startIndex + 6, startIndex + 7, startIndex + 7, startIndex + 4,
                    startIndex + 0, startIndex + 4, startIndex + 1, startIndex + 5, startIndex + 2, startIndex + 6, startIndex + 3, startIndex + 7
                });
            }
        }

        private static void AddSphere(BuildState state, Vector3 center, float radius, Color fillColor, Color wireColor, bool fill, bool wire, int segments)
        {
            if (fill)
            {
                int startIndex = state.vertices.Count;
                for (int latitude = 0; latitude <= segments; latitude++)
                {
                    float latitudeRatio = latitude / (float)segments;
                    float phiAngle = Mathf.PI * latitudeRatio;
                    for (int longitude = 0; longitude <= segments; longitude++)
                    {
                        float longitudeRatio = longitude / (float)segments;
                        float thetaAngle = Mathf.PI * 2f * longitudeRatio;
                        float sinPhiAngle = Mathf.Sin(phiAngle);
                        Vector3 point = new Vector3(Mathf.Cos(thetaAngle) * sinPhiAngle, Mathf.Cos(phiAngle), Mathf.Sin(thetaAngle) * sinPhiAngle) * radius + center;
                        state.vertices.Add(point);
                        state.colors.Add(fillColor);
                    }
                }
                for (int latitude = 0; latitude < segments; latitude++)
                {
                    for (int longitude = 0; longitude < segments; longitude++)
                    {
                        int index0 = startIndex + latitude * (segments + 1) + longitude;
                        int index1 = startIndex + (latitude + 1) * (segments + 1) + longitude;
                        int index2 = startIndex + (latitude + 1) * (segments + 1) + longitude + 1;
                        int index3 = startIndex + latitude * (segments + 1) + longitude + 1;
                        state.triangleIndices.AddRange(new int[] { index0, index1, index2, index2, index3, index0 });
                    }
                }
            }
            if (wire)
            {
                int startIndex = state.vertices.Count;
                for (int axisIndex = 0; axisIndex < 3; axisIndex++)
                {
                    for (int segmentIndex = 0; segmentIndex < segments; segmentIndex++)
                    {
                        float angle0 = segmentIndex / (float)segments * Mathf.PI * 2f;
                        float angle1 = (segmentIndex + 1) / (float)segments * Mathf.PI * 2f;
                        Vector3 point0;
                        Vector3 point1;
                        if (axisIndex == 0)
                        {
                            point0 = center + new Vector3(0, Mathf.Cos(angle0) * radius, Mathf.Sin(angle0) * radius);
                            point1 = center + new Vector3(0, Mathf.Cos(angle1) * radius, Mathf.Sin(angle1) * radius);
                        }
                        else if (axisIndex == 1)
                        {
                            point0 = center + new Vector3(Mathf.Cos(angle0) * radius, 0, Mathf.Sin(angle0) * radius);
                            point1 = center + new Vector3(Mathf.Cos(angle1) * radius, 0, Mathf.Sin(angle1) * radius);
                        }
                        else
                        {
                            point0 = center + new Vector3(Mathf.Cos(angle0) * radius, Mathf.Sin(angle0) * radius, 0);
                            point1 = center + new Vector3(Mathf.Cos(angle1) * radius, Mathf.Sin(angle1) * radius, 0);
                        }
                        state.vertices.Add(point0);
                        state.vertices.Add(point1);
                        state.colors.Add(wireColor);
                        state.colors.Add(wireColor);
                        state.lineIndices.Add(startIndex + segmentIndex * 2);
                        state.lineIndices.Add(startIndex + segmentIndex * 2 + 1);
                    }
                    startIndex += segments * 2;
                }
            }
        }

        private static void AddTriangle(BuildState state, Vector3 vertex0, Vector3 vertex1, Vector3 vertex2, Color fillColor)
        {
            int startIndex = state.vertices.Count;
            state.vertices.Add(vertex0);
            state.vertices.Add(vertex1);
            state.vertices.Add(vertex2);
            state.colors.Add(fillColor);
            state.colors.Add(fillColor);
            state.colors.Add(fillColor);
            state.triangleIndices.Add(startIndex);
            state.triangleIndices.Add(startIndex + 1);
            state.triangleIndices.Add(startIndex + 2);
        }

        private static void AddLine(BuildState state, Vector3 start, Vector3 end, Color wireColor)
        {
            int startIndex = state.vertices.Count;
            state.vertices.Add(start);
            state.vertices.Add(end);
            state.colors.Add(wireColor);
            state.colors.Add(wireColor);
            state.lineIndices.Add(startIndex);
            state.lineIndices.Add(startIndex + 1);
        }
    }
}
