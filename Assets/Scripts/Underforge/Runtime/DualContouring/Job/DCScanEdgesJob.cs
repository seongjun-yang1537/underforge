using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using static Unity.Mathematics.math;

namespace Underforge
{
    [BurstCompile]
    public struct DCScanEdgesJob : IJobParallelFor
    {
        [ReadOnly] public SDFVolume volume;

        [WriteOnly] public NativeParallelHashMap<int, HermiteData>.ParallelWriter hermiteEdges;

        public void Execute(int index)
        {
            int strideXY = volume.size.x * volume.size.y;
            int z = index / strideXY;
            int rem = index % strideXY;
            int y = rem / volume.size.x;
            int x = rem % volume.size.x;

            int3 idx = int3(x, y, z);

            float d0 = volume[idx];
            float3 p0 = volume.IndexToWorld(idx);

            if (x + 1 < volume.size.x)
            {
                int3 nextIdx = idx + int3(1, 0, 0);
                float d1 = volume[nextIdx];

                if (IsSignChanged(d0, d1))
                {
                    float3 p1 = volume.IndexToWorld(nextIdx);
                    ProcessEdge(idx, 0, p0, p1, d0, d1);
                }
            }

            if (y + 1 < volume.size.y)
            {
                int3 nextIdx = idx + int3(0, 1, 0);
                float d1 = volume[nextIdx];

                if (IsSignChanged(d0, d1))
                {
                    float3 p1 = volume.IndexToWorld(nextIdx);
                    ProcessEdge(idx, 1, p0, p1, d0, d1);
                }
            }

            if (z + 1 < volume.size.z)
            {
                int3 nextIdx = idx + int3(0, 0, 1);
                float d1 = volume[nextIdx];

                if (IsSignChanged(d0, d1))
                {
                    float3 p1 = volume.IndexToWorld(nextIdx);
                    ProcessEdge(idx, 2, p0, p1, d0, d1);
                }
            }
        }

        private bool IsSignChanged(float a, float b)
        {
            return (a > 0 && b <= 0) || (a <= 0 && b > 0);
        }

        private void ProcessEdge(int3 idx, int axis, float3 p0, float3 p1, float d0, float d1)
        {
            float3 intersectionPos = SDFMath.Interpolate(p0, p1, d0, d1);

            int3 closerIdx = abs(d0) < abs(d1) ? idx : (idx + (axis == 0 ? int3(1, 0, 0) : axis == 1 ? int3(0, 1, 0) : int3(0, 0, 1)));
            float3 normal = SDFMath.CalculateNormal(ref volume, closerIdx);

            HermiteData data = new HermiteData
            {
                position = intersectionPos,
                normal = normal
            };

            int edgeKey = DCEdgeHelper.CompressEdgeIndex(idx, axis);
            hermiteEdges.TryAdd(edgeKey, data);
        }
    }
}