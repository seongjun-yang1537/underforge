using UnityEditor;
using UnityEngine;

namespace Corelib.SUI
{
    public class SEditorGUILayoutListItem : SUIElement
    {
        private readonly SUIElement itemContent;
        private readonly bool isOddItem;
        private static readonly Color evenColor = new(0.18f, 0.18f, 0.18f);
        private static readonly Color oddColor = new(0.22f, 0.22f, 0.22f);

        public SEditorGUILayoutListItem(SUIElement content, bool isOdd)
        {
            itemContent = content;
            isOddItem = isOdd;
        }

        public override void Render()
        {
            Rect rect = EditorGUILayout.BeginVertical();
            EditorGUI.DrawRect(rect, isOddItem ? oddColor : evenColor);
            itemContent?.Render();
            EditorGUILayout.EndVertical();
        }
    }
}
