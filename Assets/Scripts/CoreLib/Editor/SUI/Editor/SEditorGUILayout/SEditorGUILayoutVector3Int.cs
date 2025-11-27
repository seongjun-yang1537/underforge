using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace Corelib.SUI
{
    public class SEditorGUILayoutVector3Int : SUIElement
    {
        private string prefix;
        private Vector3Int value;
        private string tooltip;
        private UnityAction<Vector3Int> onValueChanged;

        public SEditorGUILayoutVector3Int(string prefix, Vector3Int value)
        {
            this.prefix = prefix;
            this.value = value;
        }

        public SEditorGUILayoutVector3Int OnValueChanged(UnityAction<Vector3Int> onValueChanged)
        {
            this.onValueChanged = onValueChanged;
            return this;
        }

        public SEditorGUILayoutVector3Int Tooltip(string tooltip)
        {
            this.tooltip = tooltip;
            return this;
        }

        public override void Render()
        {
            GUIContent content = tooltip != null ? new GUIContent(prefix, tooltip) : new GUIContent(prefix);
            Vector3Int newValue = EditorGUILayout.Vector3IntField(content, value);
            if (newValue != value)
            {
                value = newValue;
                onValueChanged?.Invoke(newValue);
            }
        }
    }
}