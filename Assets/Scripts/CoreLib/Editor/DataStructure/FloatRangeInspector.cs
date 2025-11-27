using Corelib.SUI;
using UnityEngine.Events;

namespace Corelib.Utils
{
    public static class FloatRangeInspector
    {
        public static SUIElement Render(string prefix, FloatRange range)
        {
            if (range == null) return SUIElement.Empty();

            return SEditorGUILayout.Horizontal()
                .LabelWidth(60f)
                .Content(
                    SEditorGUILayout.Label(prefix)
                    + SEditorGUILayout.Float(string.Empty, range.Min)
                        .OnValueChanged(value => range.Min = value)
                    + SEditorGUILayout.Label("~")
                        .Width(8)
                    + SEditorGUILayout.Float(string.Empty, range.Max)
                        .OnValueChanged(value => range.Max = value)
                );
        }

        public static SUIElement RenderGroup(string prefix, FloatRange range, bool fold, UnityAction<bool> setter)
        {
            if (range == null) return SUIElement.Empty();

            return SEditorGUILayout.Vertical()
                .Content(
                    SEditorGUILayout.FoldGroup(prefix, fold)
                        .OnValueChanged(setter)
                        .Content(
                            Render(prefix, range)
                        )
                );
        }
    }
}
