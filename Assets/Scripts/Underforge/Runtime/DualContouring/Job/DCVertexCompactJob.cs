using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

namespace Underforge
{
    [BurstCompile]
    public struct DCVertexCompactJob : IJob
    {
        public SDFVolume volume;

        [ReadOnly] public NativeParallelHashMap<int, DCVertex> srcVertices;

        [WriteOnly] public NativeList<float3> outPositions;
        [WriteOnly] public NativeList<float3> outNormals;

        [WriteOnly] public NativeParallelHashMap<int, int> voxelToMeshIndex;

        public void Execute()
        {
            int maxIndex = volume.size.x * volume.size.y * volume.size.z;
            int currentMeshIndex = 0;

            for (int i = 0; i < maxIndex; i++)
            {
                if (srcVertices.TryGetValue(i, out DCVertex v))
                {
                    outPositions.Add(v.position);
                    outNormals.Add(v.normal);

                    voxelToMeshIndex.TryAdd(i, currentMeshIndex);

                    currentMeshIndex++;
                }
            }
        }
    }
}