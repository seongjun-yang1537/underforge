using Unity.Mathematics;

namespace Underforge
{
    public static class DCEdgeHelper
    {
        public static int CompressEdgeIndex(int3 coord, int axis)
        {
            return (axis << 30) | (coord.z << 20) | (coord.y << 10) | coord.x;
        }
    }
}