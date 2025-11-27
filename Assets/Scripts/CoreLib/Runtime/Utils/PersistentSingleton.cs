using UnityEngine;
using UnityEngine.SceneManagement;

namespace Corelib.Utils
{
    [DefaultExecutionOrder(-100)]
    public abstract class PersistentSingleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<T>();

                    if (_instance == null)
                    {
                        Debug.LogError($"[PersistentSingleton] An instance of {typeof(T)} is needed in the scene, but there is none.");
                    }
                }
                return _instance;
            }
        }

        protected virtual void Awake()
        {
            if (_instance == null)
            {
                _instance = this as T;
                DontDestroyOnLoad(gameObject);
                SceneManager.sceneLoaded += OnSceneLoaded;
                SceneManager.sceneUnloaded += OnSceneUnloaded;
            }
            else if (_instance != this)
            {
                Destroy(gameObject);
            }
        }

        protected virtual void OnDestroy()
        {
            if (_instance == this)
            {
                SceneManager.sceneLoaded -= OnSceneLoaded;
                SceneManager.sceneUnloaded -= OnSceneUnloaded;
            }
        }

        protected virtual void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
        }

        protected virtual void OnSceneUnloaded(Scene scene)
        {
        }
    }
}