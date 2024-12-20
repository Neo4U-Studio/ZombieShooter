using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AudioPlayer;

namespace ZombieShooter
{
    public class MedKitItem : ShooterItem
    {
        public int Value => Mathf.FloorToInt(this.value);
        protected override eItemType GetItemType() { return eItemType.MedKit; }

        public override void PlayItemConsumeSound()
        {
            SoundManager.Instance?.PlaySound(SoundID.SFX_ZS_ITEM_MEDKIT);
        }
    }
}