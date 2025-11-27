using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Corelib.Utils
{
    [Serializable]
    public class CTriangle : IEnumerable<Vector3>
    {
        public Vector3 v0;
        public Vector3 v1;
        public Vector3 v2;
        public Vector3 normal;
        public List<Vector3> vertices => new() { v0, v1, v2 };
        public Vector3 center => (v0 + v1 + v2) / 3.0f;
        public float area { get; private set; }
        private static readonly Vector3EqualityComparer vertexComparer = new Vector3EqualityComparer(0.0001f);

        public CTriangle(Vector3 v0, Vector3 v1, Vector3 v2, Vector3 normal)
        {
            this.v0 = v0;
            this.v1 = v1;
            this.v2 = v2;
            this.normal = normal;
            this.area = CalculateArea(v0, v1, v2);
        }

        public CTriangle(CTriangle other)
        {
            v0 = other.v0;
            v1 = other.v1;
            v2 = other.v2;
            normal = other.normal;
            this.area = CalculateArea(v0, v1, v2);
        }

        public bool Equals(CTriangle other)
        {
            if (other is null) return false;
            if (ReferenceEquals(this, other)) return true;
            var thisVertices = new HashSet<Vector3>(new[] { v0, v1, v2 }, vertexComparer);
            var otherVertices = new HashSet<Vector3>(new[] { other.v0, other.v1, other.v2 }, vertexComparer);
            return thisVertices.SetEquals(otherVertices);
        }

        public override bool Equals(object obj)
        {
            return obj is CTriangle other && Equals(other);
        }

        public override int GetHashCode()
        {
            int h0 = vertexComparer.GetHashCode(v0);
            int h1 = vertexComparer.GetHashCode(v1);
            int h2 = vertexComparer.GetHashCode(v2);
            var sortedHashes = new[] { h0, h1, h2 }.OrderBy(h => h).ToArray();
            return (sortedHashes[0], sortedHashes[1], sortedHashes[2]).GetHashCode();
        }

        public void Deconstruct(out Vector3 a, out Vector3 b, out Vector3 c)
        {
            a = v0;
            b = v1;
            c = v2;
        }

        public IEnumerator<Vector3> GetEnumerator()
        {
            yield return v0;
            yield return v1;
            yield return v2;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override string ToString()
        {
            return $"CTriangle(\n  v0: {v0},\n  v1: {v1},\n  v2: {v2}\n)";
        }

        private static float CalculateArea(Vector3 v0, Vector3 v1, Vector3 v2)
        {
            return 0.5f * Vector3.Cross(v1 - v0, v2 - v0).magnitude;
        }
    }
}
