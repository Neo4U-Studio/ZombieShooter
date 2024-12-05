using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AudioPlayer;

namespace ZombieShooter
{
    public class AmmoItem : ShooterItem
    {
        [SerializeField] private eWeaponType ammoType;

        public int Value => Mathf.FloorToInt(this.value);
        public eWeaponType AmmoType => ammoType;


        protected override eItemType GetItemType() { return eItemType.Ammo; }

        public override void PlayItemConsumeSound()
        {
            SoundManager.Instance?.PlaySound(SoundID.SFX_ZS_ITEM_AMMO);
        }
    }
}