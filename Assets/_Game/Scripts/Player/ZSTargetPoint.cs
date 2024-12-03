using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZombieShooter
{
    public class ZSTargetPoint : MonoBehaviour
    {
        public float detectionRadius = 5f;
        public LayerMask detectionLayer;

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

        public Action<ZSTargetPoint> OnPlayerEnterTarget;

        private bool isCompleted;
        private bool isActive;

        private void Awake() {
            IsCompleted = false;
            IsActive = false;
        }

        private void Update() {
            if (IsActive)
            {
                if (DetectNearbyObject(Utilities.PLAYER_TAG))
                {
                    OnPlayerEnterTarget?.Invoke(this);
                }
            }
        }

        private bool DetectNearbyObject(string tag)
        {
            Vector3 sphereCenter = transform.position;
            Collider[] colliders = Physics.OverlapSphere(sphereCenter, detectionRadius, detectionLayer);

            foreach (Collider collider in colliders)
            {
                Debug.Log("-- Hit ST");
                if (collider.gameObject.CompareTag(tag))
                {
                    return true;
                }
            }
            return false;
        }

        private void OnDrawGizmosSelected()
        {
            // Draw the detection sphere in the editor
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position, detectionRadius);
        }
    }
}