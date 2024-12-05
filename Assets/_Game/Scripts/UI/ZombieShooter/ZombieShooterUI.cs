using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZombieShooter
{
    public class ZombieShooterUI : MenuScene
    {
        public static ZombieShooterUI Instance;

        [SerializeField] ZSStatusUI statusUI;
        [SerializeField] ZSGunClipUI gunClipUIPrefab;
        [SerializeField] Transform gunClipUIContainer;
        [SerializeField] ZSRadarUI radarUI;
        [SerializeField] ZSCompassUI compassUI;
        [SerializeField] ZSCrosshairUI crosshairUI;

        public ZSStatusUI PlayerStatus => statusUI;
        public ZSCompassUI Compass => compassUI;
        public ZSCrosshairUI Crosshair => crosshairUI;

        private ZSPlayerController playControl = null;

        public override void Awake() {
            Instance = this;
            base.Awake();
        }

        public override MenuType GetMenuType()
        {
            return MenuType.ZOMBIE_SHOOTER_MAIN;
        }

        public void Initialize(ZSPlayerController player)
        {
            ClearGunClipSlots();
            playControl = player;
            radarUI.SetPlayer(playControl.transform);
            compassUI.SetPlayer(playControl.gameObject);
            crosshairUI.Initialize();
        }

        public void ToggleUI(bool toggle)
        {
            radarUI.ToggleRadar(toggle);
            compassUI.ToggleCompass(toggle);
            crosshairUI.ToggleCrosshair(toggle);
        }

        public ZSGunClipUI CreateGunClipUI()
        {
            Debug.Log("-- Create gun clip");
            return Instantiate(gunClipUIPrefab, gunClipUIContainer);
        }

        private void ClearGunClipSlots()
        {
            for (int i = gunClipUIContainer.childCount - 1; i >= 0; i--)
            {
                Destroy(gunClipUIContainer.GetChild(i).gameObject);
            }
        }
    }
}