using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Corelib.Utils;
using TriInspector;
using UnityEngine;
using UnityEngine.Events;

namespace UI
{
    [DeclareBoxGroup("Placeholder")]
    [DeclareBoxGroup("UIMonoBehaviour", Title = "UI MonoBehaviour")]
    public abstract class UIMonoBehaviour : MonoBehaviour
    {
        public UnityEvent onRenderEnd = new();

        public IUIViewHandler viewHandler;
        protected UIMonoBehaviour parent;

        [Serializable]
        public struct ChildEntry
        {
            [HideInInspector] public string fieldName;
            [ReadOnly] public UIMonoBehaviour childUI;
        }

        [Group("UIMonoBehaviour"), ReadOnly]
        [ListDrawerSettings(HideAddButton = true, HideRemoveButton = true)]
        [SerializeField] private List<ChildEntry> childUIs = new();

        private static readonly Dictionary<Type, FieldInfo[]> fieldCache = new();

        private static FieldInfo[] GetCachedFields(Type type)
        {
            if (!fieldCache.TryGetValue(type, out var fieldInfos))
            {
                const BindingFlags FieldSearchFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy;
                fieldInfos = type.GetFields(FieldSearchFlags);
                fieldCache[type] = fieldInfos;
            }
            return fieldInfos;
        }

        protected virtual void Awake()
        {
            CacheChildUIs();
            var parentTransform = transform.parent;
            if (parentTransform != null)
            {
                parent = parentTransform.GetComponentInParent<UIMonoBehaviour>();
                if (parent != null && parent.viewHandler != null)
                    BindViewHandler(parent.viewHandler);
            }
            UnityLifecycleBindUtil.ConstructLifecycleObjects(this);
        }

        protected virtual void OnEnable() => UnityLifecycleBindUtil.OnEnable(this);
        protected virtual void OnDisable() => UnityLifecycleBindUtil.OnDisable(this);

        private void CacheChildUIs()
        {
            childUIs.Clear();

            foreach (var fieldInfo in GetCachedFields(GetType()))
            {
                var fieldType = fieldInfo.FieldType;

                if (typeof(UIMonoBehaviour).IsAssignableFrom(fieldType))
                {
                    if (fieldInfo.GetValue(this) is UIMonoBehaviour childUI && childUI.transform != transform && childUI.transform.IsChildOf(transform))
                    {
                        childUI.parent = this;
                        if (viewHandler != null)
                            childUI.BindViewHandler(viewHandler);
                        childUIs.Add(new ChildEntry { fieldName = fieldInfo.Name, childUI = childUI });
                    }
                }
                else if (typeof(IEnumerable).IsAssignableFrom(fieldType))
                {
                    var elementType = fieldType.IsArray ? fieldType.GetElementType()
                                         : fieldType.IsGenericType ? fieldType.GetGenericArguments()[0] : null;

                    if (elementType != null && typeof(UIMonoBehaviour).IsAssignableFrom(elementType) &&
                        fieldInfo.GetValue(this) is IEnumerable enumerable)
                    {
                        foreach (var item in enumerable)
                            if (item is UIMonoBehaviour childUI && childUI.transform != transform && childUI.transform.IsChildOf(transform))
                            {
                                childUI.parent = this;
                                if (viewHandler != null)
                                    childUI.BindViewHandler(viewHandler);
                                childUIs.Add(new ChildEntry { fieldName = fieldInfo.Name, childUI = childUI });
                            }
                    }
                }
            }
        }

#if UNITY_EDITOR
        private void OnValidate() => CacheChildUIs();
#endif

        public IReadOnlyList<UIMonoBehaviour> Children
        {
            get
            {
                var childrenList = new List<UIMonoBehaviour>(childUIs.Count);
                foreach (var entry in childUIs) childrenList.Add(entry.childUI);
                return childrenList;
            }
        }

        public UIMonoBehaviour Parent => parent;

        public virtual void BindViewHandler(IUIViewHandler handler)
        {
            viewHandler = handler;
            foreach (var entry in childUIs) entry.childUI.BindViewHandler(handler);
        }

        [Button("Render")]
        public abstract void Render();
    }
}
