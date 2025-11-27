using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Corelib.SUI
{
    public class SGUILayoutButton : SUIElement
    {
        private readonly string label;
        private readonly GUIContent guiContent;
        private UnityAction onClick;
        private GUIStyle guiStyle;
        private float? width;
        private float? height;
        private GUILayoutOption[] layoutOptions = Array.Empty<GUILayoutOption>();
        private Func<bool> where = () => true;
        private bool isDisabled;

        public SGUILayoutButton(string label)
        {
            this.label = label;
        }

        public SGUILayoutButton(GUIContent guiContent)
        {
            this.guiContent = guiContent;
        }

        public SGUILayoutButton Where(Func<bool> where)
        {
            this.where = where ?? (() => true);
            return this;
        }

        public SGUILayoutButton OnClick(UnityAction onClick)
        {
            this.onClick = onClick;
            return this;
        }

        public SGUILayoutButton Width(float width)
        {
            this.width = width;
            return this;
        }

        public SGUILayoutButton Height(float height)
        {
            this.height = height;
            return this;
        }

        public SGUILayoutButton Disable(bool flag)
        {
            isDisabled = flag;
            return this;
        }

        public SGUILayoutButton GUIStyle(GUIStyle guiStyle)
        {
            this.guiStyle = guiStyle;
            return this;
        }

        public SGUILayoutButton LayoutOptions(params GUILayoutOption[] options)
        {
            layoutOptions = options ?? Array.Empty<GUILayoutOption>();
            return this;
        }

        public override void Render()
        {
            base.Render();
            if (!where()) return;

            var options = new List<GUILayoutOption>(layoutOptions ?? Array.Empty<GUILayoutOption>());

            if (width != null)
                options.Add(GUILayout.Width(width.Value));

            if (height != null)
                options.Add(GUILayout.Height(height.Value));

            bool prevEnabled = GUI.enabled;
            if (isDisabled) GUI.enabled = false;

            bool clicked;
            if (guiContent != null)
            {
                clicked = guiStyle != null
                    ? GUILayout.Button(guiContent, guiStyle, options.ToArray())
                    : GUILayout.Button(guiContent, options.ToArray());
            }
            else
            {
                clicked = guiStyle != null
                    ? GUILayout.Button(label, guiStyle, options.ToArray())
                    : GUILayout.Button(label, options.ToArray());
            }

            if (clicked) onClick?.Invoke();

            if (isDisabled) GUI.enabled = prevEnabled;
        }
    }
}
