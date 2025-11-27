using UnityEngine;

namespace Corelib.Utils
{
    public static class ExRect
    {
        public static Rect ClampTo(this Rect rect, Rect bounds)
        {
            rect.x = Mathf.Clamp(rect.x, 0, Mathf.Max(0, bounds.width - rect.width));
            rect.y = Mathf.Clamp(rect.y, 0, Mathf.Max(0, bounds.height - rect.height));
            rect.width = Mathf.Clamp(rect.width, 0, Mathf.Max(0, bounds.width));
            rect.height = Mathf.Clamp(rect.height, 0, Mathf.Max(0, bounds.height));
            return rect;
        }
    }
}
