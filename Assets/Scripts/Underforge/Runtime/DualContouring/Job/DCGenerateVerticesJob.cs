using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using static Unity.Mathematics.math;

namespace Underforge
{
    [BurstCompile]
    public class DCGenerateVerticesJob : IJobParallelFor
    {
        [ReadOnly] public SDFVolume volume;
        [ReadOnly] public NativeParallelHashMap<int, DCHermiteData> hermiteEdges;
        [WriteOnly] public NativeParallelHashMap<int, DCVertex>.ParallelWriter vertices;

        public void Execute(int index)
        {
            int strideXY = volume.size.x * volume.size.y;
            int z = index / strideXY;
            int rem = index % strideXY;
            int y = rem / volume.size.x;
            int x = rem % volume.size.x;
            int3 idx = new int3(x, y, z);

            if (x + 1 >= volume.size.x || y + 1 >= volume.size.y || z + 1 >= volume.size.z)
                return;

            float3 posSum = new float3(0f, 0f, 0f);
            float3 normalSum = new float3(0f, 0f, 0f);
            int count = 0;

            CollectEdge(idx, 0, ref posSum, ref normalSum, ref count);
            CollectEdge(idx + int3(0, 1, 0), 0, ref posSum, ref normalSum, ref count);
            CollectEdge(idx + int3(0, 0, 1), 0, ref posSum, ref normalSum, ref count);
            CollectEdge(idx + int3(0, 1, 1), 0, ref posSum, ref normalSum, ref count);

            CollectEdge(idx, 1, ref posSum, ref normalSum, ref count);
            CollectEdge(idx + int3(1, 0, 0), 1, ref posSum, ref normalSum, ref count);
            CollectEdge(idx + int3(0, 0, 1), 1, ref posSum, ref normalSum, ref count);
            CollectEdge(idx + int3(1, 0, 1), 1, ref posSum, ref normalSum, ref count);

            CollectEdge(idx, 2, ref posSum, ref normalSum, ref count);
            CollectEdge(idx + int3(1, 0, 0), 2, ref posSum, ref normalSum, ref count);
            CollectEdge(idx + int3(0, 1, 0), 2, ref posSum, ref normalSum, ref count);
            CollectEdge(idx + int3(1, 1, 0), 2, ref posSum, ref normalSum, ref count);


            if (count > 0)
            {
                float3 avgPos = posSum / count;
                float3 avgNormal = normalize(normalSum / count);

                DCVertex v = new DCVertex
                {
                    position = avgPos,
                    normal = avgNormal
                };

                vertices.TryAdd(index, v);
            }
        }

        private void CollectEdge(int3 edgeIdx, int axis, ref float3 pSum, ref float3 nSum, ref int count)
        {
            if (!volume.Contains(edgeIdx)) return;

            int key = DCEdgeHelper.CompressEdgeIndex(edgeIdx, axis);

            if (hermiteEdges.TryGetValue(key, out DCHermiteData data))
            {
                pSum += data.position;
                nSum += data.normal;
                count++;
            }
        }
    }
}