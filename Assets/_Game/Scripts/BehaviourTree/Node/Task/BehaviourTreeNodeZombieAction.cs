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
                        return HandleChase();
                    case eZombieAction.Attack:
                        return HandleAttack();
                    case eZombieAction.Dead:
                        zombie.TriggerDead();
                        break;
                    case eZombieAction.WanderRun:
                        zombie.TriggerWanderRun();
                        break;
                    case eZombieAction.Shoot:
                        return HandleShoot();
                    case eZombieAction.Dash:
                        return HandleDash();
                }
                return State.Success;
            }
            else
            {
                return State.Failure;
            }
        }

        private State HandleChase()
        {
            if (target)
            {
                zombie.TriggerChase(target);
                return State.Success;
            }
            else
            {
                return State.Failure;
            }
        }

        private State HandleAttack()
        {
            if (target)
            {
                zombie.TriggerAttack(target);
                return State.Success;
            }
            else
            {
                return State.Failure;
            }
        }

        private State HandleShoot()
        {
            if (target)
            {
                var boss = zombie as ZombieBossController;
                if (boss)
                {
                    boss.TriggerShootAttack(target);
                    return State.Success;
                }
                return State.Failure;
            }
            else
            {
                return State.Failure;
            }
        }

        private State HandleDash()
        {
            if (target)
            {
                var boss = zombie as ZombieBossController;
                if (boss)
                {
                    boss.TriggerDash();
                    return State.Success;
                }
                return State.Failure;
            }
            else
            {
                return State.Failure;
            }
        }
    }
}