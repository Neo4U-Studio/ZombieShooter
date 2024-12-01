using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZombieShooter
{
    public class ZombieShooterStats : MonoSingleton<ZombieShooterStats>
    {
        public static Action ON_KILL_ZOMBIE;
        public static Action ON_PLAYER_DEAD;

        public static bool GameOver { get; private set; }
        public static bool IsWin { get; private set; }
        public static int NumberKilledZombie { get; private set; }

        int maxKillAmount = 10;

        private void Awake() {
            RefreshValue();
            RegisterEvent();
        }

        private void OnDestroy() {
            UnregisterEvent();
        }

        private void RefreshValue()
        {
            GameOver = false;
            IsWin = false;
            NumberKilledZombie = 0;
        }

        private void RegisterEvent()
        {
            ON_KILL_ZOMBIE += IncreaseZombieKilled;
            ON_PLAYER_DEAD += HandlePlayerDead;
        }

        private void UnregisterEvent()
        {
            ON_KILL_ZOMBIE -= IncreaseZombieKilled;
            ON_PLAYER_DEAD -= HandlePlayerDead;
        }

#region Handle events
        private void IncreaseZombieKilled()
        {
            NumberKilledZombie++;
            if (NumberKilledZombie >= maxKillAmount)
            {
                GameOver = true;
                IsWin = true;
            }
            Debug.Log("-- Increase zombie kill " + NumberKilledZombie);
        }

        private void HandlePlayerDead()
        {
            GameOver = true;
            Debug.Log("-- Player dead ");
        }
#endregion
    }
}