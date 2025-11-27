using UnityEngine;

namespace Corelib.Utils
{
    public class ScriptableModelConfig<T> : ScriptableObject
    {
        [SerializeField]
        public T template;
        public T Get() => template;
    }
}