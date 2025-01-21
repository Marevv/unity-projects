using UnityEngine;


namespace DependencyInjection
{

    public interface IEnvironmentSystem
    {
        IEnvironmentSystem ProvideEnvironmentSystem();
        void Initialize();
    }


    public class EnvironmentSystem : MonoBehaviour, IDependencyProvider, IEnvironmentSystem
    {
        [Provide]
        public IEnvironmentSystem ProvideEnvironmentSystem()
        {
            return this;
        }

        public void Initialize()
        {
            Debug.Log($"{this.name}.Initialize()");
        }
    }

    public class SummerSystem : EnvironmentSystem
    {

    }
}

