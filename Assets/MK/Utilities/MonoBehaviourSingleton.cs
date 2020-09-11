using UnityEngine;

namespace MK
{
    public class MonoBehaviorSingleton<T> : MonoBehaviour where T : MonoBehaviorSingleton<T>
    {
        static T instance;

        /// <summary>
        /// A Singleton instance of this class.
        /// </summary>
        public static T Instance => instance == null ? instance = FindObjectOfType<T>() : instance;

        /// <summary>
        /// Assigns the Singleton. This should be called in Awake().
        /// </summary>
        protected void RegisterSingleton() => instance = (T)this;
    }
}

