using System;
using UnityEngine;


namespace DependencyInjection
{
    public class Singleton<T> : MonoBehaviour where T : Component {
        protected static T instance;
        public static bool HashInstance => instance != null;
        public static T TryGetInstance() => HashInstance ? instance : null;
        public static T Current => instance;

        public static T Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindFirstObjectByType<T>();
                    if (instance == null)
                    {
                        GameObject obj = new GameObject();
                        obj.name = typeof(T).Name + "AutoCreated";
                        instance = obj.AddComponent<T>();
                    }
                }

                return instance;
            }
        }

        protected virtual void Awake() => InitializeSingletion();

        protected virtual void InitializeSingletion()
        {
            if (!Application.isPlaying)
            {
                return;
            }

            instance = this as T;
        }
    }
}