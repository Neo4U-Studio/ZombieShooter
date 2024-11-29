using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZombieShooter
{
    public class MedKitItem : ShooterItem
    {
        protected override eItemType GetItemType() { return eItemType.MedKit; }
    }
}