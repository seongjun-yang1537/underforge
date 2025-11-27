using UnityEngine;

namespace Corelib.Utils
{
    public static class CTriangleUtils
    {
        public static Vector3 ClosestPointOnTriangle(this CTriangle triangle, Vector3 point)
        {
            Vector3 a = triangle.v0;
            Vector3 b = triangle.v1;
            Vector3 c = triangle.v2;
            Vector3 ab = b - a;
            Vector3 ac = c - a;
            Vector3 normal = Vector3.Cross(ab, ac);
            Vector3 projected = point - Vector3.Dot(point - a, normal) / normal.sqrMagnitude * normal;
            if (triangle.IsPointInTriangleOnSamePlane(projected)) return projected;
            Vector3 pAb = ClosestPointOnLineSegment(a, b, point);
            Vector3 pBc = ClosestPointOnLineSegment(b, c, point);
            Vector3 pCa = ClosestPointOnLineSegment(c, a, point);
            float dAb = (point - pAb).sqrMagnitude;
            float dBc = (point - pBc).sqrMagnitude;
            float dCa = (point - pCa).sqrMagnitude;
            float minD = Mathf.Min(dAb, dBc, dCa);
            if (minD == dAb) return pAb;
            if (minD == dBc) return pBc;
            return pCa;
        }

        private static Vector3 ClosestPointOnLineSegment(Vector3 start, Vector3 end, Vector3 point)
        {
            Vector3 segment = end - start;
            float t = Vector3.Dot(point - start, segment) / segment.sqrMagnitude;
            t = Mathf.Clamp01(t);
            return start + segment * t;
        }

        private static bool IsPointInTriangleOnSamePlane(this CTriangle triangle, Vector3 point)
        {
            Vector3 a = triangle.v0;
            Vector3 b = triangle.v1;
            Vector3 c = triangle.v2;
            Vector3 ab = b - a;
            Vector3 ac = c - a;
            Vector3 ap = point - a;
            float dotAbAp = Vector3.Dot(ab, ap);
            float dotAbAb = Vector3.Dot(ab, ab);
            float dotAcAp = Vector3.Dot(ac, ap);
            float dotAcAc = Vector3.Dot(ac, ac);
            float dotAbAc = Vector3.Dot(ab, ac);
            float invDenom = 1 / (dotAbAb * dotAcAc - dotAbAc * dotAbAc);
            float u = (dotAcAc * dotAbAp - dotAbAc * dotAcAp) * invDenom;
            float v = (dotAbAb * dotAcAp - dotAbAc * dotAbAp) * invDenom;
            return u >= 0 && v >= 0 && u + v <= 1;
        }

        public static bool IsPointInTriangle(Vector3 point, CTriangle triangle)
        {
            Vector3 a = triangle.v0;
            Vector3 b = triangle.v1;
            Vector3 c = triangle.v2;
            Vector3 ab = b - a;
            Vector3 ac = c - a;
            Vector3 ap = point - a;
            float dotAbAp = Vector3.Dot(ab, ap);
            float dotAbAb = Vector3.Dot(ab, ab);
            float dotAcAp = Vector3.Dot(ac, ap);
            float dotAcAc = Vector3.Dot(ac, ac);
            float dotAbAc = Vector3.Dot(ab, ac);
            float invDenom = 1 / (dotAbAb * dotAcAc - dotAbAc * dotAbAc);
            float u = (dotAcAc * dotAbAp - dotAbAc * dotAcAp) * invDenom;
            float v = (dotAbAb * dotAcAp - dotAbAc * dotAbAp) * invDenom;
            return u >= 0 && v >= 0 && u + v < 1;
        }

        public static bool RayIntersectsTriangle(Ray ray, CTriangle triangle, out float distance)
        {
            const float epsilon = 1e-8f;
            distance = 0f;
            Vector3 v0 = triangle.v0;
            Vector3 v1 = triangle.v1;
            Vector3 v2 = triangle.v2;
            Vector3 edge1 = v1 - v0;
            Vector3 edge2 = v2 - v0;
            Vector3 h = Vector3.Cross(ray.direction, edge2);
            float a = Vector3.Dot(edge1, h);
            if (a > -epsilon && a < epsilon) return false;
            float f = 1f / a;
            Vector3 s = ray.origin - v0;
            float u = f * Vector3.Dot(s, h);
            if (u < 0f || u > 1f) return false;
            Vector3 q = Vector3.Cross(s, edge1);
            float v = f * Vector3.Dot(ray.direction, q);
            if (v < 0f || u + v > 1f) return false;
            float t = f * Vector3.Dot(edge2, q);
            if (t > epsilon)
            {
                distance = t;
                return true;
            }
            return false;
        }

        public static Vector3 GetRandomPointInside(this CTriangle triangle, MT19937 rng = null)
        {
            rng ??= MT19937.Create();
            float r1 = rng.NextFloat();
            float r2 = rng.NextFloat();
            if (r1 + r2 > 1f)
            {
                r1 = 1f - r1;
                r2 = 1f - r2;
            }
            Vector3 edge1 = triangle.v1 - triangle.v0;
            Vector3 edge2 = triangle.v2 - triangle.v0;
            return triangle.v0 + r1 * edge1 + r2 * edge2;
        }
    }
}
