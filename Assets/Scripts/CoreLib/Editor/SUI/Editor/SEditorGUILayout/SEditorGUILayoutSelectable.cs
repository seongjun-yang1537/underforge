using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace Corelib.SUI
{
    public class SEditorGUILayoutSelectable : SUIElement
    {
        private SUIElement innerContent;
        private UnityAction onClick;
        private bool isSelected;
        private Color highlightColor = new Color(0.75f, 0.85f, 1f, 0.5f);

        public SEditorGUILayoutSelectable(SUIElement innerContent)
        {
            this.innerContent = innerContent;
        }

        public SEditorGUILayoutSelectable OnClick(UnityAction onClick)
        {
            this.onClick = onClick;
            return this;
        }

        public SEditorGUILayoutSelectable Selected(bool isSelected)
        {
            this.isSelected = isSelected;
            return this;
        }

        public override void Render()
        {
            EditorGUILayout.BeginVertical();
            innerContent?.Render();
            EditorGUILayout.EndVertical();
            Rect contentRect = GUILayoutUtility.GetLastRect();
            if (Event.current.type == EventType.Repaint && isSelected) EditorGUI.DrawRect(contentRect, highlightColor);
            if (Event.current.type == EventType.MouseDown && contentRect.Contains(Event.current.mousePosition))
            {
                onClick?.Invoke();
                Event.current.Use();
            }
        }
    }
}
