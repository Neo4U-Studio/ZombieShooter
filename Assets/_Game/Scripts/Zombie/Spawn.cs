using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using Pooling;
using System.Linq;

namespace ZombieShooter
{
    [System.Serializable]
    public struct SpawnData
    {
        public GameObject Prefab;
        public int Number;
        public float SpawnRadius;
    }

    public class Spawn : MonoBehaviour
    {
        [SerializeField] List<SpawnData> listSpawnZombie;
        [SerializeField] float detectionRadius;
        [SerializeField] LayerMask detectionLayer;
        [SerializeField] bool SpawnOnStart = true;

        public bool IsSpawnerClear => spawned && (zombieList.Count <= 0 || zombieList.All(zombie => zombie.IsDead));

        private bool spawned = false;
        private List<ZombieController> zombieList = new List<ZombieController>();

        void Start()
        {
            if (SpawnOnStart)
                SpawnAll();
        }

        void SpawnAll()
        {
            zombieList.Clear();
            foreach (var zombie in listSpawnZombie)
            {
                for (int i = 0; i < zombie.Number; i++)
                {
                    Vector3 randomPoint = this.transform.position + Random.insideUnitSphere * zombie.SpawnRadius;

                    NavMeshHit hit;
                    if (NavMesh.SamplePosition(randomPoint, out hit, 10.0f, NavMesh.AllAreas))
                    {
                        var zombieObj = zombie.Prefab.Spawn(hit.position, Quaternion.identity);
                        var zombieControl = zombieObj.GetComponent<ZombieController>();
                        zombieControl.OnZombieDead += RemoveZombieFromList;
                        zombieControl.StartZombie();
                        zombieList.Add(zombieControl);
                        // Instantiate(zombiePrefab, hit.position, Quaternion.identity);
                    }
                    else
                        i--;
                }
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

        private void RemoveZombieFromList(ZombieController zombie)
        {
            if (zombieList.Contains(zombie))
            {
                zombie.OnZombieDead -= RemoveZombieFromList;
                zombieList.Remove(zombie);
            }
        }
    }
}