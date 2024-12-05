using System;
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
        [SerializeField] float upwardForce = 5f;
        [SerializeField] float explosionRadius = 5f;
        [SerializeField] float explosionForce = 500f;

        [Tooltip("Auto explode")]
        [SerializeField] float explosionDelay = 3f;

        public Action<Collider[], Vector3, float> OnLaucherBulletExplode;

        private bool hasExploded = false;
        private float explodeTime;

        protected override void SetBulletVelocity(Vector3 direction)
		{
            rb.useGravity = true;
            Vector3 launchDirection = direction + (Vector3.up * upwardForce / currentSpeed);
            rb.velocity = launchDirection.normalized * currentSpeed;
		}

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

        protected override void TriggerHitVfx(Collision collision)
		{
			if (hitVfxPrefab != null) {
				var hitVFX = Instantiate(hitVfxPrefab, this.transform.position, Quaternion.identity);
				var ps = hitVFX.GetComponent<ParticleSystem>();
				if (ps == null)
				{
					var psChild = hitVFX.transform.GetChild(0).GetComponent<ParticleSystem>();
					Destroy(hitVFX, psChild.main.duration);
				} 
				else
				{
					Destroy(hitVFX, ps.main.duration);
				}
			}
		}

        void Explode()
        {
            hasExploded = true;

            // Debug.Log("Grenade exploded!");

            Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius);
            OnLaucherBulletExplode?.Invoke(colliders, this.transform.position, explosionForce);

            // foreach (Collider nearbyObject in colliders)
            // {
            //     Rigidbody rb = nearbyObject.GetComponent<Rigidbody>();
            //     if (rb != null)
            //     {
            //         rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            //     }
            // }
        }
    }
}