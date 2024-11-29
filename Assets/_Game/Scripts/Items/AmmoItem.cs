using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZombieShooter
{
    public class AmmoItem : ShooterItem
    {
        protected override eItemType GetItemType() { return eItemType.Ammo_Normal; }
    }
}