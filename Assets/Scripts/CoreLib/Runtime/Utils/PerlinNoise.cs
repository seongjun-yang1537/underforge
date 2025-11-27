using UnityEngine;

namespace Corelib.Utils
{
    public static class PerlinNoise
    {
        private static readonly int[] p = new int[512];

        static PerlinNoise()
        {
            int[] permutation = new int[256];
            for (int i = 0; i < 256; i++)
            {
                permutation[i] = i;
            }

            var random = new System.Random(100);
            for (int i = 255; i > 0; i--)
            {
                int j = random.Next(i + 1);
                int temp = permutation[i];
                permutation[i] = permutation[j];
                permutation[j] = temp;
            }

            for (int i = 0; i < 256; i++)
            {
                p[i] = p[i + 256] = permutation[i];
            }
        }

        public static float Noise(float x, float y, float z)
        {
            int X = (int)Mathf.Floor(x) & 255;
            int Y = (int)Mathf.Floor(y) & 255;
            int Z = (int)Mathf.Floor(z) & 255;

            x -= Mathf.Floor(x);
            y -= Mathf.Floor(y);
            z -= Mathf.Floor(z);

            float u = Fade(x);
            float v = Fade(y);
            float w = Fade(z);

            int A = p[X] + Y;
            int AA = p[A] + Z;
            int AB = p[A + 1] + Z;
            int B = p[X + 1] + Y;
            int BA = p[B] + Z;
            int BB = p[B + 1] + Z;

            float res = Lerp(w, Lerp(v, Lerp(u, Grad(p[AA], x, y, z), Grad(p[BA], x - 1, y, z)),
                                         Lerp(u, Grad(p[AB], x, y - 1, z), Grad(p[BB], x - 1, y - 1, z))),
                                    Lerp(v, Lerp(u, Grad(p[AA + 1], x, y, z - 1), Grad(p[BA + 1], x - 1, y, z - 1)),
                                         Lerp(u, Grad(p[AB + 1], x, y - 1, z - 1), Grad(p[BB + 1], x - 1, y - 1, z - 1))));
            return (res + 1) / 2;
        }

        private static float Fade(float t)
        {
            return t * t * t * (t * (t * 6 - 15) + 10);
        }

        private static float Lerp(float t, float a, float b)
        {
            return a + t * (b - a);
        }

        private static float Grad(int hash, float x, float y, float z)
        {
            int h = hash & 15;
            float u = h < 8 ? x : y;
            float v = h < 4 ? y : h == 12 || h == 14 ? x : z;
            return ((h & 1) == 0 ? u : -u) + ((h & 2) == 0 ? v : -v);
        }
    }
}
