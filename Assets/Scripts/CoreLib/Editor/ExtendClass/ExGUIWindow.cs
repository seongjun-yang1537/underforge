using UnityEditor;
using UnityEngine;

namespace Corelib.Utils
{
    public static class ExGUIWindow
    {
        static GUIStyle roundedStyle;
        static int cachedRadius = -1;

        static void EnsureRoundedStyle(int radius)
        {
            if (roundedStyle != null && cachedRadius == radius) return;

            string path = EditorGUIUtility.isProSkin
                ? "builtin skins/darkskin/images/node5.png"
                : "builtin skins/lightskin/images/node5.png";

            var tex = (Texture2D)EditorGUIUtility.Load(path);
            if (tex == null)
            {
                path = EditorGUIUtility.isProSkin
                    ? "builtin skins/darkskin/images/node4.png"
                    : "builtin skins/lightskin/images/node4.png";
                tex = (Texture2D)EditorGUIUtility.Load(path);
            }

            roundedStyle = new GUIStyle(GUIStyle.none)
            {
                normal = { background = tex },
                border = new RectOffset(12, 12, 12, 12),
                padding = new RectOffset(0, 0, 0, 0),
                margin = new RectOffset(0, 0, 0, 0),
            };
            cachedRadius = radius;
        }

        public static void DrawRoundedRect(Rect r, float radius, Color fill, Color outline)
        {
            EnsureRoundedStyle(Mathf.RoundToInt(radius));

            var prev = GUI.color;
            GUI.color = new Color(fill.r, fill.g, fill.b, Mathf.Clamp01(fill.a));
            GUI.Box(r, GUIContent.none, roundedStyle);
            GUI.color = prev;

            if (outline.a > 0f)
            {
                EditorGUI.DrawRect(new Rect(r.x, r.y, r.width, 1f), outline);
                EditorGUI.DrawRect(new Rect(r.x, r.yMax - 1f, r.width, 1f), outline);
                EditorGUI.DrawRect(new Rect(r.x, r.y, 1f, r.height), outline);
                EditorGUI.DrawRect(new Rect(r.xMax - 1f, r.y, 1f, r.height), outline);
            }
        }

        public static void DrawRoundedRectFill(Rect r, float radius, Color fill)
        {
            EnsureRoundedStyle(Mathf.RoundToInt(radius));

            var prev = GUI.color;
            GUI.color = new Color(fill.r, fill.g, fill.b, Mathf.Clamp01(fill.a));
            GUI.Box(r, GUIContent.none, roundedStyle);
            GUI.color = prev;
        }

        public static void ClearCache()
        {
            roundedStyle = null;
            cachedRadius = -1;
        }
    }
}
