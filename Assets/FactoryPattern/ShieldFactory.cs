using UnityEngine;


namespace FactoryPattern
{

    public interface IShield
    {
        void Defend();
        static IShield CreateDefault()
        {
            return new Shield();
        }
    }

    public class Shield : IShield
    {
        public void Defend()
        {
            Debug.Log("Blocking with the shield!");
        }
    }


    public abstract class ShieldFactory : ScriptableObject
    {
        public abstract IShield CreateShield();
    }

}