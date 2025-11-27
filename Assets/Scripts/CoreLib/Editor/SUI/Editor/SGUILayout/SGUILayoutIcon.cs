// Generated under Codex compliance with AGENTS.md (cave1)
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Corelib.SUI
{
    public class SGUILayoutIcon : SUIElement, IWidthable<SGUILayoutIcon>
    {
        private readonly GUIContent _guiContent;
        private float? _width;
        private float? _height;
        private Func<bool> _where = () => true;

        public SGUILayoutIcon(GUIContent guiContent)
        {
            _guiContent = guiContent;
        }

        public SGUILayoutIcon Where(Func<bool> predicate)
        {
            _where = predicate;
            return this;
        }

        public SGUILayoutIcon Width(float value)
        {
            _width = value;
            return this;
        }

        public SGUILayoutIcon Height(float value)
        {
            _height = value;
            return this;
        }

        public override void Render()
        {
            base.Render();
            if (!_where())
            {
                return;
            }

            var layoutOptions = new List<GUILayoutOption>();

            if (_width != null)
            {
                layoutOptions.Add(GUILayout.Width(_width.Value));
            }

            if (_height != null)
            {
                layoutOptions.Add(GUILayout.Height(_height.Value));
            }

            GUILayout.Label(_guiContent, layoutOptions.ToArray());
        }
    }
}
