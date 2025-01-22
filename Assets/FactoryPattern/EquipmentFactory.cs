using UnityEngine;


namespace FactoryPattern
{

    [CreateAssetMenu(fileName = "EquipmentFactory", menuName = "Equipment Factory")]
    public class EquipmentFactory : ScriptableObject
    {
        public WeaponFactory weaponFactory;
        public ShieldFactory shieldFactory;

        public IWeapon CreateWeapon()
        {
            if(weaponFactory != null)
            {
                return weaponFactory.CreateWeapon();
            }

            return IWeapon.CreateDefault();
        }

        public IShield CreateShield()
        {
            if(shieldFactory != null)
            {
                return shieldFactory.CreateShield();
            }

            return IShield.CreateDefault();
        }
    }

}