using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace ZombieShooter
{
    public class ZombieController : MonoBehaviour
    {
        public GameObject target;
        public AudioSource[] splats;
        public float walkingSpeed;
        public float runningSpeed;
        public float damageAmount = 5;
        public GameObject ragdoll;
        Animator anim;
        NavMeshAgent agent;

        enum STATE { IDLE, WANDER, ATTACK, CHASE, DEAD };
        STATE state = STATE.IDLE;

        // Start is called before the first frame update
        void Start()
        {
            anim = this.GetComponent<Animator>();
            agent = this.GetComponent<NavMeshAgent>();
        }

        void TurnOffTriggers()
        {
            anim.SetBool("isWalking", false);
            anim.SetBool("isAttacking", false);
            anim.SetBool("isRunning", false);
            anim.SetBool("isDead", false);
        }

        float DistanceToPlayer()
        {
            if (ZombieShooterStats.GameOver) return Mathf.Infinity;
            return Vector3.Distance(target.transform.position, this.transform.position);
        }

        bool CanSeePlayer()
        {
            if (DistanceToPlayer() < 10)
                return true;
            return false;
        }

        bool ForgetPlayer()
        {
            if (DistanceToPlayer() > 20)
                return true;
            return false;
        }

        public void KillZombie()
        {
            TurnOffTriggers();
            anim.SetBool("isDead", true);
            state = STATE.DEAD;
        }

        void PlaySplatAudio()
        {
            AudioSource audioSource = new AudioSource();
            int n = Random.Range(1, splats.Length);

            audioSource = splats[n];
            audioSource.Play();
            splats[n] = splats[0];
            splats[0] = audioSource;
        }

        public void DamagePlayer()
        {
            if (target != null)
            {
                target.GetComponent<PlayerController>().HandleZombieHit(damageAmount);
                PlaySplatAudio();
            }
        }


        // Update is called once per frame
        void Update()
        {
            if (target == null && ZombieShooterStats.GameOver == false)
            {
                target = GameObject.FindWithTag("Player");
                return;
            }
            switch (state)
            {
                case STATE.IDLE:
                    if (CanSeePlayer()) state = STATE.CHASE;
                    else if(Random.Range(0,5000) < 5)
                        state = STATE.WANDER;
                    break;
                case STATE.WANDER:
                    if (!agent.hasPath)
                    {
                        float newX = this.transform.position.x + Random.Range(-5, 5);
                        float newZ = this.transform.position.z + Random.Range(-5, 5);
                        float newY = Terrain.activeTerrain.SampleHeight(new Vector3(newX, 0, newZ));
                        Vector3 dest = new Vector3(newX, newY, newZ);
                        agent.SetDestination(dest);
                        agent.stoppingDistance = 0;
                        TurnOffTriggers();
                        agent.speed = walkingSpeed;
                        anim.SetBool("isWalking", true);
                    }
                    if (CanSeePlayer()) state = STATE.CHASE;
                    else if (Random.Range(0, 5000) < 5)
                    {
                        state = STATE.IDLE;
                        TurnOffTriggers();
                        agent.ResetPath();
                    }
                    break;
                case STATE.CHASE:
                    if (ZombieShooterStats.GameOver) { TurnOffTriggers(); state = STATE.WANDER; return; }
                    agent.SetDestination(target.transform.position);
                    agent.stoppingDistance = 5;
                    TurnOffTriggers();
                    agent.speed = runningSpeed;
                    anim.SetBool("isRunning", true);

                    if (agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
                    {
                        state = STATE.ATTACK;
                    }

                    if (ForgetPlayer())
                    {
                        state = STATE.WANDER;
                        agent.ResetPath();
                    }

                    break;
                case STATE.ATTACK:
                    if (ZombieShooterStats.GameOver) { TurnOffTriggers(); state = STATE.WANDER; return; }
                    TurnOffTriggers();
                    anim.SetBool("isAttacking", true);
                    this.transform.LookAt(target.transform.position);
                    if (DistanceToPlayer() > agent.stoppingDistance + 2)
                        state = STATE.CHASE;
                    break;
                case STATE.DEAD:
                    Destroy(agent);
                    this.GetComponent<Sink>().StartSink();
                    break;
            }
        }
    }
}