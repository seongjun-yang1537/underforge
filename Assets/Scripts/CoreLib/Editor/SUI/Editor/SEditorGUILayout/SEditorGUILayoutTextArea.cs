using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace Corelib.SUI
{
    public class SEditorGUILayoutTextArea : SUIElement
    {
        private string fieldLabel;
        private string fieldValue;
        private float? fieldWidth;
        private float? fieldHeight;
        private UnityAction<string> valueChangedCallback;

        public SEditorGUILayoutTextArea(string label, string value)
        {
            fieldLabel = label;
            fieldValue = value ?? string.Empty;
        }

        public SEditorGUILayoutTextArea OnValueChanged(UnityAction<string> onValueChanged)
        {
            valueChangedCallback = onValueChanged;
            return this;
        }

        public SEditorGUILayoutTextArea Width(float width)
        {
            fieldWidth = width;
            return this;
        }

        public SEditorGUILayoutTextArea Height(float height)
        {
            fieldHeight = height;
            return this;
        }

        public override void Render()
        {
            List<GUILayoutOption> layoutOptions = new();
            if (fieldWidth != null) layoutOptions.Add(GUILayout.Width(fieldWidth.Value));
            if (fieldHeight != null) layoutOptions.Add(GUILayout.Height(fieldHeight.Value));

            EditorGUILayout.BeginVertical();
            if (!string.IsNullOrEmpty(fieldLabel))
            {
                EditorGUILayout.LabelField(fieldLabel);
            }
            string newValue = EditorGUILayout.TextArea(fieldValue, layoutOptions.ToArray());
            EditorGUILayout.EndVertical();

            if (newValue != fieldValue)
            {
                fieldValue = newValue;
                valueChangedCallback?.Invoke(newValue);
            }
        }
    }
}
