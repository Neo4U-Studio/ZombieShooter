using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ZombieShooter
{
    public class ZSBossHpUI : MonoBehaviour
    {
        [SerializeField] Slider healthBar;

        public void UpdateHealth(int value, int maxValue)
        {
            var healthCurrent = Mathf.Clamp(value, 0, maxValue);
            healthBar.value = healthCurrent / (float)maxValue;
        }

        public void ToggleBossHp(bool toggle)
        {
            this.gameObject.SetActive(toggle);
        }
    }
}