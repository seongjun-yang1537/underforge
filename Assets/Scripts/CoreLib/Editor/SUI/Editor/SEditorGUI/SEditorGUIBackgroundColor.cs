using UnityEngine;

namespace Corelib.SUI
{
    public class SEditorGUIBackgroundColor : SUIElement
    {
        private Color backgroundColor;
        private SUIElement content;

        public SEditorGUIBackgroundColor(Color backgroundColor)
        {
            this.backgroundColor = backgroundColor;
        }

        public SEditorGUIBackgroundColor Content(SUIElement value)
        {
            content = value;
            return this;
        }

        public override void Render()
        {
            Color previous = GUI.backgroundColor;
            GUI.backgroundColor = backgroundColor;
            content?.Render();
            GUI.backgroundColor = previous;
        }
    }
}
