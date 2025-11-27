using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Corelib.SUI
{
    public class SEditorGUILayoutToolbar : SUIElement
    {
        private string[] labels;
        private GUIContent[] contents;
        private int selected;
        private UnityAction<int> onValueChanged;

        private float? width;
        private float? height;

        public SEditorGUILayoutToolbar(int selected, params string[] labels)
        {
            this.selected = selected;
            this.labels = labels;
        }

        public SEditorGUILayoutToolbar(int selected, params GUIContent[] contents)
        {
            this.selected = selected;
            this.contents = contents;
        }

        public SEditorGUILayoutToolbar OnValueChanged(UnityAction<int> onValueChanged)
        {
            this.onValueChanged = onValueChanged;
            return this;
        }

        public SEditorGUILayoutToolbar Width(float width)
        {
            this.width = width;
            return this;
        }

        public SEditorGUILayoutToolbar Height(float height)
        {
            this.height = height;
            return this;
        }

        public override void Render()
        {
            var options = new List<GUILayoutOption>();
            if (width.HasValue)
                options.Add(GUILayout.Width(width.Value));
            if (height.HasValue)
                options.Add(GUILayout.Height(height.Value));

            int newIndex = selected;

            if (contents != null && contents.Length > 0)
                newIndex = GUILayout.Toolbar(selected, contents, options.ToArray());
            else if (labels != null && labels.Length > 0)
                newIndex = GUILayout.Toolbar(selected, labels, options.ToArray());

            if (newIndex != selected)
            {
                selected = newIndex;
                onValueChanged?.Invoke(newIndex);
            }
        }
    }
}
