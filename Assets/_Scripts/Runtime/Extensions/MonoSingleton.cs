using UnityEngine;

namespace _Scripts.Runtime.Extensions
{
    public class MonoSingleton<T> : MonoBehaviour where T : Component
    {
        private static T _instance;

        public static T Instance
        {
            get
            {
                if (!_instance)
                {
                    _instance = FindObjectOfType<T>();
                    if (!_instance)
                    {
                        GameObject newGo = new GameObject();
                        _instance = newGo.AddComponent<T>();
                    }
                }

                return _instance;
            }
        }

        protected void Awake()
        {
            _instance = this as T;
        }
    }
}