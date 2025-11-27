using UnityEngine;

namespace Corelib.SUI
{
    public class SGUILayoutHorizontal : SUIElement
    {
        private SUIElement content;
        private string style = "";

        public SGUILayoutHorizontal()
        {

        }

        public SGUILayoutHorizontal(SUIElement content)
        {
            this.content = content;
        }

        public SGUILayoutHorizontal Style(string style)
        {
            this.style = style;
            return this;
        }

        public SGUILayoutHorizontal Content(SUIElement content)
        {
            this.content = content;
            return this;
        }

        public override void Render()
        {
            base.Render();
            GUILayout.BeginHorizontal(string.IsNullOrWhiteSpace(style) ? GUIStyle.none : style);
            content?.Render();
            GUILayout.EndHorizontal();
        }
    }
}
