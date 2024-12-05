using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pooling;
using DG.Tweening;
using AudioPlayer;
using Unity.VisualScripting;

namespace ZombieShooter
{
    public abstract class ZSWeaponController : MonoBehaviour
    {
#if UNITY_EDITOR
        public GameHeader headerEditor = new GameHeader() { header = "General" };
#endif
        [SerializeField] protected Sprite iconUI;

        [Tooltip("Set -1 for infinity ammo")]
        [SerializeField] protected int maxAmmo = 50;
        [SerializeField] protected int maxClipAmmo = 50;
        [SerializeField] protected float fireRate = 0.5f;
        [SerializeField] protected int damage = 5;
        [SerializeField] protected GameObject shootVfxPrefab;

        public eWeaponType Type => GetWeaponType();
        [HideInInspector] public bool IsShooting = false;
        
        protected int remainingAmmo;
        protected int currentAmmoInClip;
        protected Transform shotContainer;
        protected float countdownShot = 0.5f;

        protected abstract eWeaponType GetWeaponType();

        protected ZSGunClipUI uiSlot = null;

        public virtual void Initialize(Transform shotContainer)
        {
            uiSlot = ZombieShooterUI.Instance?.CreateGunClipUI();
            this.shotContainer = shotContainer;
            IsShooting = false;
            FillAmmo();
            uiSlot?.SetIcon(iconUI);
            uiSlot?.ToggleSelect(false);
        }

        public virtual void FillAmmo()
        {
            remainingAmmo = maxAmmo;
            currentAmmoInClip = maxClipAmmo;
            uiSlot?.SetAmmo(currentAmmoInClip, maxClipAmmo);
            uiSlot?.SetRemainingAmmo(remainingAmmo);
        }

        public void IncreaseAmmo(int value)
        {
            if (remainingAmmo >= 0)
            {
                remainingAmmo = Mathf.Clamp(remainingAmmo + value, 0, maxAmmo);
            }
            uiSlot?.SetRemainingAmmo(remainingAmmo);
        }

        public void DecreaseAmmo(int value)
        {
            if (remainingAmmo >= 0)
            {
                remainingAmmo = Mathf.Clamp(remainingAmmo - value, 0, maxAmmo);
            }
            uiSlot?.SetRemainingAmmo(remainingAmmo);
        }

        public void ConsumeAmmo()
        {
            if (currentAmmoInClip > 0)
            {
                currentAmmoInClip--;
            }
            uiSlot?.SetAmmo(currentAmmoInClip, maxClipAmmo);
        }

        public bool TryFillAmmoClip()
        {
            if (maxClipAmmo < 0 
                || currentAmmoInClip < 0
                || currentAmmoInClip >= maxClipAmmo
                || remainingAmmo == 0)
            {
                return false;
            }
            else
            {
                if (remainingAmmo < 0) // Inf ammo
                {
                    currentAmmoInClip = maxClipAmmo;
                }
                else
                {
                    int amount = maxClipAmmo - currentAmmoInClip;
                    amount = Mathf.Clamp(amount, 0, remainingAmmo);
                    DecreaseAmmo(amount);
                    currentAmmoInClip += amount;
                }
                uiSlot?.SetAmmo(currentAmmoInClip, maxClipAmmo);
                return true;
            }
        }

        protected void ResetConutdown()
        {
            countdownShot = fireRate;
        }

        public bool IsShootAvailable()
        {
            return currentAmmoInClip != 0;
        }

        public virtual void OnWeaponSwitched()
        {
            IsShooting = false;
            uiSlot?.ToggleSelect(false);
        }

        public virtual void OnWeaponActive()
        {
            uiSlot?.ToggleSelect(true);
        }

        private void Update() {
            if (IsShooting && IsShootAvailable())
            {
                countdownShot -= Time.deltaTime;
                if (countdownShot <= 0)
                {
                    HandleShoot();
                    ResetConutdown();
                }
            }
            else
            {
                ResetConutdown();
            }
        }

        protected virtual void HandleShoot()
        {
            // Subclass
            ConsumeAmmo();
            PlayShootVfx(shotContainer);
            PlayShootSfx();
        }

#region Vfx/Sfx
        private void PlayShootVfx(Transform parent)
        {
            if (shootVfxPrefab)
            {
                var vfx = shootVfxPrefab.Spawn(parent.position, parent.rotation, parent);
                DOVirtual.DelayedCall(1f, () => vfx.Despawn());
            }
        }

        protected virtual void PlayShootSfx()
        {
            SoundManager.Instance?.PlaySound(SoundID.SFX_ZS_PLAYER_SHOT);
        }
#endregion
    }
}