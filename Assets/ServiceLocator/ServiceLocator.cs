using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace ServiceLocator
{

    public class ServiceLocator : MonoBehaviour
    {
        static ServiceLocator global;
        static Dictionary<Scene, ServiceLocator> sceneContainers;

        static List<GameObject> tmpSceneObjects;

        readonly ServiceManager services = new ServiceManager();

        const string k_globalServiceLocatorName = "ServiceLocator [Global]";
        const string k_sceneServiceLocatorName = "ServiceLocator [Scene]";


        internal void ConfigureAsGlobal(bool dontDestroyOnLoad)
        {
            if (global == this)
            {
                Debug.LogWarning("Service.ConfigureAsGlobal: Already configured as global.", this);
            }
            else if (global != null)
            {
                Debug.LogError("Service.ConfigureAsGlobal: Another ServiceLocator is already configured as global.", this);
            }
            else
            {
                global = this;
                if (dontDestroyOnLoad) DontDestroyOnLoad(gameObject);
            }
        }

        internal void ConfigureForScene()
        {
            Scene scene = gameObject.scene;
            if (sceneContainers.ContainsKey(scene))
            {
                Debug.LogError("ServiceLocator.ConfigureForScene: Another ServiceLocator is already configured for this scene", this);
                return;
            }

            sceneContainers.Add(scene, this);

        }

        public static ServiceLocator Global
        {
            get
            {
                if (global != null) return global;


                if (FindFirstObjectByType<ServiceLocatorGlobalBootstrapper>() is { } found)
                {
                    found.BootstrapOnDemand();
                    return global;
                }

                var container = new GameObject(k_globalServiceLocatorName, typeof(ServiceLocator));
                container.AddComponent<ServiceLocatorGlobalBootstrapper>().BootstrapOnDemand();

                return global;
            }
        }



        public static ServiceLocator For(MonoBehaviour mb)
        {
            return mb.GetComponentInParent<ServiceLocator>() == null ? ForSceneOf(mb) : Global;
        }

        public static ServiceLocator ForSceneOf(MonoBehaviour mb)
        {
            Scene scene = mb.gameObject.scene;

            if (sceneContainers.TryGetValue(scene, out ServiceLocator container) && container != mb)
            {
                return container;
            }

            tmpSceneObjects.Clear();

            scene.GetRootGameObjects(tmpSceneObjects);

            foreach (GameObject go in tmpSceneObjects.Where(go => go.GetComponent<ServiceLocatorSceneBootstrapper>() != null))
            {
                if (go.TryGetComponent(out ServiceLocatorSceneBootstrapper bootstrapper) && bootstrapper.Container != mb)
                {
                    bootstrapper.BootstrapOnDemand();
                    return bootstrapper.Container;
                }
            }

            return Global;

        }

        public ServiceLocator Register<T>(T service)
        {
            services.Register(service);
            return this;
        }

        public ServiceLocator Register(Type type, object service)
        {
            services.Register(type, service);
            return this;
        }

        public ServiceLocator Get<T>(out T service) where T : class
        {
            if (TryGetService(out service)) return this;

            if (TryGetNextInHierarchy(out ServiceLocator container))
            {
                container.Get(out service);
                return this;
            }

            throw new ArgumentException($"ServiceLocator.Get: Service of type {typeof(T).FullName} not registered.");
        }



        bool TryGetService<T>(out T service) where T : class
        {
            return services.TryGet(out service);
        }

        bool TryGetNextInHierarchy(out ServiceLocator container)
        {
            if (this == global)
            {
                container = null;
                return false;
            }

            container = transform.parent != null ? transform.parent.GetComponentInParent<ServiceLocator>() != null ? transform.parent.GetComponentInParent<ServiceLocator>() : null : null;
            return container != null;
        }

        private void OnDestroy()
        {
            if (this == global)
            {
                global = null;
            }
            else if (sceneContainers.ContainsValue(this))
            {
                sceneContainers.Remove(gameObject.scene);
            }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void ResetStatics()
        {
            global = null;
            sceneContainers = new Dictionary<Scene, ServiceLocator>();
            tmpSceneObjects = new List<GameObject>();
        }


#if UNITY_EDITOR
        [MenuItem("GameObject/ServiceLocator/Add Global")]
        static void AddGlobal()
        {
            var go = new GameObject(k_globalServiceLocatorName, typeof(ServiceLocatorGlobalBootstrapper));
        }

        [MenuItem("GameObject/ServiceLocator/Add Scene")]
        static void AddScene()
        {
            var go = new GameObject(k_sceneServiceLocatorName, typeof(ServiceLocatorSceneBootstrapper));
        }
#endif

    }

    public interface ILocalization
    {
        string GetLocalizedWord(string key);
    }

    public class MockLocalization : ILocalization
    {
        readonly List<string> words = new List<string>() { "pies", "kot", "ryba", "samochód", "dom" };
        readonly System.Random random = new System.Random();

        public string GetLocalizedWord(string key)
        {
            return words[random.Next(words.Count)];
        }
    }

    public interface ISerializer
    {
        void Seralize();
    }

    public class MockSerializer : ISerializer
    {
        public void Seralize()
        {
            Debug.Log("MockSerializer.Serialize");
        }
    }

    public interface IAudioService
    {
        void Play();
    }

    public class MockAudioService : IAudioService
    {
        public void Play()
        {
            Debug.Log("MockAudioService.Play");
        }
    }

    public interface IGameService
    {
        void StartGame();
    }

    public class MockMapService : IGameService
    {
        public void StartGame()
        {
            Debug.Log("MockMapService.StartGame");
        }
    }
}