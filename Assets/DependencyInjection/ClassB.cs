using UnityEngine;


namespace DependencyInjection
{
    public class ClassB : MonoBehaviour
    {
        [Inject] ServiceA serviceA;
        [Inject] ServiceB serviceB;
        FactoryA factoryA;

        [Inject]
        public void Init(FactoryA factoryA)
        {
            this.factoryA = factoryA;
        }

        private void Start()
        {
            serviceA.Initialize("ServiceA initialized from ClassB");
            serviceB.Initialize("ServiceB initialized from ClassB");
            factoryA.CreateServiceA().Initialize("Service A initialized from FactoryA");
        }
    }
}

