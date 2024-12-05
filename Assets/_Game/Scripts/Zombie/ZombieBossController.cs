using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pooling;
using UnityEngine.AI;
using ZSBehaviourTree;

namespace ZombieShooter
{
    public class ZombieBossController : ZombieController
    {
        public enum eZombieBossAttack
        {
            Normal,
            Shoot
        }
#if UNITY_EDITOR
        public GameHeader headerEditor5 = new GameHeader() { header = "Boss" };
#endif
        [SerializeField] protected float shootRange = 10;
        [SerializeField] protected GameObject bulletPrefab;
        [SerializeField] protected int numberOfShootBullet = 3;
        [SerializeField] protected float timeBetweenShoot = 0.2f;
        [SerializeField] protected float shotDamageAmount = 10;

        private eZombieBossAttack attackType;

        protected override eZombieType GetZombieType()
        {
            return eZombieType.Zombie_Boss;
        }

        protected override bool IsAllowUseRagdoll()
        {
            return false;
        }

        protected override void AssignBehaviourTreeData(BehaviourTree tree)
        {
            base.AssignBehaviourTreeData(tree);
            behaviourTree.BindData(Utilities.ZOMBIE_SHOOT_RANGE_KEY, shootRange);
        }

        public override void StartZombie()
        {
            base.StartZombie();
            ZombieShooterUI.Instance?.UpdateBossHp(currentHealth, health);
        }

        private void Update() {
            if (ZombieShooterUI.Instance)
            {
                if (!IsDead && DistanceToPlayer() <= dectectRange)
                {
                    ZombieShooterUI.Instance.ToggleBossHp(true);
                    ZombieShooterUI.Instance.UpdateBossHp(currentHealth, health);
                }
                else
                {
                    ZombieShooterUI.Instance.ToggleBossHp(false);
                }
            }
        }

        public override void DamagePlayer()
        {
            if (target)
            {
                switch (attackType)
                {
                    case eZombieBossAttack.Normal:
                        if (DistanceToPlayer() <= attackRange)
                        {
                            currentDamageAmount = damageAmount;
                            base.DamagePlayer();
                        }
                        break;
                    case eZombieBossAttack.Shoot:
                        ShootBullet();
                        break;
                }
            }
            
        }

        public override void TriggerAttack(GameObject target) // Normal attack
        {
            // Debug.Log("-- Trigger attack");
            attackType = eZombieBossAttack.Normal;
            base.TriggerAttack(target);
        }

        public void TriggerShootAttack(GameObject target) // Shoot attack
        {
            // Debug.Log("-- Trigger shoot");
            attackType = eZombieBossAttack.Shoot;
            base.TriggerAttack(target);
        }

        public void TriggerDash()
        {
            // Debug.Log("-- Trigger dashh111");
            if (target)
            {
                // Debug.Log("-- Trigger dashh2222");
                Vector3 randomPoint = GetRandomPointInDirection(this.transform,
                    target.transform.position - this.transform.position, 5, 10);
                NavMeshHit hit;
                if (NavMesh.SamplePosition(randomPoint, out hit, 10.0f, NavMesh.AllAreas))
                {
                    float newX = hit.position.x;
                    float newZ = hit.position.z;
                    float newY = Terrain.activeTerrain.SampleHeight(new Vector3(newX, 0, newZ));
                    Vector3 dest = new Vector3(newX, newY, newZ);
                    agent.SetDestination(dest);
                    agent.stoppingDistance = 0;
                    TurnOffTriggers();
                    agent.speed = 20;
                    anim.SetBool(HashAnimatorRun, true);
                }
            }
        }

        Vector3 GetRandomPointInDirection(Transform target, Vector3 direction, float minDist, float maxDist)
        {
            Vector3 normalizedDirection = direction.normalized;
            float distance = Random.Range(minDist, maxDist);
            Vector3 randomPoint = target.position + normalizedDirection * distance;

            return randomPoint;
        }

        private void ShootBullet()
        {
            if (bulletPrefab)
            {
                StartCoroutine(ShootBullet_Cor());
            }
        }

        IEnumerator ShootBullet_Cor()
        {
            for (int i = 0; i < numberOfShootBullet; i++)
            {
                var spawnPos = this.transform.position + Vector3.up * 1f + this.transform.transform.forward * 1f;
                var bullet = bulletPrefab.Spawn(spawnPos, Quaternion.identity).GetComponent<ProjectileController>();
                bullet.Fire(target.transform.position, CheckBulletHitTarget);
                yield return DelayUtils.Wait(timeBetweenShoot);
            }
        }

        private void CheckBulletHitTarget(GameObject obj)
        {
            if (obj.CompareTag(Utilities.PLAYER_TAG))
            {
                currentDamageAmount = shotDamageAmount;
                base.DamagePlayer();
            }
        }
    }
}