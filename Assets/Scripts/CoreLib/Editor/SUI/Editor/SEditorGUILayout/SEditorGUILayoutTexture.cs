using System.Collections.Generic;
using UnityEngine;

namespace Corelib.SUI
{
    public class SEditorGUILayoutTexture : SUIElement
    {
        private Texture sourceTexture;
        private float? width;
        private float? height;

        public SEditorGUILayoutTexture(Texture sourceTexture)
        {
            this.sourceTexture = sourceTexture;
        }

        public SEditorGUILayoutTexture Width(float value)
        {
            width = value;
            return this;
        }

        public SEditorGUILayoutTexture Height(float value)
        {
            height = value;
            return this;
        }

        public override void Render()
        {
            List<GUILayoutOption> options = new();
            if (width != null) options.Add(GUILayout.Width(width.Value));
            if (height != null) options.Add(GUILayout.Height(height.Value));
            GUILayout.Label(sourceTexture, options.ToArray());
        }
    }
}
