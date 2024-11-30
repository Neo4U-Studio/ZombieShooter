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

        public void IncreaseHealth(int value)
        {
            Health += value;
            Health = Mathf.Clamp(Health, 0, maxHealth);
        }
    }
}