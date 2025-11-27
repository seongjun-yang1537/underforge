using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace Corelib.SUI
{
    public class SEditorGUILayoutFoldGroup : SUIElement, IWidthable<SEditorGUILayoutFoldGroup>
    {
        private string label;
        private bool value;
        private SUIElement content;
        private SUIElement headerRight;
        private UnityAction<bool> onValueChanged;
        private float? elementWidth;

        public SEditorGUILayoutFoldGroup(string label, bool value)
        {
            this.label = label;
            this.value = value;
        }

        public SEditorGUILayoutFoldGroup OnValueChanged(UnityAction<bool> onValueChanged)
        {
            this.onValueChanged = onValueChanged;
            return this;
        }

        public SEditorGUILayoutFoldGroup Content(SUIElement content = null)
        {
            this.content = content;
            return this;
        }

        public SEditorGUILayoutFoldGroup HeaderRight(SUIElement headerRight)
        {
            this.headerRight = headerRight;
            return this;
        }

        public SEditorGUILayoutFoldGroup Width(float width)
        {
            elementWidth = width;
            return this;
        }

        public override void Render()
        {
            var containerElement = SEditorGUILayout.Vertical();
            if (elementWidth != null) containerElement = containerElement.Width(elementWidth.Value);
            containerElement.Content(
                SEditorGUILayout.Vertical("helpbox")
                .Content(
                    SEditorGUILayout.Horizontal()
                    .Content(
                        SGUILayout.Space(15)
                        + SEditorGUILayout.FoldoutHeaderGroup(label, value)
                        .OnValueChanged(changed =>
                        {
                            value = changed;
                            onValueChanged?.Invoke(changed);
                        })
                        + (headerRight ?? SUIElement.Empty())
                    )
                )
                + SEditorGUILayout.Vertical("helpbox")
                .Content(
                    SEditorGUILayout.Action(() =>
                    {
                        if (value)
                            content?.Render();
                    })
                )
            ).Render();
        }
    }
}