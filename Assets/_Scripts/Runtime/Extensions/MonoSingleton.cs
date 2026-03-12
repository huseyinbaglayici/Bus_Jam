using UnityEngine;

namespace _Scripts.Runtime.Extensions
{
    public static class ApplicationState
    {
        public static bool IsQuitting { get; private set; }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Reset() => IsQuitting = false; // Editor reset

        public static void SetQuitting() => IsQuitting = true;
    }

    public class MonoSingleton<T> : MonoBehaviour where T : Component
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (ApplicationState.IsQuitting) return null;
                if (_instance != null) return _instance;

                _instance = FindObjectOfType<T>();
                if (_instance != null) return _instance;

                _instance = new GameObject(typeof(T).Name).AddComponent<T>();
                return _instance;
            }
        }

        public static bool IsAvailable => _instance != null && !ApplicationState.IsQuitting;

        protected virtual void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this as T;
        }

        protected virtual void OnDestroy()
        {
            if (_instance == this) _instance = null;
        }

        private void OnApplicationQuit() => ApplicationState.SetQuitting();
    }
}