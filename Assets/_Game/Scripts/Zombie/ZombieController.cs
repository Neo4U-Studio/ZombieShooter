using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using AudioPlayer;

namespace ZombieShooter
{
    public class ZombieController : MonoBehaviour
    {
        // public static readonly int HashAnimatorIdle = Animator.StringToHash("Idle");
        public static readonly int HashAnimatorWalk = Animator.StringToHash("Walk");
        public static readonly int HashAnimatorRun = Animator.StringToHash("Run");
        public static readonly int HashAnimatorAttack = Animator.StringToHash("Attack");
        public static readonly int HashAnimatorDeath = Animator.StringToHash("Death");

        [SerializeField] private Animator animator;

        private void Awake() {
            if (!animator)
            {
                animator = this.GetComponent<Animator>();
            }
        }

        private void Update() {
            UpdateBehaviour();
        }

        private void UpdateBehaviour()
        {
            
            if (Input.GetKeyDown(KeyCode.W))
            {
                Debug.Log("-- Press W");
                animator?.SetBool(HashAnimatorWalk, !animator.GetBool(HashAnimatorWalk));
            }
        }
    }
}