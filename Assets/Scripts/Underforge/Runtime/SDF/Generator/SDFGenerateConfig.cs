using System;
using Unity.Mathematics;

namespace Underforge
{
    [Serializable]
    public struct SDFGenerateConfig
    {
        public SDFGenerateType type;

        public float offset;

        public float noiseScale;
        public float noiseAmplitude;
        public float2 noiseSeed;

        public static SDFGenerateConfig DefaultPerlin => new SDFGenerateConfig
        {
            type = SDFGenerateType.Perlin,
            offset = 16f,
            noiseScale = 0.05f,
            noiseAmplitude = 10f,
            noiseSeed = float2.zero
        };

        public static SDFGenerateConfig FlatPlane(float height) => new SDFGenerateConfig
        {
            type = SDFGenerateType.Plane,
            offset = height
        };
    }
}