using Unity.Mathematics;

namespace Underforge
{
    public static class DCEdgeHelper
    {
        public static int CompressEdgeIndex(int3 coord, int axis)
        {
            return (axis << 30) | (coord.z << 20) | (coord.y << 10) | coord.x;
        }

        public static void DecompressEdgeIndex(int compressed, out int3 coord, out int axis)
        {
            axis = compressed >> 30;
            int x = compressed & 0x3FF;
            int y = (compressed >> 10) & 0x3FF;
            int z = (compressed >> 20) & 0x3FF;
            coord = new int3(x, y, z);
        }
    }
}