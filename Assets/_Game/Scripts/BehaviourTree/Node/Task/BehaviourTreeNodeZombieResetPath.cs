using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZombieShooter;

namespace ZSBehaviourTree
{
    public class BehaviourTreeNodeZombieResetPath : BehaviourTreeNodeTask
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

        protected override State OnUpdate()
        {
            if (zombie)
            {
                zombie.ResetAgent();
                return State.Success;
            }
            else
            {
                return State.Failure;
            }
        }
    }
}