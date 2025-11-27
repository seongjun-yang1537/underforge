using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace Corelib.SUI
{
    public class SEditorGUILayoutVector3 : SUIElement
    {
        private string prefix;
        private Vector3 value;
        private string tooltip;
        private UnityAction<Vector3> onValueChanged;

        public SEditorGUILayoutVector3(string prefix, Vector3 value)
        {
            this.prefix = prefix;
            this.value = value;
        }

        public SEditorGUILayoutVector3 OnValueChanged(UnityAction<Vector3> onValueChanged)
        {
            this.onValueChanged = onValueChanged;
            return this;
        }

        public SEditorGUILayoutVector3 Tooltip(string tooltip)
        {
            this.tooltip = tooltip;
            return this;
        }

        public override void Render()
        {
            GUIContent content = tooltip != null ? new GUIContent(prefix, tooltip) : new GUIContent(prefix);
            Vector3 newValue = EditorGUILayout.Vector3Field(content, value);
            if (newValue != value)
            {
                value = newValue;
                onValueChanged?.Invoke(newValue);
            }
        }
    }
}