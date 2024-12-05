using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZombieShooter;

namespace ZSBehaviourTree
{
    public class BehaviourTreeNodeZombieCanAttack : BehaviourTreeNodeTrueFalse
    {
        private string ZombieKey = Utilities.ZOMBIE_TAG;
        private ZombieController zombie = null;

        protected override void OnStart()
        {
            if (!zombie)
            {
                var zombieData = GetData(ZombieKey) as ZombieController;
                if (zombieData != null)
                {
                    zombie = zombieData;
                }
            }
            base.OnStart();
        }

        protected override void CheckResult()
        {
            if (zombie)
            {
                Result = zombie.CanAttack();
            }
            else
            {
                Result = false;
            }
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