using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UObject = UnityEngine.Object;

namespace Corelib.SUI
{
    public class SEditorGUILayoutObject : SUIElement
    {
        private string label;
        private UObject value;
        private Type type;
        private UnityAction<UObject> onValueChanged;

        private float? width;

        public SEditorGUILayoutObject(string label, UObject value, Type type)
        {
            this.label = label;
            this.value = value;
            this.type = type;
        }

        public SEditorGUILayoutObject OnValueChanged(UnityAction<UObject> onValueChanged)
        {
            this.onValueChanged = onValueChanged;
            return this;
        }

        public SEditorGUILayoutObject Width(float width)
        {
            this.width = width;
            return this;
        }


        public override void Render()
        {
            List<GUILayoutOption> guiOptions = new();
            if (width != null) guiOptions.Add(GUILayout.Width(width.Value));

            GUIContent content = string.IsNullOrEmpty(label) ? GUIContent.none : new GUIContent(label);
            UObject newValue = EditorGUILayout.ObjectField(content, value, type, true, guiOptions.ToArray());
            if (newValue != value)
            {
                value = newValue;
                onValueChanged?.Invoke(newValue);
            }
        }
    }
}