

using Unity.Mathematics;
using UnityEngine;

namespace Corelib.Utils
{
    public static class ExVector4
    {
        public static float3 ToFloat3(this Vector4 v) => new float3(v.x, v.y, v.z);
        public static float4 ToFloat4(this Vector4 v) => new float4(v.x, v.y, v.z, v.w);
    }
}

