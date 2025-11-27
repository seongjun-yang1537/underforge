using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace Corelib.SUI
{
    public class SEditorGUILayoutLabel : SUIElement
    {
        private string label;
        private float? width;
        private bool isBold = false;
        private TextAnchor? textAnchor;
        private Color? color;
        private UnityAction<int> onValueChanged;
        private TextClipping? textClipping;
        private static readonly GUIStyle guiStyle = new GUIStyle(EditorStyles.label);

        public SEditorGUILayoutLabel(string label)
        {
            this.label = label;
        }

        public SEditorGUILayoutLabel Width(float width)
        {
            this.width = width;
            return this;
        }

        public SEditorGUILayoutLabel Bold()
        {
            this.isBold = true;
            return this;
        }

        public SEditorGUILayoutLabel Align(TextAnchor textAnchor)
        {
            this.textAnchor = textAnchor;
            return this;
        }

        public SEditorGUILayoutLabel Clip(TextClipping textClipping)
        {
            this.textClipping = textClipping;
            return this;
        }

        public SEditorGUILayoutLabel Color(Color color)
        {
            this.color = color;
            return this;
        }

        public override void Render()
        {
            List<GUILayoutOption> options = new();
            if (width != null) options.Add(GUILayout.Width(width.Value));

            var originalFontStyle = guiStyle.fontStyle;
            var originalAlignment = guiStyle.alignment;
            var originalColor = guiStyle.normal.textColor;
            var originalClipping = guiStyle.clipping;
            if (isBold) guiStyle.fontStyle = FontStyle.Bold;
            if (textAnchor != null) guiStyle.alignment = textAnchor.Value;
            if (color != null) guiStyle.normal.textColor = color.Value;
            if (textClipping != null) guiStyle.clipping = textClipping.Value;
            EditorGUILayout.LabelField(label, guiStyle, options.ToArray());
            guiStyle.fontStyle = originalFontStyle;
            guiStyle.alignment = originalAlignment;
            guiStyle.normal.textColor = originalColor;
            guiStyle.clipping = originalClipping;
        }
    }
}
