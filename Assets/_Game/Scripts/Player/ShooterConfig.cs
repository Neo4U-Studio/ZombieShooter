using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZombieShooter
{
    [CreateAssetMenu(menuName = "Player Configs/ShooterConfig")]
    public class ShooterConfig : ScriptableObject
    {
        // Character
        public float Speed = 10f;
        public float JumpForce = 2f;
        public float Gravity = -30f;

        // Inventory
        public int MaxAmmo = 50;
        public int MaxHealth = 100;
    }
}