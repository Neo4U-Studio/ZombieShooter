using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZombieShooter;

namespace ZSBehaviourTree
{
    public class BehaviourTreeNodeIsZombieDead : BehaviourTreeNodeTrueFalse
    {
        private string ZombieKey = Utilities.ZOMBIE_TAG;
        private ZombieController zombie = null;

        protected override void OnStart()
        {
            var zombieData = GetData(ZombieKey) as ZombieController;

            if (zombieData != null)
            {
                zombie = zombieData;
            }
            base.OnStart();
        }

        protected override void OnStop()
        {
            zombie = null;
            base.OnStop();
        }

        protected override void CheckResult()
        {
            Result = zombie.IsDead;
        }

        protected override State OnUpdate()
        {
            if (zombie)
            {
                return base.OnUpdate();
            }
            else
            {
                return State.Failure;
            }
        }
    }
}