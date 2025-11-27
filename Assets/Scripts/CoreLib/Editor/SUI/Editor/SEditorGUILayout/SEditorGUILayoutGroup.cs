using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;

namespace Corelib.SUI
{
    public class SEditorGUILayoutGroup : SUIElement
    {
        private SUIElement content;
        private SUIElement headerRight;
        private string title = "";
        private Func<bool> where = () => true;

        public SEditorGUILayoutGroup(string title)
        {
            this.title = title;
        }

        public SEditorGUILayoutGroup Content(SUIElement content = null)
        {
            this.content = content;
            return this;
        }

        public SEditorGUILayoutGroup Where(Func<bool> callback)
        {
            this.where = callback;
            return this;
        }

        public SEditorGUILayoutGroup HeaderRight(SUIElement headerRight)
        {
            this.headerRight = headerRight;
            return this;
        }

        public override void Render()
        {
            SEditorGUILayout.Vertical("box")
            .Content(
                SEditorGUILayout.Vertical("HelpBox")
                .Content(
                    SEditorGUILayout.Label($"[{title}]")
                    .Bold()
                    .Align(TextAnchor.MiddleCenter)
                    + (headerRight ?? SUIElement.Empty())
                )
                + SEditorGUILayout.Separator()
                + SEditorGUILayout.Action(() =>
                {
                    if (where())
                        content?.Render();
                })
            )
            .Render();
        }
    }
}