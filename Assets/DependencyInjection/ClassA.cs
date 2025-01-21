using UnityEngine;


namespace DependencyInjection
{
    public class ClassA : MonoBehaviour { 

        [Inject] ServiceA serviceA;

        [Inject]
        public void Init(ServiceA serviceA)
        {
            this.serviceA = serviceA;
        }

        [Inject] IEnvironmentSystem environmentSystem;

        //[Inject] SummerSystem SummerSystem { get; set; }



        private void Start()
        {
            serviceA.Initialize("ServiceA initialized from ClassA");
            environmentSystem.Initialize();
            //SummerSystem.Initialize();
        }
    }
}