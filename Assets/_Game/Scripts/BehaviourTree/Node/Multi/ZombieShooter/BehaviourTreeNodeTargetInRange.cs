using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZombieShooter;

namespace ZSBehaviourTree
{
    public class BehaviourTreeNodeTargetInRange : BehaviourTreeNodeTrueFalse
    {
        [Tooltip("Key data to find target")]
        public string TargetKey = Utilities.PLAYER_TAG;

        [Tooltip("Key data to find range")]
        public string CheckRangeKey = Utilities.ZOMBIE_DETECT_RANGE_KEY;

        [Tooltip("Default range")]
        public float Range = 10f;

        private string ZombieKey = Utilities.ZOMBIE_TAG;
        private ZombieController zombie = null;
        private ZSPlayerController player = null;
        private GameObject target = null;
        private float currentRangeCheck = -1;

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
            
            if (!target)
            {
                var targetData = GetData(TargetKey);
                if (targetData != null)
                {
                    switch (targetData)
                    {
                        case ZSPlayerController:
                            player = targetData as ZSPlayerController;
                            target = player.gameObject;
                            break;
                        case GameObject:
                            target = targetData as GameObject;
                            break;
                    }
                }
            }

            if (currentRangeCheck < 0)
            {
                var rangeData = GetData(CheckRangeKey);
                if (rangeData != null)
                {
                    currentRangeCheck = (float)rangeData;
                }
                else
                {
                    currentRangeCheck = Range;
                }
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
            if (player && player.IsDead)
            {
                Result = false;
                return;
            }
            Result = Vector3.Distance(zombie.transform.position, target.transform.position) <= currentRangeCheck;
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