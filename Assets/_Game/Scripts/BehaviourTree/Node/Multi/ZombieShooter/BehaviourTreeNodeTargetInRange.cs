using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZombieShooter;

namespace ZSBehaviourTree
{
    public class BehaviourTreeNodeTargetInRange : BehaviourTreeNodeTrueFalse
    {
        public string TargetKey = Utilities.PLAYER_TAG;
        public float Range = 10f;

        private string ZombieKey = Utilities.ZOMBIE_TAG;
        

        private ZombieController zombie = null;
        private GameObject target = null;

        protected override void OnStart()
        {
            var zombieData = GetData(ZombieKey) as ZombieController;
            if (zombieData != null)
            {
                zombie = zombieData;
            }

            var targetData = GetData(TargetKey) as GameObject;
            if (targetData != null)
            {
                target = targetData;
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
            Result = Vector3.Distance(zombie.transform.position, target.transform.position) <= Range;
        }

        protected override State OnUpdate()
        {
            if (zombie && target)
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