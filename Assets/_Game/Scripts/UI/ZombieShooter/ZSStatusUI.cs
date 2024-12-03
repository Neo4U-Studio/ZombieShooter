using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ZombieShooter
{
    public class ZSStatusUI : MonoBehaviour
    {
        [SerializeField] Slider healthBar;
        [SerializeField] Text healthText;

        [SerializeField] Slider energyBar;
        [SerializeField] Text energyText;

        [SerializeField] Text ammoText;

        int healthMax;
        float healthCurrent;

        int energyMax;
        float energyCurrent;

        int ammoMax;
        int ammoCurrent;

        public void SetupHealth(int maxValue)
        {
            healthMax = maxValue;
            healthCurrent = healthMax;
            UpdateHealthGUI();
        }

        public void UpdateHealth(int value)
        {
            healthCurrent = Mathf.Clamp(value, 0, healthMax);
            UpdateHealthGUI();
        }

        private void UpdateHealthGUI()
        {
            healthBar.value = healthCurrent / (float)healthMax;
            healthText.text = $"{Mathf.CeilToInt(healthCurrent)}/{healthMax}";
        }

        public void SetupEnergy(int maxValue)
        {
            energyMax = maxValue;
            energyCurrent = energyMax;
            UpdateEnergyGUI();
        }

        public void UpdateEnergy(int value)
        {
            energyCurrent = Mathf.Clamp(value, 0, energyMax);
            UpdateEnergyGUI();
        }

        private void UpdateEnergyGUI()
        {
            energyBar.value = energyCurrent / (float)energyMax;
            energyText.text = $"{Mathf.CeilToInt(energyCurrent)}/{energyMax}";
        }

        public void SetAmmo(int value)
        {
            ammoCurrent = value;
            ammoText.text = ammoCurrent.ToString();
        }
    }
}