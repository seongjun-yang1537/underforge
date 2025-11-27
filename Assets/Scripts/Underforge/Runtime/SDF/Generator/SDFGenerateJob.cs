using System;
using Unity.Burst;
using Unity.Jobs;
using Unity.Mathematics;
using static Unity.Mathematics.math;

namespace Underforge
{
    [BurstCompile]
    public struct SDFGenerateJob : IJobParallelFor
    {
        public SDFVolume volume;

        public SDFGenerateType type;
        public float offset;
        public float noiseScale;
        public float noiseAmp;
        public float2 noiseOffset;

        public void Execute(int index)
        {
            int strideXY = volume.size.x * volume.size.y;
            int z = index / strideXY;
            int rem = index % strideXY;
            int y = rem / volume.size.x;
            int x = rem % volume.size.x;

            int3 idx = int3(x, y, z);
            float3 pos = volume.IndexToWorld(idx);

            float density = 0f;

            switch (type)
            {
                case SDFGenerateType.Empty:
                    density = 100.0f;
                    break;

                case SDFGenerateType.Full:
                    density = -100.0f;
                    break;

                case SDFGenerateType.Plane:
                    density = pos.y - offset;
                    break;

                case SDFGenerateType.Perlin:
                    float noiseVal = noise.cnoise((pos.xz + noiseOffset) * noiseScale);
                    float terrainHeight = offset + (noiseVal * noiseAmp);

                    density = pos.y - terrainHeight;
                    break;
            }

            volume[idx] = density;
        }
    }
}