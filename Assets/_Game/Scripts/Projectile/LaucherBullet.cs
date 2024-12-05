using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZombieShooter
{
    public class LaucherBullet : ProjectileController
    {
#if UNITY_EDITOR
        public GameHeader headerEditor1 = new GameHeader() { header = "Laucher" };
#endif
        [SerializeField] float explosionRadius = 5f;
        [SerializeField] float explosionForce = 500f;

        [Tooltip("Auto explode")]
        [SerializeField] float explosionDelay = 3f;

        private bool hasExploded = false;
        private float explodeTime;

        protected override void ResetBullet()
        {
            hasExploded = false;
            explodeTime = explosionDelay;
            base.ResetBullet();
        }

        protected override void Update()
        {
            explodeTime -= Time.deltaTime;
            if (explodeTime <= 0f && !hasExploded)
            {
                Explode();
            }
        }

        protected override void TriggerBulletEffect()
        {
            if (!hasExploded)
            {
                Explode();
            }
        }

        void Explode()
        {
            hasExploded = true;

            Debug.Log("Grenade exploded!");

            Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);

            foreach (Collider nearbyObject in colliders)
            {
                Rigidbody rb = nearbyObject.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
                }
            }
        }
    }
}