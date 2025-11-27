using Unity.Mathematics;
using static Unity.Mathematics.math;

namespace Underforge
{
    public static class SDFMath
    {
        public static float3 Interpolate(float3 p1, float3 p2, float v1, float v2)
        {
            float t = (abs(v1 - v2) > 0.00001f) ? (-v1 / (v2 - v1)) : 0.5f;
            return lerp(p1, p2, t);
        }

        public static float3 CalculatenormalFromGrid(ref SDFVolume volume, int3 idx)
        {
            float dx = volume[idx + int3(1, 0, 0)] - volume[idx - int3(1, 0, 0)];
            float dy = volume[idx + int3(0, 1, 0)] - volume[idx - int3(0, 1, 0)];
            float dz = volume[idx + int3(0, 0, 1)] - volume[idx - int3(0, 0, 1)];

            return normalize(float3(dx, dy, dz));
        }
    }
}