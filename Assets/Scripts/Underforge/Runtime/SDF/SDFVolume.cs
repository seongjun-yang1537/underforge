using System;
using Unity.Collections;
using Unity.Mathematics;
using static Unity.Mathematics.math;

namespace Underforge
{
    public struct SDFVolume : IDisposable
    {
        public float3 origin;
        public int3 size;
        public NativeArray<float> densities;

        public SDFVolume(float3 center, int3 size, Allocator allocator)
        {
            this.size = size;
            this.origin = center - ((float3)size * 0.5f);

            int count = size.x * size.y * size.z;
            this.densities = new NativeArray<float>(count, allocator);
        }

        public float3 IndexToWorld(int3 idx) => origin + (float3)idx;
        public int3 WorldToIndex(float3 pos) => (int3)floor(pos - origin);
        public bool Contains(int3 idx) => all(idx >= 0) && all(idx < size);

        public int FlattenIndex(int3 idx) => (idx.z * size.x * size.y) + (idx.y * size.x) + idx.x;

        public float this[int3 idx]
        {
            get => Contains(idx) ? densities[FlattenIndex(idx)] : float.MaxValue;
            set { if (Contains(idx)) densities[FlattenIndex(idx)] = value; }
        }

        public void Dispose()
        {
            if (densities.IsCreated) densities.Dispose();
        }
    }
}