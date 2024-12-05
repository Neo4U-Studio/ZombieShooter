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
		[SerializeField] float speed = 20f;
		
		[Tooltip("From 0% to 100%")]
		[Range(0, 100)]
		[SerializeField] float accuracy;
		[SerializeField] float timeToDestroy = 10f;
		[SerializeField] GameObject hitVfxPrefab;

		private Vector3 offset;
		private bool collided;
		private Rigidbody rb;

		private bool isMoving;
		private Action<GameObject> onHitObject = null;
		private float currentMovingTime;
		private float currentSpeed;

		private void Awake() {
			isMoving = false;
			rb = GetComponent<Rigidbody>();
		}

		public void Fire(Vector3 target, Action<GameObject> onHitObject = null)
		{
			rb.isKinematic = false;
			this.transform.LookAt(target);
			this.onHitObject = onHitObject;
			this.currentMovingTime = 0f;
			this.currentSpeed = speed;
			this.collided = false;
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
			SetBulletVelocity(target);
			isMoving = true;
		}

		protected virtual void SetBulletVelocity(Vector3 target)
		{
			var direction = (target - this.transform.position).normalized;
			rb.velocity = (direction + offset).normalized * currentSpeed;
		}

		private void Update() {
			if (isMoving)
			{
				currentMovingTime += Time.deltaTime;
				if (currentMovingTime >= timeToDestroy)
				{
					DestroyProjectile(false);
				}
			}
		}

		private void OnCollisionEnter (Collision co)
		{
			if (co.gameObject.tag != "Bullet" && !collided)
			{
				Debug.Log("-- Bullet hit " + co.gameObject.tag);
				collided = true;			
				currentSpeed = 0;
				rb.isKinematic = true;

				ContactPoint contact = co.contacts[0];
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

				DestroyProjectile(false);
				onHitObject?.Invoke(co.gameObject);
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