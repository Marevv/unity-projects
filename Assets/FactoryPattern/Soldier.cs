using UnityEngine;


namespace FactoryPattern
{
    public class Soldier : MonoBehaviour
    {
        [SerializeField] EquipmentFactory equipmentFactory;


        IWeapon weapon;
        IShield shield;


        private void Start()
        {
            weapon = equipmentFactory.CreateWeapon();
            shield = equipmentFactory.CreateShield();

            Attack();
            Defend();
        }

        public void Attack() => weapon.Attack();
        public void Defend() => shield.Defend();
    }
}
