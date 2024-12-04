using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ZombieShooter;

namespace ZSBehaviourTree
{
    public class BehaviourTreeNodeZombieAction : BehaviourTreeNodeTask
    {
        public eZombieAction ZombieAction = eZombieAction.Idle;
        public string TargetKey = Utilities.PLAYER_TAG;
        
        private string ZombieKey = Utilities.ZOMBIE_TAG;

        private ZombieController zombie = null;
        private ZSPlayerController player = null;
        private GameObject target = null;

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
            base.OnStart();
        }

        protected override State OnUpdate()
        {
            if (zombie)
            {
                zombie.TurnOffTriggers();
                switch (ZombieAction)
                {
                    case eZombieAction.Idle:
                        zombie.ResetAgent();
                        zombie.TriggerIdle();
                        break;
                    case eZombieAction.Wander:
                        zombie.TriggerWander();
                        break;
                    case eZombieAction.Chase:
                        if (target)
                        {
                            zombie.TriggerChase(target);
                            return State.Success;
                        }
                        else
                        {
                            return State.Failure;
                        }
                    case eZombieAction.Attack:
                        if (target)
                        {
                            zombie.TriggerAttack(target);
                            return State.Success;
                        }
                        else
                        {
                            return State.Failure;
                        }
                    case eZombieAction.Dead:
                        zombie.TriggerDead();
                        break;
                }
                return State.Success;
            }
            else
            {
                return State.Failure;
            }
        }
    }
}