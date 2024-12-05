using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pooling;

namespace ZombieShooter
{
    public class ZSWeaponLauncher : ZSWeaponController
    {
#if UNITY_EDITOR
        public GameHeader headerEditor1 = new GameHeader() { header = "Launcher" };
#endif
        [SerializeField] protected GameObject bulletPrefab;

        private Camera mainCamera;

        protected override eWeaponType GetWeaponType()
        {
            return eWeaponType.Launcher;
        }

        public override void Initialize(Transform shotContainer)
        {
            mainCamera = CameraManager.Instance.GetOverlayCamera(eOverlayCamera.MAIN);
            base.Initialize(shotContainer);
        }

        protected override void HandleShoot()
        {
            base.HandleShoot();
            if (bulletPrefab)
            {
                var shotDirection = mainCamera.transform.forward;
                var bullet = bulletPrefab.Spawn(this.shotContainer.position, Quaternion.identity).GetComponent<LaucherBullet>();
                bullet.Fire(this.shotContainer.position + shotDirection * 1f);
                bullet.OnLaucherBulletExplode -= HandleExplode;
                bullet.OnLaucherBulletExplode += HandleExplode;
            }
        }

        private void HandleExplode(Collider[] colliders, Vector3 explodePos, float force)
        {
            foreach (Collider nearbyObject in colliders)
            {
                if (nearbyObject.gameObject.CompareTag(Utilities.ZOMBIE_TAG))
                {
                    var zombie = nearbyObject.GetComponent<ZombieController>();
                    zombie.DamageToZombie(damage, (nearbyObject.transform.position - explodePos).normalized, force, true);
                }
            }
        }
    }
}