using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace Corelib.SUI
{
    public class SEditorGUILayoutScrollView : SUIElement
    {
        private Vector2 scrollPosition;
        private float viewHeight;
        private SUIElement contentElement;
        private UnityAction<Vector2> positionChanged;
        private bool shouldExpandHeight;
        private readonly bool enableHorizontalScroll;

        public SEditorGUILayoutScrollView(Vector2 position, float height, SUIElement content, bool enableHorizontal = true)
        {
            scrollPosition = position;
            viewHeight = height;
            contentElement = content;
            enableHorizontalScroll = enableHorizontal;
        }

        public SEditorGUILayoutScrollView OnPositionChanged(UnityAction<Vector2> callback)
        {
            positionChanged = callback;
            return this;
        }

        public SEditorGUILayoutScrollView ExpandHeight()
        {
            shouldExpandHeight = true;
            return this;
        }

        public override void Render()
        {
            GUILayoutOption heightOption = shouldExpandHeight
                ? GUILayout.ExpandHeight(true)
                : GUILayout.Height(viewHeight);
            Vector2 newPosition = enableHorizontalScroll
                ? EditorGUILayout.BeginScrollView(scrollPosition, heightOption)
                : EditorGUILayout.BeginScrollView(scrollPosition, GUIStyle.none, GUI.skin.verticalScrollbar, heightOption);
            if (newPosition != scrollPosition)
            {
                scrollPosition = newPosition;
                positionChanged?.Invoke(scrollPosition);
            }
            contentElement?.Render();
            EditorGUILayout.EndScrollView();
        }
    }
}
