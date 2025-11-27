using System;
using System.Collections.Generic;
using UnityEngine;

namespace Corelib.Utils
{
    [Serializable]
    public class PObb
    {
        public Vector3 center;
        public Vector3 size;
        public Quaternion rotation;

        public Vector3 Min => ToBounds().min;
        public Vector3 Max => ToBounds().max;

        public PObb(Vector3 center, Vector3 size, Quaternion rotation)
        {
            this.center = center;
            this.size = size;
            this.rotation = rotation;
        }

        Vector3[] GetAxes()
        {
            return new[]
            {
                rotation * Vector3.right,
                rotation * Vector3.up,
                rotation * Vector3.forward
            };
        }

        float GetProjectionRadius(Vector3 axis)
        {
            axis.Normalize();
            var e = size * 0.5f;
            var u = GetAxes();
            return Mathf.Abs(Vector3.Dot(u[0], axis)) * e.x + Mathf.Abs(Vector3.Dot(u[1], axis)) * e.y + Mathf.Abs(Vector3.Dot(u[2], axis)) * e.z;
        }

        public bool Contains(Vector3 point, float distance)
        {
            var local = Quaternion.Inverse(rotation) * (point - center);
            var h = size * 0.5f + Vector3.one * distance;
            return Mathf.Abs(local.x) <= h.x && Mathf.Abs(local.y) <= h.y && Mathf.Abs(local.z) <= h.z;
        }

        public bool Contains(Vector3 point)
        {
            var local = Quaternion.Inverse(rotation) * (point - center);
            var h = size * 0.5f;
            return Mathf.Abs(local.x) <= h.x && Mathf.Abs(local.y) <= h.y && Mathf.Abs(local.z) <= h.z;
        }

        public bool ContainsXZ(Vector3 point)
        {
            var local = Quaternion.Inverse(rotation) * (point - center);
            var h = size * 0.5f;
            return Mathf.Abs(local.x) <= h.x && Mathf.Abs(local.z) <= h.z;
        }

        public bool Intersects(PObb other)
        {
            var axes = new List<Vector3>();
            var aAxes = GetAxes();
            var bAxes = other.GetAxes();
            axes.AddRange(aAxes);
            axes.AddRange(bAxes);
            for (int i = 0; i < 3; i++)
                for (int j = 0; j < 3; j++)
                {
                    var axis = Vector3.Cross(aAxes[i], bAxes[j]);
                    if (axis.sqrMagnitude > 1e-6f) axes.Add(axis.normalized);
                }
            var t = other.center - center;
            foreach (var axis in axes)
            {
                float dist = Mathf.Abs(Vector3.Dot(t, axis));
                float ra = GetProjectionRadius(axis);
                float rb = other.GetProjectionRadius(axis);
                if (dist > ra + rb) return false;
            }
            return true;
        }

        public bool Intersects(Ray ray, out float distance)
        {
            var inv = Quaternion.Inverse(rotation);
            var localOrigin = inv * (ray.origin - center);
            var localDir = inv * ray.direction;
            var bounds = new Bounds(Vector3.zero, size);
            if (bounds.IntersectRay(new Ray(localOrigin, localDir), out distance)) return true;
            distance = 0f;
            return false;
        }

        public float Volume => size.x * size.y * size.z;

        public Bounds ToBounds()
        {
            var h = size * 0.5f;
            var corners = new[]
            {
                rotation * new Vector3( h.x,  h.y,  h.z),
                rotation * new Vector3(-h.x,  h.y,  h.z),
                rotation * new Vector3( h.x, -h.y,  h.z),
                rotation * new Vector3( h.x,  h.y, -h.z),
                rotation * new Vector3(-h.x, -h.y,  h.z),
                rotation * new Vector3(-h.x,  h.y, -h.z),
                rotation * new Vector3( h.x, -h.y, -h.z),
                rotation * new Vector3(-h.x, -h.y, -h.z)
            };
            var min = corners[0];
            var max = corners[0];
            for (int i = 1; i < 8; i++)
            {
                min = Vector3.Min(min, corners[i]);
                max = Vector3.Max(max, corners[i]);
            }
            min += center;
            max += center;
            return new Bounds((min + max) * 0.5f, max - min);
        }

        public static bool DoBoxesIntersect(PObb a, PObb b)
        {
            return a.Intersects(b);
        }

        public List<PObb> Subtract(PObb other)
        {
            throw new NotImplementedException();
        }

        public PObb GenerateRandomRoom(MT19937 rng, IList<RoomSpawnInfo> spawnInfos)
        {
            throw new NotImplementedException();
        }

        public List<Vector3> GetRandomPoints(int count, MT19937 rng)
        {
            if (rng == null)
                throw new ArgumentNullException(nameof(rng));
            if (count <= 0)
                return new List<Vector3>();

            var points = new List<Vector3>(count);
            var half = size * 0.5f;
            for (int i = 0; i < count; i++)
            {
                float x = rng.NextFloat(-half.x, half.x);
                float y = rng.NextFloat(-half.y, half.y);
                float z = rng.NextFloat(-half.z, half.z);
                var local = new Vector3(x, y, z);
                points.Add(center + rotation * local);
            }
            return points;
        }
    }
}
