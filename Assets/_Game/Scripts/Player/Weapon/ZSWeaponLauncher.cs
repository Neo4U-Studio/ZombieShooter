using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZombieShooter
{
    public class ZSWeaponLauncher : ZSWeaponController
    {
        protected override eWeaponType GetWeaponType()
        {
            return eWeaponType.Launcher;
        }
    }
}