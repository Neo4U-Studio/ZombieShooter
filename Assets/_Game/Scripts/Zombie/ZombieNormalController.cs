using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using ZSBehaviourTree;

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