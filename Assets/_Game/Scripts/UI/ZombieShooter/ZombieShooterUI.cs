using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZombieShooter
{
    public class ZombieShooterUI : MonoSingleton<ZombieShooterUI>
    {
        [SerializeField] ZSStatusUI statusUI;
        [SerializeField] ZSGunClipUI gunClipUI;
        [SerializeField] ZSRadarUI radarUI;
        [SerializeField] ZSCompassUI compassUI;

        public ZSStatusUI PlayStatus => statusUI;
        public ZSCompassUI Compass => compassUI;

        private PlayerController playControl = null;

        public void Initialize(PlayerController player)
        {
            playControl = player;
            radarUI.SetPlayer(playControl.transform);
            compassUI.SetPlayer(playControl.gameObject);
        }

        public void ToggleUI(bool toggle)
        {
            radarUI.ToggleRadar(toggle);
            compassUI.ToggleCompass(toggle);
        }

        public void SetGunClipValue(int ammo, int clip)
        {
            gunClipUI.SetAmmo(ammo, clip);
        }
    }
}