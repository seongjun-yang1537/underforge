using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.IMGUI.Controls;
using UnityEngine;
using UnityEngine.Events;

namespace Corelib.SUI
{
    public class SEditorGUILayoutSearchablePopup : SUIElement
    {
        private int selectedIndex;
        private readonly string[] displayOptions;
        private UnityAction<int> valueChanged;
        private float? elementWidth;
        private float? elementHeight;

        public SEditorGUILayoutSearchablePopup(int index, string[] options)
        {
            selectedIndex = index;
            displayOptions = options ?? Array.Empty<string>();
        }

        public SEditorGUILayoutSearchablePopup OnValueChanged(UnityAction<int> callback)
        {
            valueChanged = callback;
            return this;
        }

        public SEditorGUILayoutSearchablePopup Width(float width)
        {
            elementWidth = width;
            return this;
        }

        public SEditorGUILayoutSearchablePopup Height(float height)
        {
            elementHeight = height;
            return this;
        }

        public override void Render()
        {
            GUILayoutOption[] layoutOptions = BuildLayoutOptions();
            string label = ResolveLabel();
            GUIContent labelContent = new GUIContent(label);
            Rect popupRect = GUILayoutUtility.GetRect(labelContent, EditorStyles.popup, layoutOptions);
            if (GUI.Button(popupRect, labelContent, EditorStyles.popup))
            {
                SearchablePopupWindow.Show(popupRect, displayOptions, selectedIndex, OnOptionSelected);
            }
        }

        private GUILayoutOption[] BuildLayoutOptions()
        {
            if (elementWidth.HasValue && elementHeight.HasValue)
            {
                return new GUILayoutOption[] { GUILayout.Width(elementWidth.Value), GUILayout.Height(elementHeight.Value) };
            }
            if (elementWidth.HasValue)
            {
                return new GUILayoutOption[] { GUILayout.Width(elementWidth.Value) };
            }
            if (elementHeight.HasValue)
            {
                return new GUILayoutOption[] { GUILayout.Height(elementHeight.Value) };
            }
            return Array.Empty<GUILayoutOption>();
        }

        private string ResolveLabel()
        {
            if (displayOptions.Length == 0)
            {
                selectedIndex = -1;
                return "";
            }
            if (selectedIndex < 0 || selectedIndex >= displayOptions.Length)
            {
                selectedIndex = Mathf.Clamp(selectedIndex, 0, displayOptions.Length - 1);
            }
            return displayOptions[selectedIndex];
        }

        private void OnOptionSelected(int index)
        {
            if (index < 0 || index >= displayOptions.Length)
            {
                return;
            }
            selectedIndex = index;
            valueChanged?.Invoke(index);
        }

        private class SearchablePopupWindow : PopupWindowContent
        {
            private readonly string[] options;
            private readonly Action<int> onSelect;
            private int selectedIndex;
            private SearchField searchField;
            private string searchText = string.Empty;
            private Vector2 scrollPosition;

            private SearchablePopupWindow(string[] options, int index, Action<int> onSelect)
            {
                this.options = options ?? Array.Empty<string>();
                this.onSelect = onSelect;
                selectedIndex = Mathf.Clamp(index, 0, this.options.Length > 0 ? this.options.Length - 1 : 0);
            }

            public static void Show(Rect activatorRect, string[] options, int index, Action<int> onSelect)
            {
                PopupWindow.Show(activatorRect, new SearchablePopupWindow(options, index, onSelect));
            }

            public override Vector2 GetWindowSize()
            {
                return new Vector2(320f, 400f);
            }

            public override void OnGUI(Rect rect)
            {
                if (searchField == null)
                {
                    searchField = new SearchField();
                }
                searchText = searchField.OnGUI(searchText);
                List<int> filteredIndices = BuildFilteredIndices();
                HandleKeyboard(filteredIndices);
                using (var scrollScope = new EditorGUILayout.ScrollViewScope(scrollPosition))
                {
                    scrollPosition = scrollScope.scrollPosition;
                    if (filteredIndices.Count == 0)
                    {
                        Rect emptyRect = GUILayoutUtility.GetRect(0f, EditorGUIUtility.singleLineHeight, GUILayout.ExpandWidth(true));
                        if (Event.current.type == EventType.Repaint)
                        {
                            EditorStyles.label.Draw(emptyRect, "No Results", false, false, false, false);
                        }
                    }
                    else
                    {
                        foreach (int optionIndex in filteredIndices)
                        {
                            RenderOption(optionIndex);
                        }
                    }
                }
            }

            private List<int> BuildFilteredIndices()
            {
                List<int> indices = new List<int>();
                for (int i = 0; i < options.Length; i++)
                {
                    string option = options[i] ?? string.Empty;
                    if (string.IsNullOrEmpty(searchText) || option.IndexOf(searchText, StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        indices.Add(i);
                    }
                }
                return indices;
            }

            private void HandleKeyboard(List<int> filteredIndices)
            {
                Event currentEvent = Event.current;
                if (currentEvent.type != EventType.KeyDown)
                {
                    return;
                }
                if (currentEvent.keyCode == KeyCode.Escape)
                {
                    editorWindow.Close();
                    currentEvent.Use();
                    return;
                }
                if (currentEvent.keyCode == KeyCode.Return || currentEvent.keyCode == KeyCode.KeypadEnter)
                {
                    if (filteredIndices.Count > 0)
                    {
                        SelectOption(filteredIndices[0]);
                    }
                    currentEvent.Use();
                }
            }

            private void RenderOption(int optionIndex)
            {
                Rect optionRect = GUILayoutUtility.GetRect(0f, EditorGUIUtility.singleLineHeight + 4f, GUILayout.ExpandWidth(true));
                bool isHover = optionRect.Contains(Event.current.mousePosition);
                bool isSelected = optionIndex == selectedIndex;
                string optionLabel = options[optionIndex] ?? string.Empty;
                if (Event.current.type == EventType.Repaint)
                {
                    GUIStyle style = new GUIStyle("MenuItem");
                    style.Draw(optionRect, optionLabel, isHover, false, isSelected, isHover);
                }
                EditorGUIUtility.AddCursorRect(optionRect, MouseCursor.Link);
                if (Event.current.type == EventType.MouseDown && Event.current.button == 0 && optionRect.Contains(Event.current.mousePosition))
                {
                    SelectOption(optionIndex);
                    Event.current.Use();
                }
            }

            private void SelectOption(int optionIndex)
            {
                selectedIndex = optionIndex;
                onSelect?.Invoke(optionIndex);
                editorWindow.Close();
            }
        }
    }
}
