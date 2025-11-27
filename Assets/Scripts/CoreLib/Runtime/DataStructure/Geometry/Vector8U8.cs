using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Corelib.Utils
{
    [Serializable]
    [StructLayout(LayoutKind.Explicit)]
    public struct Vector8U8
    {
        [FieldOffset(0)] public uint packedA;
        [FieldOffset(4)] public uint packedB;
        [FieldOffset(0)] public byte x;
        [FieldOffset(1)] public byte y;
        [FieldOffset(2)] public byte z;
        [FieldOffset(3)] public byte w;
        [FieldOffset(4)] public byte u;
        [FieldOffset(5)] public byte v;
        [FieldOffset(6)] public byte s;
        [FieldOffset(7)] public byte t;

        public Vector8U8(float x, float y, float z, float w, float u, float v, float s, float t)
        {
            packedA = 0;
            packedB = 0;
            this.x = Quantize(x);
            this.y = Quantize(y);
            this.z = Quantize(z);
            this.w = Quantize(w);
            this.u = Quantize(u);
            this.v = Quantize(v);
            this.s = Quantize(s);
            this.t = Quantize(t);
        }

        private static byte Quantize(float value)
        {
            return (byte)Mathf.Clamp(Mathf.RoundToInt(value * 255f), 0, 255);
        }

        private static float Dequantize(byte value)
        {
            return value / 255f;
        }

        public float this[int index]
        {
            get
            {
                switch (index)
                {
                    case 0: return Dequantize(x);
                    case 1: return Dequantize(y);
                    case 2: return Dequantize(z);
                    case 3: return Dequantize(w);
                    case 4: return Dequantize(u);
                    case 5: return Dequantize(v);
                    case 6: return Dequantize(s);
                    case 7: return Dequantize(t);
                    default: throw new IndexOutOfRangeException();
                }
            }
            set
            {
                switch (index)
                {
                    case 0: x = Quantize(value); break;
                    case 1: y = Quantize(value); break;
                    case 2: z = Quantize(value); break;
                    case 3: w = Quantize(value); break;
                    case 4: u = Quantize(value); break;
                    case 5: v = Quantize(value); break;
                    case 6: s = Quantize(value); break;
                    case 7: t = Quantize(value); break;
                    default: throw new IndexOutOfRangeException();
                }
            }
        }

        public override string ToString()
        {
            return $"({Dequantize(x):F3}, {Dequantize(y):F3}, {Dequantize(z):F3}, {Dequantize(w):F3}, " +
                   $"{Dequantize(u):F3}, {Dequantize(v):F3}, {Dequantize(s):F3}, {Dequantize(t):F3})";
        }

        public static Vector8U8 zero => new Vector8U8(0f, 0f, 0f, 0f, 0f, 0f, 0f, 0f);
        public static Vector8U8 one => new Vector8U8(1f, 1f, 1f, 1f, 1f, 1f, 1f, 1f);
    }
}
