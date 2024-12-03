using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZombieShooter
{
    public class ZSGameStats : MonoSingleton<ZSGameStats>
    {
        public static Action ON_KILL_ZOMBIE;
        public static Action ON_PLAYER_DEAD;

        public static bool GameOver { get; private set; }
        public static bool IsWin { get; private set; }
        public static int NumberKilledZombie { get; private set; }

        // int maxKillAmount = 10;

        private void Awake() {
            RefreshValue();
            RegisterEvent();
        }

        private void OnDestroy() {
            UnregisterEvent();
        }

        public void RefreshValue()
        {
            GameOver = false;
            IsWin = false;
            NumberKilledZombie = 0;
        }

        private void RegisterEvent()
        {
            ON_KILL_ZOMBIE += OnIncreaseZombieKilled;
            ON_PLAYER_DEAD += OnPlayerDead;
        }

        private void UnregisterEvent()
        {
            ON_KILL_ZOMBIE -= OnIncreaseZombieKilled;
            ON_PLAYER_DEAD -= OnPlayerDead;
        }

#region Handle events
        private void OnIncreaseZombieKilled()
        {
            NumberKilledZombie++;
            // if (NumberKilledZombie >= maxKillAmount)
            // {
            //     GameOver = true;
            //     IsWin = true;
            // }
            Debug.Log("-- Increase zombie kill " + NumberKilledZombie);
        }

        private void OnPlayerDead()
        {
            GameOver = true;
            Debug.Log("-- Player dead ");
        }
#endregion
    }
}