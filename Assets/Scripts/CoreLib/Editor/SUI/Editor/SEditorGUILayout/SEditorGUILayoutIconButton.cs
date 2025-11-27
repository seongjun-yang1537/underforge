// Generated under Codex compliance with AGENTS.md (cave1)
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace Corelib.SUI
{
    public class SEditorGUILayoutIconButton : SUIElement, IWidthable<SEditorGUILayoutIconButton>
    {
        private readonly string iconName;
        private readonly string buttonLabel;
        private readonly Vector2 buttonIconSize;
        private UnityAction onClick;
        private float? width;
        private float? height;
        private Func<bool> where = () => true;
        private bool isDisabled;

        public SEditorGUILayoutIconButton(string iconName, string buttonLabel, Vector2 iconSize)
        {
            this.iconName = iconName;
            this.buttonLabel = buttonLabel;
            buttonIconSize = iconSize;
        }

        public SEditorGUILayoutIconButton Where(Func<bool> predicate)
        {
            where = predicate;
            return this;
        }

        public SEditorGUILayoutIconButton OnClick(UnityAction clickAction)
        {
            onClick = clickAction;
            return this;
        }

        public SEditorGUILayoutIconButton Width(float value)
        {
            width = value;
            return this;
        }

        public SEditorGUILayoutIconButton Height(float value)
        {
            height = value;
            return this;
        }

        public SEditorGUILayoutIconButton Disable(bool flag)
        {
            isDisabled = flag;
            return this;
        }

        public override void Render()
        {
            base.Render();
            if (!where())
            {
                return;
            }

            var layoutOptions = new List<GUILayoutOption>();

            if (width != null)
            {
                layoutOptions.Add(GUILayout.Width(width.Value));
            }

            if (height != null)
            {
                layoutOptions.Add(GUILayout.Height(height.Value));
            }

            if (isDisabled)
            {
                EditorGUI.BeginDisabledGroup(true);
            }

            Vector2 previousIconSize = EditorGUIUtility.GetIconSize();
            EditorGUIUtility.SetIconSize(buttonIconSize);
            try
            {
                GUIContent iconContent = EditorGUIUtility.IconContent(iconName) ?? new GUIContent();
                iconContent.text = buttonLabel;

                if (GUILayout.Button(iconContent, layoutOptions.ToArray()))
                {
                    onClick?.Invoke();
                }
            }
            finally
            {
                EditorGUIUtility.SetIconSize(previousIconSize);

                if (isDisabled)
                {
                    EditorGUI.EndDisabledGroup();
                }
            }
        }
    }
}
