using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pooling;

namespace ZombieShooter
{
    public class ZombieArcherControllerr : ZombieController
    {
#if UNITY_EDITOR
        public GameHeader headerEditor5 = new GameHeader() { header = "Archer" };
#endif
        [SerializeField] protected GameObject bulletPrefab;

        protected override eZombieType GetZombieType()
        {
            return eZombieType.Zombie_Archer;
        }

        public override bool CanAttack()
        {
            if (target)
            {
                RaycastHit hitInfo;
                var shotDirection = target.transform.position - this.transform.position;
                shotDirection.Normalize();
                if (Physics.Raycast(this.transform.position, shotDirection, out hitInfo, 200))
                {
                    GameObject hitObj = hitInfo.collider.gameObject;
                    if (hitObj.CompareTag(Utilities.PLAYER_TAG))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public override void DamagePlayer()
        {
            if (target && bulletPrefab)
            {
                var spawnPos = this.transform.position + Vector3.up * 1f + this.transform.transform.forward * 1f;
                var bullet = bulletPrefab.Spawn(spawnPos, Quaternion.identity).GetComponent<ProjectileController>();
                bullet.Fire(target.transform.position, CheckBulletHitTarget);
            }
        }

        private void CheckBulletHitTarget(GameObject obj)
        {
            if (obj.CompareTag(Utilities.PLAYER_TAG))
            {
                base.DamagePlayer();
            }
        }
    }
}