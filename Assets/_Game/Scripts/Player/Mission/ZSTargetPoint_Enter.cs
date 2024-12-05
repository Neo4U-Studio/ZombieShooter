using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZombieShooter
{
    public class ZSTargetPoint_Enter : ZSTargetPoint
    {
        [SerializeField] protected float detectionRadius = 5f;
        [SerializeField] protected LayerMask detectionLayer;

        protected override void CheckMission()
        {
            if (DetectNearbyObject(Utilities.PLAYER_TAG))
            {
                OnPlayerCompleteMission?.Invoke(this);
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