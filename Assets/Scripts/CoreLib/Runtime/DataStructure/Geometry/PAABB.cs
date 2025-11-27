using UnityEngine;

namespace Corelib.Utils
{
    public struct PAABB
    {
        public Vector3 Min;
        public Vector3 Max;

        public PAABB(Vector3 min, Vector3 max)
        {
            Min = min;
            Max = max;
        }

        public Vector3 Center => 0.5f * (Min + Max);
        public Vector3 Size => Max - Min;

        public Bounds ToBounds()
        {
            return new Bounds(Center, Size);
        }

        public bool Intersects(PAABB other)
        {
            return (Min.x <= other.Max.x && Max.x >= other.Min.x) &&
                   (Min.y <= other.Max.y && Max.y >= other.Min.y) &&
                   (Min.z <= other.Max.z && Max.z >= other.Min.z);
        }

        public bool Contains(Vector3 point)
        {
            return (point.x >= Min.x && point.x <= Max.x) &&
                   (point.y >= Min.y && point.y <= Max.y) &&
                   (point.z >= Min.z && point.z <= Max.z);
        }

        public override string ToString()
        {
            return $"PAABB(Min={Min}, Max={Max})";
        }
    }
}
