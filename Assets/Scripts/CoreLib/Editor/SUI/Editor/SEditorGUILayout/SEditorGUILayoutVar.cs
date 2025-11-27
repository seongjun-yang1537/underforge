using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Corelib.Utils;
using UObject = UnityEngine.Object;

namespace Corelib.SUI
{
    static class EnumDefaults
    {
        private static readonly Dictionary<Type, Enum> enumCache = new();
        public static Enum Get(Type type)
        {
            if (!enumCache.TryGetValue(type, out var enumValue))
            {
                enumValue = (Enum)Enum.GetValues(type).GetValue(0);
                enumCache[type] = enumValue;
            }
            return enumValue;
        }
    }

    public class SEditorGUILayoutVar<T> : SUIElement
    {
        private string fieldLabel;
        private T fieldValue;
        private UnityAction<T> valueChangedCallback;
        private float? fieldWidth;
        private float? fieldHeight;

        public SEditorGUILayoutVar(string label, T value)
        {
            fieldLabel = label;
            fieldValue = value;
        }

        public SEditorGUILayoutVar<T> OnValueChanged(UnityAction<T> onValueChanged)
        {
            valueChangedCallback = onValueChanged;
            return this;
        }

        public SEditorGUILayoutVar<T> Width(float width)
        {
            fieldWidth = width;
            return this;
        }

        public SEditorGUILayoutVar<T> Height(float height)
        {
            fieldHeight = height;
            return this;
        }

        public override void Render()
        {
            var type = typeof(T);
            SUIElement element = SUIElement.Empty();
            if (typeof(UObject).IsAssignableFrom(type))
                element = SEditorGUILayout.Object(fieldLabel, fieldValue as UObject, type).OnValueChanged(v =>
                {
                    fieldValue = (T)(object)v;
                    valueChangedCallback?.Invoke((T)(object)v);
                });
            else if (type.IsEnum)
                element = SEditorGUILayout.Enum(fieldLabel, fieldValue == null ? EnumDefaults.Get(type) : (Enum)(object)fieldValue).OnValueChanged(v =>
                {
                    var enumValue = (T)(object)v;
                    fieldValue = enumValue;
                    valueChangedCallback?.Invoke(enumValue);
                });
            else if (type == typeof(int))
            {
                var intElement = SEditorGUILayout.Int(fieldLabel, Convert.ToInt32(fieldValue)).OnValueChanged(v =>
                {
                    fieldValue = (T)(object)v;
                    valueChangedCallback?.Invoke((T)(object)v);
                });
                if (fieldWidth != null) intElement.Width(fieldWidth.Value);
                if (fieldHeight != null) intElement.Height(fieldHeight.Value);
                element = intElement;
            }
            else if (type == typeof(uint))
            {
                var unsignedIntElement = SEditorGUILayout.Int(fieldLabel, Convert.ToInt32(fieldValue)).Min(0).OnValueChanged(v =>
                {
                    var newValue = (uint)v;
                    fieldValue = (T)(object)newValue;
                    valueChangedCallback?.Invoke((T)(object)newValue);
                });
                if (fieldWidth != null) unsignedIntElement.Width(fieldWidth.Value);
                if (fieldHeight != null) unsignedIntElement.Height(fieldHeight.Value);
                element = unsignedIntElement;
            }
            else if (type == typeof(float))
            {
                var floatElement = SEditorGUILayout.Float(fieldLabel, Convert.ToSingle(fieldValue)).OnValueChanged(v =>
                {
                    fieldValue = (T)(object)v;
                    valueChangedCallback?.Invoke((T)(object)v);
                });
                if (fieldWidth != null) floatElement.Width(fieldWidth.Value);
                if (fieldHeight != null) floatElement.Height(fieldHeight.Value);
                element = floatElement;
            }
            else if (type == typeof(bool))
            {
                var toggleElement = SEditorGUILayout.Toggle(fieldLabel, Convert.ToBoolean(fieldValue)).OnValueChanged(v =>
                {
                    fieldValue = (T)(object)v;
                    valueChangedCallback?.Invoke((T)(object)v);
                });
                element = toggleElement;
            }
            else if (type == typeof(string))
            {
                var textElement = SEditorGUILayout.Text(fieldLabel, fieldValue as string ?? string.Empty).OnValueChanged(v =>
                {
                    fieldValue = (T)(object)v;
                    valueChangedCallback?.Invoke((T)(object)v);
                });
                if (fieldWidth != null) textElement.Width(fieldWidth.Value);
                if (fieldHeight != null) textElement.Height(fieldHeight.Value);
                element = textElement;
            }
            else if (type == typeof(Vector3))
            {
                var vector3Element = SEditorGUILayout.Vector3(fieldLabel, fieldValue == null ? default : (Vector3)(object)fieldValue).OnValueChanged(v =>
                {
                    fieldValue = (T)(object)v;
                    valueChangedCallback?.Invoke((T)(object)v);
                });
                element = vector3Element;
            }
            else if (type == typeof(Vector3Int))
            {
                var vector3IntElement = SEditorGUILayout.Vector3Int(fieldLabel, fieldValue == null ? default : (Vector3Int)(object)fieldValue).OnValueChanged(v =>
                {
                    fieldValue = (T)(object)v;
                    valueChangedCallback?.Invoke((T)(object)v);
                });
                element = vector3IntElement;
            }
            else if (type == typeof(IntRange))
            {
                var intRange = fieldValue as IntRange ?? new IntRange(0, 0);
                fieldValue = (T)(object)intRange;
                var intRangeElement = SEditorGUILayout.Horizontal().LabelWidth(60f).Content(
                    SEditorGUILayout.Label(fieldLabel)
                    + SEditorGUILayout.Int("", intRange.Min).OnValueChanged(v =>
                    {
                        intRange.Min = v;
                        fieldValue = (T)(object)intRange;
                        valueChangedCallback?.Invoke((T)(object)intRange);
                    })
                    + SEditorGUILayout.Label("~").Width(8)
                    + SEditorGUILayout.Int("", intRange.Max).OnValueChanged(v =>
                    {
                        intRange.Max = v;
                        fieldValue = (T)(object)intRange;
                        valueChangedCallback?.Invoke((T)(object)intRange);
                    })
                );
                element = intRangeElement;
            }
            else
                element = SEditorGUILayout.Label($"{fieldLabel}: Unsupported type '{type.Name}'");
            element.Render();
        }
    }
}
