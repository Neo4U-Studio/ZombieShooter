using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZombieShooter
{
    public class ZombieNormalController : ZombieController
    {
        protected override eZombieType GetZombieType()
        {
            return eZombieType.Zombie_Normal;
        }

        public override void DamagePlayer()
        {
            if (DistanceToPlayer() <= attackRange)
            {
                base.DamagePlayer();
            }
        }
    }
}