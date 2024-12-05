using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZombieShooter
{
    public class ZSTargetPoint_KillZombie : ZSTargetPoint
    {
        [SerializeField] protected Spawn spawner;

        protected override void Awake()
        {
            if (!spawner)
            {
                spawner = GetComponent<Spawn>();
            }
            base.Awake();
        }

        protected override void CheckMission()
        {
            // Debug.Log("-- Check spawner " + spawner.IsSpawnerClear);
            if (spawner && spawner.IsSpawnerClear)
            {
                OnPlayerCompleteMission?.Invoke(this);
            }
        }
    }
}