using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace Corelib.SUI
{
    public class SEditorGUILayoutPopup : SUIElement
    {
        private int selectedIndex;
        private string[] displayOptions;
        private UnityAction<int> valueChanged;
        private float? elementWidth;
        private float? elementHeight;

        public SEditorGUILayoutPopup(int index, string[] options)
        {
            selectedIndex = index;
            displayOptions = options;
        }

        public SEditorGUILayoutPopup OnValueChanged(UnityAction<int> callback)
        {
            valueChanged = callback;
            return this;
        }

        public SEditorGUILayoutPopup Width(float width)
        {
            elementWidth = width;
            return this;
        }

        public SEditorGUILayoutPopup Height(float height)
        {
            elementHeight = height;
            return this;
        }

        public override void Render()
        {
            GUILayoutOption[] layoutOptions;
            if (elementWidth.HasValue && elementHeight.HasValue)
                layoutOptions = new GUILayoutOption[] { GUILayout.Width(elementWidth.Value), GUILayout.Height(elementHeight.Value) };
            else if (elementWidth.HasValue)
                layoutOptions = new GUILayoutOption[] { GUILayout.Width(elementWidth.Value) };
            else if (elementHeight.HasValue)
                layoutOptions = new GUILayoutOption[] { GUILayout.Height(elementHeight.Value) };
            else
                layoutOptions = Array.Empty<GUILayoutOption>();
            int newIndex = EditorGUILayout.Popup(selectedIndex, displayOptions, layoutOptions);
            if (newIndex != selectedIndex)
            {
                selectedIndex = newIndex;
                valueChanged?.Invoke(newIndex);
            }
        }
    }
}
