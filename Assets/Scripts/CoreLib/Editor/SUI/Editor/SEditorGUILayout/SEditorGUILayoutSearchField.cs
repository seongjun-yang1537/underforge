using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.Events;

namespace Corelib.SUI
{
    public class SEditorGUILayoutSearchField : SUIElement
    {
        private static readonly Dictionary<string, SearchField> SearchFieldCache = new();

        private string searchText;
        private readonly string controlKey;
        private readonly SearchField searchFieldControl;
        private float? width;
        private float? height;
        private UnityAction<string> onValueChanged;

        public SEditorGUILayoutSearchField(string text, string controlName = null)
        {
            searchText = text;
            controlKey = string.IsNullOrEmpty(controlName) ? string.Empty : controlName;
            searchFieldControl = GetSearchFieldControl();
        }

        public SEditorGUILayoutSearchField OnValueChanged(UnityAction<string> callback)
        {
            onValueChanged = callback;
            return this;
        }

        public SEditorGUILayoutSearchField Width(float value)
        {
            width = value;
            return this;
        }

        public SEditorGUILayoutSearchField Height(float value)
        {
            height = value;
            return this;
        }

        public override void Render()
        {
            List<GUILayoutOption> options = new();
            if (width != null) options.Add(GUILayout.Width(width.Value));
            float rectHeight = height ?? EditorGUIUtility.singleLineHeight;
            Rect rect = EditorGUILayout.GetControlRect(false, rectHeight, options.ToArray());
            string newText = searchFieldControl.OnGUI(rect, searchText);
            if (newText != searchText)
            {
                searchText = newText;
                onValueChanged?.Invoke(newText);
            }
        }

        private SearchField GetSearchFieldControl()
        {
            if (!SearchFieldCache.TryGetValue(controlKey, out SearchField cachedControl) || cachedControl == null)
            {
                cachedControl = new SearchField();
                SearchFieldCache[controlKey] = cachedControl;
            }

            return cachedControl;
        }
    }
}
