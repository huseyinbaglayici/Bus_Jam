using UnityEngine;

namespace _Scripts.Runtime.Managers
{
    public class PersistentManager : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
        }
    }
}