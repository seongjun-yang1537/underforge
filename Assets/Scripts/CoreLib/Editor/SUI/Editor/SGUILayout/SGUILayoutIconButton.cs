// Generated under Codex compliance with AGENTS.md (cave1)
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace Corelib.SUI
{
    public class SGUILayoutIconButton : SUIElement, IWidthable<SGUILayoutIconButton>
    {
        private readonly GUIContent _guiContent;
        private UnityAction _onClick;
        private float? _width;
        private float? _height;
        private Func<bool> _where = () => true;
        private bool _isDisabled;

        public SGUILayoutIconButton(GUIContent guiContent)
        {
            _guiContent = guiContent;
        }

        public SGUILayoutIconButton Where(Func<bool> predicate)
        {
            _where = predicate;
            return this;
        }

        public SGUILayoutIconButton OnClick(UnityAction clickAction)
        {
            _onClick = clickAction;
            return this;
        }

        public SGUILayoutIconButton Width(float value)
        {
            _width = value;
            return this;
        }

        public SGUILayoutIconButton Height(float value)
        {
            _height = value;
            return this;
        }

        public SGUILayoutIconButton SetDisable(bool flag)
        {
            _isDisabled = flag;
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

            var wasEnabled = GUI.enabled;
            GUI.enabled = !_isDisabled;

            if (GUILayout.Button(_guiContent, layoutOptions.ToArray()))
            {
                _onClick?.Invoke();
            }

            GUI.enabled = wasEnabled;
        }
    }
}
