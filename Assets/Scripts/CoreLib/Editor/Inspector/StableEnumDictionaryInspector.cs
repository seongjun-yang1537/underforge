using System;
using System.Collections.Generic;
using System.Linq;
using Corelib.SUI;
using UnityEditor;
using UnityEngine;

namespace Corelib.Utils
{
    public static class StableEnumDictionaryInspector
    {
        private static class TempCache<TKey, TValue>
        {
            public static TKey NewKey;
            public static TValue NewValue;
        }

        public static SUIElement Render<TKey, TValue>(
            StableEnumDictionary<TKey, TValue> dictionary,
            Action onModified = null,
            Func<TValue, Action<TValue>, SUIElement> customValueRenderer = null)
            where TKey : Enum
        {
            if (dictionary == null)
                return SEditorGUILayout.Label(
                    $"StableEnumDictionary<{typeof(TKey).Name}, {typeof(TValue).Name}>: (null)");

            // 캐시 불러오기
            var newKey = TempCache<TKey, TValue>.NewKey;
            var newValue = TempCache<TKey, TValue>.NewValue;

            var container = SEditorGUILayout.Vertical();
            var elements = new List<SUIElement>();

            var originalLabelWidth = EditorGUIUtility.labelWidth;
            EditorGUIUtility.labelWidth = 60f;

            elements.Add(
                SEditorGUILayout.Horizontal()
                    .Content(
                        SEditorGUILayout.Var("New Key", newKey).OnValueChanged(val =>
                        {
                            newKey = val;
                            TempCache<TKey, TValue>.NewKey = val;
                        })
                        + (customValueRenderer != null
                            ? customValueRenderer(newValue, val =>
                                {
                                    newValue = val;
                                    TempCache<TKey, TValue>.NewValue = val;
                                })
                            : SEditorGUILayout.Var("New Value", newValue).OnValueChanged(val =>
                                {
                                    newValue = val;
                                    TempCache<TKey, TValue>.NewValue = val;
                                }))
                        + SEditorGUILayout.Button("+").Width(20)
                            .OnClick(() =>
                            {
                                if (dictionary.ContainsKey(newKey))
                                {
                                    Debug.LogError($"Key '{newKey}' 는 이미 존재합니다.");
                                    return;
                                }

                                dictionary.Add(newKey, newValue);
                                newKey = default;
                                newValue = default;
                                onModified?.Invoke();
                                GUI.changed = true;
                            })
                    )
            );
            elements.Add(SEditorGUILayout.Separator());
            elements.Add(SEditorGUILayout.Space(2));

            // 기존 항목 렌더링
            foreach (var kvp in dictionary.EnumPairs.ToList())
            {
                var currentKey = kvp.Key;
                elements.Add(
                    SEditorGUILayout.Horizontal()
                        .Content(
                            SEditorGUILayout.Var("Key", currentKey)
                            + (customValueRenderer != null
                                ? customValueRenderer(kvp.Value, v =>
                                    {
                                        dictionary[currentKey] = v;
                                        onModified?.Invoke();
                                        GUI.changed = true;
                                    })
                                : SEditorGUILayout.Var("Value", kvp.Value).OnValueChanged(v =>
                                    {
                                        dictionary[currentKey] = v;
                                        onModified?.Invoke();
                                        GUI.changed = true;
                                    }))
                            + SEditorGUILayout.Button("-").Width(20)
                                .OnClick(() =>
                                {
                                    dictionary.Remove(currentKey);
                                    onModified?.Invoke();
                                    GUI.changed = true;
                                })
                        )
                );
            }

            // labelWidth 원복
            elements.Add(SEditorGUILayout.Action(() =>
                EditorGUIUtility.labelWidth = originalLabelWidth));

            if (elements.Any())
                container.Content(elements.Aggregate((curr, next) => curr + next));

            // 캐시 저장
            TempCache<TKey, TValue>.NewKey = newKey;
            TempCache<TKey, TValue>.NewValue = newValue;

            return SEditorGUILayout.Group($"Dictionary<{typeof(TKey).Name}, {typeof(TValue).Name}>")
                                    .Content(container);
        }

    }
}
