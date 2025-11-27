using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UObject = UnityEngine.Object;

namespace Corelib.SUI
{
    public class SEditorGUILayout : SUIElement
    {
        protected SEditorGUILayout(UnityAction onRender) : base(onRender)
        {
        }

        public static SEditorGUILayoutLabel Label(string label)
            => new SEditorGUILayoutLabel(label);

        public static SEditorGUILayoutButton Button(string label)
            => new SEditorGUILayoutButton(label);

        public static SEditorGUILayoutButton Button(GUIContent guiContent)
            => new SEditorGUILayoutButton(guiContent);

        public static SEditorGUILayoutIconButton IconButton(string iconName, string label, Vector2 iconSize)
            => new SEditorGUILayoutIconButton(iconName, label, iconSize);

        public static SEditorGUILayoutTexture Texture(Texture image)
            => new SEditorGUILayoutTexture(image);

        public static SEditorGUILayoutHorizontal Horizontal(string style = "") =>
            new SEditorGUILayoutHorizontal(style);

        public static SEditorGUILayoutVertical Vertical(string style = "") =>
            new SEditorGUILayoutVertical(style);

        public static SEditorGUILayoutVertical Vertical(GUIStyle guiStyle) =>
            new SEditorGUILayoutVertical(guiStyle);

        public static SEditorGUILayoutToggle Toggle(string label, bool value)
            => new SEditorGUILayoutToggle(label, value);

        public static SEditorGUILayoutSlider Slider(string prefix, float value, float minValue = 0f, float maxValue = 1.0f)
            => new SEditorGUILayoutSlider(prefix, value, minValue, maxValue);

        public static SEditorGUILayoutFoldoutHeaderGroup FoldoutHeaderGroup(string prefix, bool foldout) =>
            new SEditorGUILayoutFoldoutHeaderGroup(prefix, foldout);

        public static SEditorGUILayoutFoldout Foldout(string prefix, bool foldout) =>
            new SEditorGUILayoutFoldout(prefix, foldout);

        public static SUIElement Action(UnityAction action) =>
            new SEditorGUILayout(() => action?.Invoke());

        public static SEditorGUILayoutText Text(string prefix, string text = "")
            => new SEditorGUILayoutText(prefix, text);

        public static SEditorGUILayoutTextArea TextArea(string label, string text = "")
            => new SEditorGUILayoutTextArea(label, text);

        public static SEditorGUILayoutVector3 Vector3(string prefix, Vector3 value)
            => new SEditorGUILayoutVector3(prefix, value);

        public static SEditorGUILayoutVector3Int Vector3Int(string prefix, Vector3Int value)
            => new SEditorGUILayoutVector3Int(prefix, value);

        public static SEditorGUILayoutInt Int(string prefix, int value)
            => new SEditorGUILayoutInt(prefix, value);

        public static SEditorGUILayoutFloat Float(string prefix, float value)
            => new SEditorGUILayoutFloat(prefix, value);

        public static SEditorGUILayoutColor Color(string prefix, Color color)
            => new SEditorGUILayoutColor(prefix, color);

        public static SEditorGUILayoutMinMaxSlider MinMaxSlider(float minValue, float maxValue)
            => new SEditorGUILayoutMinMaxSlider(minValue, maxValue);

        public static SEditorGUILayoutProperty Property(SerializedProperty property)
            => new SEditorGUILayoutProperty(property);

        public static SEditorGUILayoutObject Object(string label, UObject obj, Type type)
            => new SEditorGUILayoutObject(label, obj, type);

        public static SEditorGUILayoutProgressBar ProgressBar(string label, float value)
            => new SEditorGUILayoutProgressBar(label, value);

        public static SEditorGUILayoutEnum Enum(string label, Enum value)
            => new SEditorGUILayoutEnum(label, value);

        public static SEditorGUILayoutEnumFlags EnumFlags(string label, Enum value)
            => new SEditorGUILayoutEnumFlags(label, value);

        public static SEditorGUILayoutGroup Group(string title)
            => new SEditorGUILayoutGroup(title);

        public static SEditorGUILayout Separator(float thickness = 1.0f)
            => new SEditorGUILayout(() =>
            {
                GUILayout.Space(1f);
                Rect rect = EditorGUILayout.GetControlRect(false, thickness);
                rect.height = thickness;
                EditorGUI.DrawRect(rect, new Color(0.3f, 0.3f, 0.3f));
                GUILayout.Space(1f);
            });

        public static SEditorGUILayout VerticalSeparator(float thickness = 1.0f, float height = 0f)
            => new SEditorGUILayout(() =>
            {
                GUILayout.Space(1f);
                GUILayoutOption heightOption = height > 0f ? GUILayout.Height(height) : GUILayout.ExpandHeight(true);
                Rect rect = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, GUILayout.Width(thickness), heightOption);
                rect.width = thickness;
                if (height > 0f)
                {
                    rect.height = height;
                }

                EditorGUI.DrawRect(rect, new Color(0.3f, 0.3f, 0.3f));
                GUILayout.Space(1f);
            });

        public static SEditorGUILayoutFoldGroup FoldGroup(string label, bool foldout)
            => new SEditorGUILayoutFoldGroup(label, foldout);

        public static SEditorGUILayout Space(float space = 10f)
            => new SEditorGUILayout(() => EditorGUILayout.Space(space));

        public static SEditorGUILayoutHelpBox HelpBox(string label, MessageType messageType)
            => new SEditorGUILayoutHelpBox(label, messageType);

        public static SEditorGUILayoutToolbar Toolbar(int selected, params string[] labels)
            => new SEditorGUILayoutToolbar(selected, labels);

        public static SEditorGUILayoutToolbar Toolbar(int selected, params GUIContent[] contents)
            => new SEditorGUILayoutToolbar(selected, contents);

        public static SEditorGUILayoutVerticalList<T> VerticalList<T>(IList<T> source, Func<T, SUIElement> itemRenderer)
            => new SEditorGUILayoutVerticalList<T>(source, itemRenderer);

        public static SEditorGUILayoutSelectable Selectable(SUIElement content)
            => new SEditorGUILayoutSelectable(content);

        public static SEditorGUILayoutSearchField SearchField(string text, string controlName = null)
            => new SEditorGUILayoutSearchField(text, controlName);

        public static SEditorGUILayoutScrollView ScrollView(Vector2 position, float height, SUIElement content, bool enableHorizontalScroll = true)
            => new SEditorGUILayoutScrollView(position, height, content, enableHorizontalScroll);

        public static SEditorGUILayoutListItem ListItem(SUIElement content, bool isOdd)
            => new SEditorGUILayoutListItem(content, isOdd);

        public static SEditorGUILayoutPopup Popup(int index, string[] options)
            => new SEditorGUILayoutPopup(index, options);

        public static SEditorGUILayoutSearchablePopup SearchablePopup(int index, string[] options)
            => new SEditorGUILayoutSearchablePopup(index, options);

        public static SEditorGUILayoutVar<T> Var<T>(string label, T value)
            => new SEditorGUILayoutVar<T>(label, value);

        public static SUIElement Header(string title)
            => Vertical("HelpBox")
                .Content(
                    Label($"[{title}]")
                    .Bold()
                    .Align(TextAnchor.MiddleCenter)
                );
    }
}