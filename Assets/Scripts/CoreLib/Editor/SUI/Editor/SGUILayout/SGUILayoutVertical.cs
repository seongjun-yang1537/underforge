using UnityEngine;

namespace Corelib.SUI
{
    public class SGUILayoutVertical : SUIElement
    {
        private SUIElement content;
        private string style = "";

        public SGUILayoutVertical()
        {

        }

        public SGUILayoutVertical(SUIElement content)
        {
            this.content = content;
        }

        public SGUILayoutVertical Style(string style)
        {
            this.style = style;
            return this;
        }

        public SGUILayoutVertical Content(SUIElement content)
        {
            this.content = content;
            return this;
        }

        public override void Render()
        {
            base.Render();
            GUILayout.BeginVertical(string.IsNullOrWhiteSpace(style) ? GUIStyle.none : style);
            content?.Render();
            GUILayout.EndVertical();
        }
    }
}
