using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using ZSBehaviourTree;

namespace ZombieShooter
{
    public class ZombieController : MonoBehaviour
    {
        [SerializeField] public BehaviourTreeRunner treeRunner;
        public GameObject target;
        // public AudioSource[] splats;
        public float walkingSpeed;
        public float runningSpeed;
        public float damageAmount = 5;
        public float distanceToTarget = 2f;
        public GameObject ragdoll;

        public bool IsDead { get; private set; }

        Animator anim;
        NavMeshAgent agent;
        BehaviourTree behaviourTree;

        // Start is called before the first frame update
        private void Awake() {
            IsDead = false;
            anim = this.GetComponent<Animator>();
            agent = this.GetComponent<NavMeshAgent>();
        }

        private void Start() {
            target = ZombieShooterManager.Instance.Player.gameObject;
            if (treeRunner)
            {
                treeRunner.RunTree(AssignBehaviourTreeData);
            }
        }

        private void AssignBehaviourTreeData(BehaviourTree tree)
        {
            behaviourTree = tree;
            behaviourTree.BindData(Utilities.PLAYER_TAG, target);
            behaviourTree.BindData(Utilities.ZOMBIE_TAG, this);
        }

        float DistanceToPlayer()
        {
            if (ZSGameStats.GameOver) return Mathf.Infinity;
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
            TriggerDead();
        }

        // void PlaySplatAudio()
        // {
        //     AudioSource audioSource = new AudioSource();
        //     int n = Random.Range(1, splats.Length);

        //     audioSource = splats[n];
        //     audioSource.Play();
        //     splats[n] = splats[0];
        //     splats[0] = audioSource;
        // }

        public void DamagePlayer()
        {
            if (target != null)
            {
                target.GetComponent<ZSPlayerController>().HandleZombieHit(damageAmount);
                // PlaySplatAudio();
            }
        }

        // public bool Isdead()
        // {
        //     return state == STATE.DEAD;
        // }

        public void TurnOffTriggers()
        {
            anim.SetBool("isWalking", false);
            anim.SetBool("isAttacking", false);
            anim.SetBool("isRunning", false);
            anim.SetBool("isDead", false);
        }

        public void TriggerIdle()
        {
            TurnOffTriggers();
        }

        public void TriggerWander()
        {
            if (!agent.hasPath)
            {
                float newX = this.transform.position.x + Random.Range(-5, 5);
                float newZ = this.transform.position.z + Random.Range(-5, 5);
                float newY = Terrain.activeTerrain.SampleHeight(new Vector3(newX, 0, newZ));
                Vector3 dest = new Vector3(newX, newY, newZ);
                agent.SetDestination(dest);
                agent.stoppingDistance = 0;
                // TurnOffTriggers();
                agent.speed = walkingSpeed;
                anim.SetBool("isWalking", true);
            }
        }

        public void TriggerChase(GameObject target)
        {
            agent.SetDestination(target.transform.position);
            agent.stoppingDistance = distanceToTarget;
            // TurnOffTriggers();
            agent.speed = runningSpeed;
            anim.SetBool("isRunning", true);
        }

        public void TriggerAttack(GameObject target)
        {
            anim.SetBool("isAttacking", true);
            this.transform.LookAt(target.transform.position);
        }

        public void TriggerDead()
        {
            anim.SetBool("isDead", true);
            IsDead = true;
        }

        public void ResetAgent()
        {
            agent.ResetPath();
        }


        // Update is called once per frame
        // void Update()
        // {
        //     if (target == null && ZSGameStats.GameOver == false)
        //     {
        //         target = GameObject.FindWithTag("Player");
        //         return;
        //     }
        //     switch (state)
        //     {
        //         case STATE.IDLE:
        //             if (CanSeePlayer()) state = STATE.CHASE;
        //             else if(Random.Range(0,5000) < 5)
        //                 state = STATE.WANDER;
        //             break;
        //         case STATE.WANDER:
        //             if (!agent.hasPath)
        //             {
        //                 float newX = this.transform.position.x + Random.Range(-5, 5);
        //                 float newZ = this.transform.position.z + Random.Range(-5, 5);
        //                 float newY = Terrain.activeTerrain.SampleHeight(new Vector3(newX, 0, newZ));
        //                 Vector3 dest = new Vector3(newX, newY, newZ);
        //                 agent.SetDestination(dest);
        //                 agent.stoppingDistance = 0;
        //                 TurnOffTriggers();
        //                 agent.speed = walkingSpeed;
        //                 anim.SetBool("isWalking", true);
        //             }
        //             if (CanSeePlayer()) state = STATE.CHASE;
        //             else if (Random.Range(0, 5000) < 5)
        //             {
        //                 state = STATE.IDLE;
        //                 TurnOffTriggers();
        //                 agent.ResetPath();
        //             }
        //             break;
        //         case STATE.CHASE:
        //             if (ZSGameStats.GameOver) { TurnOffTriggers(); state = STATE.WANDER; return; }
        //             agent.SetDestination(target.transform.position);
        //             agent.stoppingDistance = 5;
        //             TurnOffTriggers();
        //             agent.speed = runningSpeed;
        //             anim.SetBool("isRunning", true);

        //             if (agent.remainingDistance <= agent.stoppingDistance && !agent.pathPending)
        //             {
        //                 state = STATE.ATTACK;
        //             }

        //             if (ForgetPlayer())
        //             {
        //                 state = STATE.WANDER;
        //                 agent.ResetPath();
        //             }

        //             break;
        //         case STATE.ATTACK:
        //             if (ZSGameStats.GameOver) { TurnOffTriggers(); state = STATE.WANDER; return; }
        //             TurnOffTriggers();
        //             anim.SetBool("isAttacking", true);
        //             this.transform.LookAt(target.transform.position);
        //             if (DistanceToPlayer() > agent.stoppingDistance + 2)
        //                 state = STATE.CHASE;
        //             break;
        //         case STATE.DEAD:
        //             Destroy(agent);
        //             this.GetComponent<Sink>().StartSink();
        //             break;
        //     }
        // }
    }
}