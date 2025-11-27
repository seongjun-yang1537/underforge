using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace Corelib.SUI
{
    public class SEditorGUILayoutVertical : SUIElement
    {
        private SUIElement content;
        private string style = "";
        private GUIStyle guiStyle;
        private Func<bool> where = () => true;
        private float? elementWidth;

        public SEditorGUILayoutVertical()
        {

        }

        public SEditorGUILayoutVertical(string style)
        {
            this.style = style;
        }

        public SEditorGUILayoutVertical(GUIStyle guiStyle)
        {
            this.guiStyle = guiStyle;
        }

        public SEditorGUILayoutVertical Content(SUIElement content = null)
        {
            this.content = content;
            return this;
        }

        public SEditorGUILayoutVertical Style(string style)
        {
            this.style = style;
            return this;
        }

        public SEditorGUILayoutVertical Where(Func<bool> callback)
        {
            this.where = callback;
            return this;
        }

        public SEditorGUILayoutVertical Width(float width)
        {
            elementWidth = width;
            return this;
        }

        public override void Render()
        {
            var options = new List<GUILayoutOption>();
            if (elementWidth != null) options.Add(GUILayout.Width(elementWidth.Value));
            if (guiStyle != null)
                EditorGUILayout.BeginVertical(guiStyle, options.ToArray());
            else
                EditorGUILayout.BeginVertical(string.IsNullOrWhiteSpace(style) ? GUIStyle.none : style, options.ToArray());

            if (where())
                content?.Render();
            EditorGUILayout.EndVertical();
        }
    }
}