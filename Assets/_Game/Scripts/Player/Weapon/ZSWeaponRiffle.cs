using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Pooling;

namespace ZombieShooter
{
    public class ZSWeaponRiffle : ZSWeaponController
    {
#if UNITY_EDITOR
        public GameHeader headerEditor1 = new GameHeader() { header = "Riffle" };
#endif
        [SerializeField] private GameObject bloodVfxPrefab;

        private Camera mainCamera;

        public override void Initialize(Transform shotContainer)
        {
            mainCamera = CameraManager.Instance.GetOverlayCamera(eOverlayCamera.MAIN);
            base.Initialize(shotContainer);
        }

        protected override eWeaponType GetWeaponType()
        {
            return eWeaponType.Riffle;
        }

        protected override void HandleShoot()
        {
            base.HandleShoot();
            RaycastHit hitInfo;
            var shotPosition = mainCamera.ViewportToWorldPoint(Vector3.one * 0.5f);
            var shotDirection = mainCamera.transform.forward;
            if (Physics.Raycast(shotPosition, shotDirection, out hitInfo, 200))
            {
                GameObject hitObj = hitInfo.collider.gameObject;
                if (hitObj.CompareTag(Utilities.ZOMBIE_TAG))
                {
                    PlayZombieBloodVfx(hitInfo);
                    // ZSGameStats.ON_KILL_ZOMBIE?.Invoke();
                    var zombie = hitObj.GetComponent<ZombieController>();
                    zombie.DamageToZombie(damage, shotDirection, 10000, UnityEngine.Random.Range(0, 10) <= 5);
                }
            }
        }

#region vfx
        private void PlayZombieBloodVfx(RaycastHit hitInfo)
        {
            if (bloodVfxPrefab)
            {
                GameObject blood = bloodVfxPrefab.Spawn(hitInfo.point, Quaternion.identity);
                blood.transform.LookAt(this.transform.position);
                DOVirtual.DelayedCall(1f, () => blood.Despawn());
            }
        }
#endregion
    }
}