using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Pooling;
using UnityEngine;

namespace ZombieShooter
{
	public class ProjectileController : MonoBehaviour
	{
#if UNITY_EDITOR
        public GameHeader headerEditor = new GameHeader() { header = "General" };
#endif
		[SerializeField] float speed = 20f;
		
		[Tooltip("From 0% to 100%")]
		[Range(0, 100)]
		[SerializeField] protected float accuracy;
		[SerializeField] protected float timeToDestroy = 10f;
		[SerializeField] protected GameObject hitVfxPrefab;

		protected Vector3 offset;
		protected bool collided;
		protected Rigidbody rb;

		protected bool isMoving;
		protected Action<GameObject> onHitObject = null;
		protected float currentMovingTime;
		protected float currentSpeed;

		private void Awake() {
			isMoving = false;
			rb = GetComponent<Rigidbody>();
		}

		public void Fire(Vector3 target, Action<GameObject> onHitObject = null)
		{
			this.transform.LookAt(target);
			this.onHitObject = onHitObject;
			ResetBullet();
			CalculateOffset();
			SetBulletVelocity((target - this.transform.position).normalized);
			isMoving = true;
		}

		protected virtual void ResetBullet()
		{
			rb.isKinematic = false;
			this.currentMovingTime = 0f;
			this.currentSpeed = speed;
			this.collided = false;
		}

		protected virtual void CalculateOffset()
		{
			if (accuracy != 100) {
				accuracy = 1 - (accuracy / 100);

				for (int i = 0; i < 2; i++) {
					var val = 1 * UnityEngine.Random.Range (-accuracy, accuracy);
					var index = UnityEngine.Random.Range (0, 2);
					if (i == 0) {
						if (index == 0)
							offset = new Vector3 (0, -val, 0);
						else
							offset = new Vector3 (0, val, 0);
					} else {
						if (index == 0)
							offset = new Vector3 (0, offset.y, -val);
						else
							offset = new Vector3 (0, offset.y, val);
					}
				}
			}
			else
			{
				offset = Vector3.zero;
			}
		}

		protected virtual void SetBulletVelocity(Vector3 direction)
		{
			rb.velocity = (direction + offset).normalized * currentSpeed;
		}

		protected virtual void Update() {
			if (isMoving)
			{
				currentMovingTime += Time.deltaTime;
				if (currentMovingTime >= timeToDestroy)
				{
					DestroyProjectile();
				}
			}
		}

		private void OnCollisionEnter (Collision collision)
		{
			if (collision.gameObject.tag != "Bullet" && !collided)
			{
				// Debug.Log("-- Bullet hit " + co.gameObject.tag);
				collided = true;			
				currentSpeed = 0;
				rb.isKinematic = true;

				TriggerBulletEffect();
				TriggerHitVfx(collision);

				DestroyProjectile(true);
				onHitObject?.Invoke(collision.gameObject);
			}
		}

		protected virtual void TriggerBulletEffect()
		{
			// Init in sub class
		}

		protected virtual void TriggerHitVfx(Collision collision)
		{
			ContactPoint contact = collision.contacts[0];
			Quaternion rot = Quaternion.FromToRotation (Vector3.up, contact.normal);
			Vector3 pos = contact.point;

			if (hitVfxPrefab != null) {
				var hitVFX = Instantiate(hitVfxPrefab, pos, rot);
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

		private void DestroyProjectile(bool immediately = true)
		{
			isMoving = false;
			this.transform.DOScale(0.2f, immediately ? 0f : 0.5f).OnComplete(() => {
				this.gameObject.Despawn();
			});
		}
	}
}