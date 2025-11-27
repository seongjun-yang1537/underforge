using Corelib.SUI;
using UnityEngine.Events;

namespace Corelib.Utils
{
    public static class IntRangeeInspector
    {
        static bool foldItemModelData;

        public static SUIElement Render(string prefix, IntRange range)
        {
            if (range == null) return SUIElement.Empty();

            return SEditorGUILayout.Horizontal()
            .LabelWidth(60f)
            .Content(
                SEditorGUILayout.Label(prefix)
                + SEditorGUILayout.Int("", range.Min)
                .OnValueChanged(value => range.Min = value)
                + SEditorGUILayout.Label("~")
                .Width(8)
                + SEditorGUILayout.Int("", range.Max)
                .OnValueChanged(value => range.Max = value)
            );
        }
    }
}