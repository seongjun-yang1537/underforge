using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace Corelib.SUI
{
    public class SEditorGUILayoutVerticalList<T> : SUIElement
    {
        private IList<T> source;
        private Func<T, SUIElement> itemRenderer;
        private Vector2 scrollPosition;
        private float viewHeight = 200f;
        private float itemHeight = 20f;
        private UnityAction<Vector2> scrollChanged;

        public SEditorGUILayoutVerticalList(IList<T> source, Func<T, SUIElement> itemRenderer)
        {
            this.source = source;
            this.itemRenderer = itemRenderer;
        }

        public SEditorGUILayoutVerticalList<T> ViewHeight(float value)
        {
            viewHeight = value;
            return this;
        }

        public SEditorGUILayoutVerticalList<T> ItemHeight(float value)
        {
            itemHeight = value;
            return this;
        }

        public SEditorGUILayoutVerticalList<T> OnScrollChanged(UnityAction<Vector2> callback)
        {
            scrollChanged = callback;
            return this;
        }

        public SEditorGUILayoutVerticalList<T> ScrollPosition(Vector2 position)
        {
            scrollPosition = position;
            return this;
        }

        public override void Render()
        {
            Vector2 updatedScroll = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(viewHeight));
            if (updatedScroll != scrollPosition)
            {
                scrollPosition = updatedScroll;
                scrollChanged?.Invoke(scrollPosition);
            }
            int firstIndex = Mathf.Max(0, Mathf.FloorToInt(scrollPosition.y / itemHeight));
            int visibleCount = Mathf.CeilToInt(viewHeight / itemHeight) + 1;
            int lastIndex = Mathf.Min(source.Count, firstIndex + visibleCount);
            GUILayout.Space(firstIndex * itemHeight);
            for (int i = firstIndex; i < lastIndex; i++)
                itemRenderer(source[i])?.Render();
            GUILayout.Space((source.Count - lastIndex) * itemHeight);
            EditorGUILayout.EndScrollView();
            rect = GUILayoutUtility.GetLastRect();
        }
    }
}
