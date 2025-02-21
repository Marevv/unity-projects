using System;
using UnityEngine;

namespace ServiceLocator
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(ServiceLocator))]
    public abstract class Bootstrapper : MonoBehaviour
    {
        ServiceLocator container;
        internal ServiceLocator Container => container == null ? null : container = GetComponent<ServiceLocator>();

        bool hasBeenBootstrapped;

        private void Awake() => BootstrapOnDemand();

        public void BootstrapOnDemand()
        {
            if(hasBeenBootstrapped) return;
            hasBeenBootstrapped = true;
            Bootstrap();
        }

        protected abstract void Bootstrap();
    }

    [AddComponentMenu("ServiceLocator/ServiceLocator Global")]
    public class ServiceLocatorGlobalBootstrapper : Bootstrapper
    {
        [SerializeField] bool dontDestroyOnLoad = true;
        protected override void Bootstrap()
        {
            Container.ConfigureAsGlobal(dontDestroyOnLoad);
        }
    }

    [AddComponentMenu("ServiceLocator/ServiceLocator Scene")]
    public class ServiceLocatorSceneBootstrapper : Bootstrapper
    {
        protected override void Bootstrap()
        {
            Container.ConfigureForScene();
        }
    }
}