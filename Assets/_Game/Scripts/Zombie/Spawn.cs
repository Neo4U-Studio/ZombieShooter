using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

namespace ZombieShooter
{
    public class Spawn : MonoBehaviour
    {
        public GameObject zombiePrefab;
        public int number;
        public float spawnRadius;
        public float detectionRadius = 5f;
        public LayerMask detectionLayer;
        public bool SpawnOnStart = true;

        private bool spawned = false;

        void Start()
        {
            if (SpawnOnStart)
                SpawnAll();
        }

        void SpawnAll()
        {
            for (int i = 0; i < number; i++)
            {
                Vector3 randomPoint = this.transform.position + Random.insideUnitSphere * spawnRadius;

                NavMeshHit hit;
                if (NavMesh.SamplePosition(randomPoint, out hit, 10.0f, NavMesh.AllAreas))
                {
                    Instantiate(zombiePrefab, hit.position, Quaternion.identity);
                }
                else
                    i--;
            }
            spawned = true;
            this.gameObject.SetActive(false);
        }

        private void Update() {
            if (!SpawnOnStart && !spawned)
            {
                if (DetectNearbyObject(Utilities.PLAYER_TAG))
                {
                    SpawnAll();
                }
            }
        }

        private bool DetectNearbyObject(string tag)
        {
            Vector3 sphereCenter = transform.position;
            Collider[] colliders = Physics.OverlapSphere(sphereCenter, detectionRadius, detectionLayer);

            foreach (Collider collider in colliders)
            {
                if (collider.gameObject.CompareTag(tag))
                {
                    return true;
                }
            }
            return false;
        }
    }
}