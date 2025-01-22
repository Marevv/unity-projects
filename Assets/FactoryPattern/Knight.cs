using UnityEngine;


namespace FactoryPattern
{
    public class Knight : MonoBehaviour
    {
        [SerializeField] WeaponFactory weaponFactory;
        IWeapon weapon = IWeapon.CreateDefault();


        void Start()
        {
            if (weaponFactory != null)
                weapon = weaponFactory.CreateWeapon();

            Attack();
        }

        public void Attack() => weapon?.Attack();
    }

}