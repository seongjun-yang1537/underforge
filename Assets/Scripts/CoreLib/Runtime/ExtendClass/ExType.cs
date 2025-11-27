using System;
using System.Collections.Generic;

namespace Corelib.Utils
{
    public static class ExType
    {
        public static int GetEnumLength(this Type enumType)
            => Enum.GetValues(enumType).Length;

        public static Type LCAType(Type typeA, Type typeB)
        {
            var ancestorsOfA = new HashSet<Type>();
            Type currentType = typeA;
            while (currentType != null)
            {
                ancestorsOfA.Add(currentType);
                currentType = currentType.BaseType;
            }

            currentType = typeB;
            while (currentType != null)
            {
                if (ancestorsOfA.Contains(currentType))
                {
                    return currentType;
                }
                currentType = currentType.BaseType;
            }

            return typeof(object);
        }
    }
}