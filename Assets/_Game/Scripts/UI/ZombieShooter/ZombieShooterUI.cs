using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Pooling;
using DG.Tweening;

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
        [SerializeField] ZSBossHpUI bossHpUI;
        [SerializeField] GameObject splatterPrefab;
        [SerializeField] RectTransform splatterContainer;

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
            bossHpUI.ToggleBossHp(false);
        }

        public void ToggleUI(bool toggle)
        {
            radarUI.ToggleRadar(toggle);
            compassUI.ToggleCompass(toggle);
            crosshairUI.ToggleCrosshair(toggle);
        }

        public void ToggleBossHp(bool toggle)
        {
            bossHpUI.ToggleBossHp(toggle);
            compassUI.ToggleCompass(!toggle);
        }

        public void UpdateBossHp(int value, int maxValue)
        {
            bossHpUI.UpdateHealth(value, maxValue);
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

        public void SpawnRandomSplatter()
        {
            Rect rect = splatterContainer.rect;
            float randomX = Random.Range(rect.xMin, rect.xMax);
            float randomY = Random.Range(rect.yMin, rect.yMax);
            float randomScale = Random.Range(0.6f, 1.2f);
            float randomFade = Random.Range(0.6f, 1f);
            Vector2 randomPosition = new Vector2(randomX, randomY);
            GameObject splatterObj = Instantiate(splatterPrefab, splatterContainer);

            RectTransform splatterRect = splatterObj.transform as RectTransform;
            splatterRect.anchoredPosition = randomPosition;
            splatterRect.localScale = Vector3.one * 0.5f;

            Image splatterImg = splatterObj.GetComponent<Image>();

            Sequence sp = DOTween.Sequence();
            sp.Append(splatterImg.DOFade(0f, 0f));
            sp.Append(splatterImg.DOFade(randomFade, 0.1f));
            sp.Join(splatterObj.transform.DOScale(randomScale, 0.1f));
            sp.Append(splatterImg.DOFade(0f, 2f).SetDelay(0.1f));
            sp.OnComplete(() => {
                Destroy(splatterObj);
            });
        }
    }
}