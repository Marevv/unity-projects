﻿using UnityEngine;


namespace FactoryPattern
{
    [CreateAssetMenu(fileName = "BowFactory", menuName = "Weapon Factory/Bow")]
    public class BowFactory : WeaponFactory
    {
        public override IWeapon CreateWeapon()
        {
            return new Bow();
        }
    }

}