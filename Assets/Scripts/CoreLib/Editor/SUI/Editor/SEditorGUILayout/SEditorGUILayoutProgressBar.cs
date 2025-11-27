using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Corelib.SUI
{
    public class SEditorGUILayoutProgressBar : SUIElement
    {
        private string progressLabel;
        private float progressValue;
        private float? elementWidth;

        public SEditorGUILayoutProgressBar(string label, float value)
        {
            progressLabel = label;
            progressValue = value;
        }

        public SEditorGUILayoutProgressBar Width(float width)
        {
            elementWidth = width;
            return this;
        }

        public override void Render()
        {
            List<GUILayoutOption> layoutOptions = new();
            if (elementWidth != null) layoutOptions.Add(GUILayout.Width(elementWidth.Value));
            Rect controlRect = EditorGUILayout.GetControlRect(layoutOptions.ToArray());
            EditorGUI.ProgressBar(controlRect, progressValue, progressLabel);
        }
    }
}

