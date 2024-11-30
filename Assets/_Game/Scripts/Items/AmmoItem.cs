using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AudioPlayer;

namespace ZombieShooter
{
    public class AmmoItem : ShooterItem
    {
        public int Value => Mathf.FloorToInt(this.value);
        protected override eItemType GetItemType() { return eItemType.Ammo_Normal; }

        public override void PlayItemConsumeSound()
        { 
            SoundManager.Instance?.PlaySound(SoundID.SFX_ZS_ITEM_AMMO);
        }
    }
}