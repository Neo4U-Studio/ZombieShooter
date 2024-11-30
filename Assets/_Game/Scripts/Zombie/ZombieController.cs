using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZombieShooter
{
    public class ZombieController : MonoBehaviour
    {
        public static readonly int HashAnimatorWalk = Animator.StringToHash("Walk");
        public static readonly int HashAnimatorRun = Animator.StringToHash("Run");
        public static readonly int HashAnimatorAttack = Animator.StringToHash("Attack");
        public static readonly int HashAnimatorDeath = Animator.StringToHash("Dead");

        Animator anim;

        // Start is called before the first frame update
        void Start()
        {
            anim = this.GetComponent<Animator>();
        }

        // Update is called once per frame
        void Update()
        {
            if (Input.GetKey(KeyCode.W))
            {
                anim.SetBool(HashAnimatorWalk, true);
            }
            else
                anim.SetBool(HashAnimatorWalk, false);

            if (Input.GetKey(KeyCode.R))
            {
                anim.SetBool(HashAnimatorRun, true);
            }
            else
                anim.SetBool(HashAnimatorRun, false);

            if (Input.GetKey(KeyCode.A))
            {
                anim.SetBool(HashAnimatorAttack, true);
            }
            else
                anim.SetBool(HashAnimatorAttack, false);

            if (Input.GetKey(KeyCode.D))
            {
                anim.SetBool(HashAnimatorDeath, true);
            }

        }
    }
}