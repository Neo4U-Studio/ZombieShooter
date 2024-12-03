using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZombieShooter
{
    [CreateAssetMenu(menuName = "Player Configs/PlayerConfig")]
    public class ZSPlayerConfig : ScriptableObject
    {
        // Character
        public float WalkSpeed = 4f;
        public float RunSpeed = 8f;
        public float JumpForce = 2f;
        public float Gravity = -30f;
        public float ReloadTime = 3f;

        // Status
        public int MaxHealth = 100;
        public int MaxEnergy = 100;
        public float IncreaseEnergySpeed = 10;
        public float DecreaseEnergySpeed = 20;

        // Inventory
        public int MaxAmmo = 50;
        public int AmmoClip = 10;
    }
}