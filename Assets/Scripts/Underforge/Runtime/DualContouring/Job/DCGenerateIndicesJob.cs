using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using static Unity.Mathematics.math;

namespace Underforge
{
    [BurstCompile]
    public struct DCGenerateIndicesJob : IJobParallelFor
    {
        [ReadOnly] public SDFVolume volume;
        [ReadOnly] public NativeParallelHashMap<int, DCHermiteData> hermiteEdges;
        [ReadOnly] public NativeParallelHashMap<int, int> voxelToMeshIndex;

        [WriteOnly] public NativeList<int>.ParallelWriter outIndices;
        public void Execute(int index)
        {
            int strideXY = volume.size.x * volume.size.y;
            int z = index / strideXY;
            int rem = index % strideXY;
            int y = rem / volume.size.x;
            int x = rem % volume.size.x;
            int3 idx = int3(x, y, z);

            if (x + 1 >= volume.size.x || y + 1 >= volume.size.y || z + 1 >= volume.size.z)
                return;

            ProcessEdge(idx, 0);
            ProcessEdge(idx, 1);
            ProcessEdge(idx, 2);
        }

        private void ProcessEdge(int3 idx, int axis)
        {
            int edgeKey = DCEdgeHelper.CompressEdgeIndex(idx, axis);

            if (!hermiteEdges.ContainsKey(edgeKey)) return;

            int3 v0, v1, v2, v3;

            if (axis == 0)
            {
                v0 = idx;
                v1 = idx + int3(0, 1, 0);
                v2 = idx + int3(0, 0, 1);
                v3 = idx + int3(0, 1, 1);
            }
            else if (axis == 1)
            {
                v0 = idx;
                v1 = idx + int3(1, 0, 0);
                v2 = idx + int3(0, 0, 1);
                v3 = idx + int3(1, 0, 1);
            }
            else
            {
                v0 = idx;
                v1 = idx + int3(1, 0, 0);
                v2 = idx + int3(0, 1, 0);
                v3 = idx + int3(1, 1, 0);
            }

            if (!GetMeshIndex(v0, out int i0) || !GetMeshIndex(v1, out int i1) ||
                !GetMeshIndex(v2, out int i2) || !GetMeshIndex(v3, out int i3))
                return;

            float3 edgeNormal = hermiteEdges[edgeKey].normal;
            bool flip = (axis == 0 && edgeNormal.x > 0) ||
                        (axis == 1 && edgeNormal.y > 0) ||
                        (axis == 2 && edgeNormal.z > 0);

            if (axis == 0)
            {
                AddQuad(i0, i1, i3, i2, flip);
            }
            else if (axis == 1)
            {
                AddQuad(i0, i2, i3, i1, flip);
            }
            else
            {
                AddQuad(i0, i1, i3, i2, flip);
            }
        }

        private bool GetMeshIndex(int3 idx, out int meshIdx)
        {
            int key = volume.FlattenIndex(idx);
            return voxelToMeshIndex.TryGetValue(key, out meshIdx);
        }

        private void AddQuad(int a, int b, int c, int d, bool flip)
        {
            if (flip)
            {
                outIndices.AddNoResize(a); outIndices.AddNoResize(c); outIndices.AddNoResize(b);
                outIndices.AddNoResize(a); outIndices.AddNoResize(d); outIndices.AddNoResize(c);
            }
            else
            {
                outIndices.AddNoResize(a); outIndices.AddNoResize(b); outIndices.AddNoResize(c);
                outIndices.AddNoResize(a); outIndices.AddNoResize(c); outIndices.AddNoResize(d);
            }
        }
    }
}