using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Pooling;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using ZSBehaviourTree;

namespace ZombieShooter
{
    public abstract class ZombieController : MonoBehaviour
    {
        [SerializeField] protected BehaviourTreeRunner treeRunner;
        [SerializeField] protected Sink sinkZombie;
        [SerializeField] protected float walkingSpeed;
        [SerializeField] protected float runningSpeed;
        [SerializeField] protected float dectectRange = 15; // Range detect target
        [SerializeField] protected float attackRange = 4;  // Range attack target
        [SerializeField] protected float damageAmount = 5;
        [SerializeField] protected GameObject ragdollPrefab;

        public bool IsDead { get; private set; }
        public eZombieType Type => GetZombieType();

        protected ZSPlayerController target;
        protected Animator anim;
        protected NavMeshAgent agent;
        protected BehaviourTree behaviourTree;
        protected MakeRadarObject radar;

        private float distanceToTarget = 2f;

        protected abstract eZombieType GetZombieType();

        protected virtual void Awake() {
            anim = this.GetComponent<Animator>();
            radar = this.GetComponent<MakeRadarObject>();
            // agent = this.AddComponent<NavMeshAgent>();
        }

        // protected virtual void Start() {
        //     target = ZombieShooterManager.Instance.Player;
        //     if (treeRunner)
        //     {
        //         treeRunner.RunTree(AssignBehaviourTreeData);
        //     }
        // }

        public void StartZombie()
        {
            if (!agent)
            {
                agent = this.AddComponent<NavMeshAgent>();
            }
            ToggleRadar (true);
            ToggleZombieCollider(true);
            target = ZombieShooterManager.Instance.Player;
            agent.enabled = true;
            IsDead = false;
            if (treeRunner)
            {
                treeRunner.RunTree(AssignBehaviourTreeData);
            }
        }

        public void ToggleZombieCollider(bool toggle)
        {
            Collider[] colList = this.transform.GetComponentsInChildren<Collider>();
            foreach (Collider c in colList)
            {
                c.enabled = toggle;
            }
        }

        public void ToggleRadar(bool toggle)
        {
            if (radar)
            {
                radar.enabled = toggle;
            }
        }

        protected virtual void AssignBehaviourTreeData(BehaviourTree tree)
        {
            behaviourTree = tree;
            behaviourTree.BindData(Utilities.PLAYER_TAG, target);
            behaviourTree.BindData(Utilities.ZOMBIE_TAG, this);
            behaviourTree.BindData(Utilities.ZOMBIE_DETECT_RANGE_KEY, dectectRange);
            behaviourTree.BindData(Utilities.ZOMBIE_ATTACK_RANGE_KEY, attackRange);
        }

        public virtual void KillZombie(Vector3 shootDirection)
        {
            ToggleRadar(false);
            if (ragdollPrefab & (UnityEngine.Random.Range(0, 10) < 5))
            {
                var newRD = Instantiate(ragdollPrefab, this.transform.position, this.transform.rotation);
                newRD.transform.Find("Hips").GetComponent<Rigidbody>().AddForce(shootDirection * 10000f);
                TurnOffTriggers();
                ResetAgent();
                TriggerDead(false);
                DestroyZombie();
                DOVirtual.DelayedCall(5f, () => Sink.StartSink(newRD, 2, 5));
            }
            else
            {
                TurnOffTriggers();
                ResetAgent();
                TriggerDead(true);
                Sink.StartSink(this.gameObject, 2, 10);
            }
        }

        public void DestroyZombie()
        {
            Debug.Log("-- Destroy zombie");
            this.agent.enabled = false;
            this.gameObject.Despawn();
        }

        public virtual void DamagePlayer()
        {
            if (target != null)
            {
                target.HandleZombieHit(damageAmount);
                // PlaySplatAudio();
            }
        }

        protected float DistanceToPlayer()
        {
            return target && !target.IsDead ? Vector3.Distance(target.transform.position, this.transform.position) : Mathf.Infinity;
        }

        public virtual void TurnOffTriggers()
        {
            anim.SetBool("isWalking", false);
            anim.SetBool("isAttacking", false);
            anim.SetBool("isRunning", false);
            anim.SetBool("isDead", false);
        }

        public virtual void TriggerIdle()
        {
            TurnOffTriggers();
        }

        public virtual void TriggerWander()
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

        public virtual void TriggerChase(GameObject target)
        {
            agent.SetDestination(target.transform.position);
            agent.stoppingDistance = distanceToTarget;
            // TurnOffTriggers();
            agent.speed = runningSpeed;
            anim.SetBool("isRunning", true);
        }

        public virtual void TriggerAttack(GameObject target)
        {
            anim.SetBool("isAttacking", true);

            // this.transform.LookAt(target.transform);
            Vector3 lookDirection = target.transform.position - transform.position;
            lookDirection.y = 0; // Prevent rotation on the Y-axis
            transform.rotation = Quaternion.LookRotation(lookDirection);
        }

        public virtual void TriggerDead(bool playAnim = true)
        {
            if (playAnim)
            {
                anim.SetBool("isDead", true);
            }
            IsDead = true;
        }

        public void ResetAgent()
        {
            agent.ResetPath();
        }
    }
}