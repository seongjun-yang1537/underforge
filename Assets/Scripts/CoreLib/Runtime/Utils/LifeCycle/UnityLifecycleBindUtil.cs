using System;
using System.Reflection;
using UnityEngine;

namespace Corelib.Utils
{
    public static class UnityLifecycleBindUtil
    {
        private static readonly BindingFlags FieldBindingFlags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

        public static void ConstructLifecycleObjects(object target)
        {
            var fields = target.GetType().GetFields(FieldBindingFlags);

            foreach (var field in fields)
            {
                if (field.GetCustomAttribute<UnityLifecycleBindAttribute>() == null)
                    continue;

                if (field.GetValue(target) != null)
                    continue;

                var fieldType = field.FieldType;

                ConstructorInfo ctor = null;
                foreach (var constructor in fieldType.GetConstructors())
                {
                    var parameters = constructor.GetParameters();
                    if (parameters.Length == 1 && parameters[0].ParameterType.IsAssignableFrom(target.GetType()))
                    {
                        ctor = constructor;
                        break;
                    }
                }

                if (ctor == null)
                {
                    Debug.LogError($"[UnityLifecycleBind] {fieldType.Name}에 {target.GetType().Name}을 인자로 받는 적합한 생성자가 없음");
                    continue;
                }

                var instance = ctor.Invoke(new[] { target });
                field.SetValue(target, instance);
            }
        }

        public static void Awake(object target) => CallLifecycle(target, lifecycle => lifecycle.Awake());

        public static void OnEnable(object target) => CallLifecycle(target, lifecycle => lifecycle.OnEnable());

        public static void Start(object target) => CallLifecycle(target, lifecycle => lifecycle.Start());

        public static void Update(object target) => CallLifecycle(target, lifecycle => lifecycle.Update());

        public static void LateUpdate(object target) => CallLifecycle(target, lifecycle => lifecycle.LateUpdate());

        public static void FixedUpdate(object target) => CallLifecycle(target, lifecycle => lifecycle.FixedUpdate());

        public static void OnDisable(object target) => CallLifecycle(target, lifecycle => lifecycle.OnDisable());

        public static void OnCollisionEnter(object target, Collision collision) => CallLifecycle(target, lifecycle => lifecycle.OnCollisionEnter(collision));

        public static void OnCollisionExit(object target, Collision collision) => CallLifecycle(target, lifecycle => lifecycle.OnCollisionExit(collision));

        public static void OnTriggerEnter(object target, Collider other) => CallLifecycle(target, lifecycle => lifecycle.OnTriggerEnter(other));

        public static void OnTriggerExit(object target, Collider other) => CallLifecycle(target, lifecycle => lifecycle.OnTriggerExit(other));

        public static void OnApplicationFocus(object target, bool hasFocus) => CallLifecycle(target, lifecycle => lifecycle.OnApplicationFocus(hasFocus));

        public static void OnApplicationPause(object target, bool isPaused) => CallLifecycle(target, lifecycle => lifecycle.OnApplicationPause(isPaused));

        public static void OnApplicationQuit(object target) => CallLifecycle(target, lifecycle => lifecycle.OnApplicationQuit());

        public static void OnDestroy(object target) => CallLifecycle(target, lifecycle => lifecycle.OnDestroy());

        private static void CallLifecycle(object target, Action<IUnityLifecycleAware> action)
        {
            var fields = target.GetType().GetFields(FieldBindingFlags);

            foreach (var field in fields)
            {
                if (field.GetCustomAttribute<UnityLifecycleBindAttribute>() == null)
                    continue;

                if (field.GetValue(target) is IUnityLifecycleAware lifecycle)
                {
                    action(lifecycle);
                }
            }
        }
    }
}
