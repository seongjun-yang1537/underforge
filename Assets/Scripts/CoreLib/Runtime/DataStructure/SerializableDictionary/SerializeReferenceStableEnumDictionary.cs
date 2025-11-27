using System;
using System.Collections.Generic;
using UnityEngine;

namespace Corelib.Utils
{
    [Serializable]
    public class SerializeReferenceStableEnumDictionary<TKey, TValue> : SerializableDictionaryBase<string, TValue, SerializeReferenceStorage<TValue>> where TKey : Enum
    {
        static string KeyToString(TKey key) => Enum.GetName(typeof(TKey), key);

        public TValue this[TKey key]
        {
            get => base[KeyToString(key)];
            set => base[KeyToString(key)] = value;
        }

        public bool ContainsKey(TKey key) => base.ContainsKey(KeyToString(key));

        public void Add(TKey key, TValue value) => base.Add(KeyToString(key), value);

        public bool Remove(TKey key) => base.Remove(KeyToString(key));

        public bool TryGetValue(TKey key, out TValue value) => base.TryGetValue(KeyToString(key), out value);

        public IEnumerable<KeyValuePair<TKey, TValue>> EnumPairs
        {
            get
            {
                foreach (var kv in this)
                {
                    if (Enum.TryParse(typeof(TKey), kv.Key, out var parsed))
                        yield return new KeyValuePair<TKey, TValue>((TKey)parsed, kv.Value);
                }
            }
        }

        protected override void SetValue(SerializeReferenceStorage<TValue>[] storage, int i, TValue value)
        {
            storage[i] = new SerializeReferenceStorage<TValue>();
            storage[i].data = value;
        }

        protected override TValue GetValue(SerializeReferenceStorage<TValue>[] storage, int i)
        {
            return storage[i].data;
        }
    }

    [Serializable]
    public class SerializeReferenceStorage<T>
    {
        [SerializeReference, SubclassSelector]
        public T data;
    }
}

