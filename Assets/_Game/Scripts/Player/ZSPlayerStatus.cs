using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZombieShooter
{
    public class ZSPlayerStatus : MonoBehaviour
    {
        // int maxAmmo = 50;
        // public int Ammo { get; private set; }

        int maxHealth = 100;
        public int Health { get; private set; }

        int maxEnergy = 100;
        public float Energy { get; private set; }
        

        public void Initialize(ZSPlayerConfig playerConfig)
        {
            // maxAmmo = playerConfig.MaxAmmo;
            // Ammo = maxAmmo;
            // ZombieShooterUI.Instance?.PlayerStatus.SetAmmo(Ammo);

            maxHealth = playerConfig.MaxHealth;
            Health = maxHealth;
            ZombieShooterUI.Instance?.PlayerStatus.SetupHealth(maxHealth);

            maxEnergy = playerConfig.MaxEnergy;
            Energy = maxEnergy;
            ZombieShooterUI.Instance?.PlayerStatus.SetupEnergy(maxEnergy);
        }

        // public void IncreaseAmmo(int value)
        // {
        //     Ammo += value;
        //     Ammo = Mathf.Clamp(Ammo, 0, maxAmmo);
        //     ZombieShooterUI.Instance?.PlayerStatus.SetAmmo(Ammo);
        //     // Debug.Log("--Increase ammo " + Ammo);
        // }

        // public void DecreaseAmmo(int value)
        // {
        //     Ammo -= value;
        //     Ammo = Mathf.Clamp(Ammo, 0, maxAmmo);
        //     ZombieShooterUI.Instance?.PlayerStatus.SetAmmo(Ammo);
        //     // Debug.Log("--Decrease ammo " + Ammo);
        // }

        // public void ConsumeAmmo()
        // {
        //     DecreaseAmmo(1);
        // }

        // public bool TryFillAmmoClip(ref int currentAmmo, int ammoClip)
        // {
        //     if (currentAmmo >= ammoClip)
        //     {
        //         return false;
        //     }
        //     else if (Ammo <= 0)
        //     {
        //         return false;
        //     }
        //     else
        //     {
        //         int amount = ammoClip - currentAmmo;
        //         amount = Mathf.Clamp(amount, 0, Ammo);
        //         DecreaseAmmo(amount);
        //         currentAmmo += amount;
        //         return true;
        //     }
        // }

        // public void FillAmmo()
        // {
        //     Ammo = maxAmmo;
        // }

        public void IncreaseHealth(int value)
        {
            Health += value;
            Health = Mathf.Clamp(Health, 0, maxHealth);
            ZombieShooterUI.Instance?.PlayerStatus.UpdateHealth(Health);
            // Debug.Log("--Increase health " + Health);
        }

        public void DecreaseHealth(int value)
        {
            Health -= value;
            Health = Mathf.Clamp(Health, 0, maxHealth);
            ZombieShooterUI.Instance?.PlayerStatus.UpdateHealth(Health);
            // Debug.Log("--Decrease health " + Health);
        }

        public void IncreaseEnergy(float value)
        {
            if (Energy < maxEnergy)
            {
                Energy += value;
                Energy = Mathf.Clamp(Energy, 0, maxEnergy);
                ZombieShooterUI.Instance?.PlayerStatus.UpdateEnergy(Mathf.CeilToInt(Energy));
                // Debug.Log("--Increase energy " + Energy);
            }
        }

        public void DecreaseEnergy(float value)
        {
            if (Energy > 0)
            {
                Energy -= value;
                Energy = Mathf.Clamp(Energy, 0, maxEnergy);
                ZombieShooterUI.Instance?.PlayerStatus.UpdateEnergy(Mathf.CeilToInt(Energy));
                // Debug.Log("--Decrease energy " + Energy);
            }
        }
    }
}