using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZombieShooter
{
    public abstract class ZSTargetPoint : MonoBehaviour
    {
        public bool IsCompleted
        {
            get => isCompleted;
            set
            {
                isCompleted = value;
                if (value)
                {
                    IsActive = false;
                }
            }
        }

        public bool IsActive
        {
            get => isActive;
            set
            {
                isActive = value;
                this.gameObject.SetActive(value);
            }
        }

        public Action<ZSTargetPoint> OnPlayerCompleteMission;

        private bool isCompleted;
        private bool isActive;

        protected virtual void Awake() {
            IsCompleted = false;
            IsActive = false;
        }

        protected virtual void Update() {
            if (IsActive)
            {
                CheckMission();
            }
        }

        protected virtual void CheckMission()
        {
            // Check in subclass
        }
    }
}