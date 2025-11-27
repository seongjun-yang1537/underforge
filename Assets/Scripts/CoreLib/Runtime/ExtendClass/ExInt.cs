using System;

namespace Corelib.Utils
{
    public static class ExInt
    {
        public static int DivideFloor(this int value, int divisor)
        {
            if (divisor == 0) throw new DivideByZeroException();
            if (value >= 0) return value / divisor;
            int positiveValue = -value;
            return -((positiveValue + divisor - 1) / divisor);
        }

        public static int ModuloFloor(this int value, int divisor)
        {
            if (divisor == 0) throw new DivideByZeroException();
            int remainder = value % divisor;
            return remainder < 0 ? remainder + divisor : remainder;
        }
    }
}
