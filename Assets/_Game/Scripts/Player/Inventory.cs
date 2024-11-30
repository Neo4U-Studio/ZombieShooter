using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZombieShooter
{
    public class Inventory : MonoBehaviour
    {
        int maxAmmo = 50;
        public int Ammo { get; private set; }

        int maxHealth = 100;
        public int Health { get; private set; }
        

        public void Initialize(ShooterConfig playerConfig)
        {
            maxAmmo = playerConfig.MaxAmmo;
            Ammo = maxAmmo;

            maxHealth = playerConfig.MaxHealth;
            Health = maxHealth;
        }

        public void IncreaseAmmo(int value)
        {
            Ammo += value;
            Ammo = Mathf.Clamp(Ammo, 0, maxAmmo);
        }

        public void DecreaseAmmo(int value)
        {
            Ammo -= value;
            Ammo = Mathf.Clamp(Ammo, 0, maxAmmo);
        }

        public void ConsumeAmmo()
        {
            DecreaseAmmo(1);
        }

        public bool TryFillAmmoClip(ref int currentAmmo, int ammoClip)
        {
            if (currentAmmo >= ammoClip)
            {
                return false;
            }
            else if (Ammo <= 0)
            {
                return false;
            }
            else
            {
                int amount = ammoClip - currentAmmo;
                amount = Mathf.Clamp(amount, 0, Ammo);
                DecreaseAmmo(amount);
                currentAmmo += amount;
                return true;
            }
        }

        public void FillAmmo()
        {
            Ammo = maxAmmo;
        }

        public void IncreaseHealth(int value)
        {
            Health += value;
            Health = Mathf.Clamp(Health, 0, maxHealth);
        }

        public void DecreaseHealth(int value)
        {
            Health -= value;
            Health = Mathf.Clamp(Health, 0, maxHealth);
        }
    }
}