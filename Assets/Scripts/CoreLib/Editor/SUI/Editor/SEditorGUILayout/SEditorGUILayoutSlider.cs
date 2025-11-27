using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace Corelib.SUI
{
    public class SEditorGUILayoutSlider : SUIElement
    {
        private string preifx;
        private float value;
        private float minValue;
        private float maxValue;
        private float? width;

        private UnityAction<float> onValueChanged;

        public SEditorGUILayoutSlider(string preifx, float value, float minValue, float maxValue)
        {
            this.preifx = preifx;
            this.value = value;
            this.minValue = minValue;
            this.maxValue = maxValue;
        }

        public SEditorGUILayoutSlider OnValueChanged(UnityAction<float> onValueChanged)
        {
            this.onValueChanged = onValueChanged;
            return this;
        }

        public SEditorGUILayoutSlider Width(float width)
        {
            this.width = width;
            return this;
        }

        public override void Render()
        {
            var options = new List<GUILayoutOption>();

            if (width != null)
            {
                options.Add(GUILayout.Width(width.Value));
            }

            float newValue = EditorGUILayout.Slider(preifx, value, minValue, maxValue, options.ToArray());
            if (newValue != value)
            {
                value = newValue;
                onValueChanged?.Invoke(newValue);
            }
        }
    }
}