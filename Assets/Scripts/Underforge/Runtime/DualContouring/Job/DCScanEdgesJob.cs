using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using static Unity.Mathematics.math;

namespace Underforge
{
    [BurstCompile]
    public struct DCScanEdgesJob : IJobParallelFor
    {
        [ReadOnly] public SDFVolume volume;

        // [WriteOnly] public 

        public void Execute(int index)
        {
        }
    }
}