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
        public static readonly int HashAnimatorWalk = Animator.StringToHash("isWalking");
        public static readonly int HashAnimatorAttack = Animator.StringToHash("isAttacking");
        public static readonly int HashAnimatorRun = Animator.StringToHash("isRunning");
        public static readonly int HashAnimatorDead = Animator.StringToHash("isDead");

#if UNITY_EDITOR
        public GameHeader headerEditor = new GameHeader() { header = "Components" };
#endif
        [SerializeField] protected BehaviourTreeRunner treeRunner;
        
        [SerializeField] protected GameObject ragdollPrefab;

#if UNITY_EDITOR
        public GameHeader headerEditor2 = new GameHeader() { header = "Params" };
#endif
        [SerializeField] protected int health = 20;
        [SerializeField] protected float walkingSpeed;
        [SerializeField] protected float runningSpeed;
        [SerializeField] protected float dectectRange = 15; // Range detect target
        [SerializeField] protected float attackRange = 4;  // Range attack target
        [SerializeField] protected float damageAmount = 5;

        public bool IsDead => currentHealth <= 0;
        public eZombieType Type => GetZombieType();

        protected ZSPlayerController target;
        protected Animator anim;
        protected NavMeshAgent agent;
        protected BehaviourTree behaviourTree;
        protected MakeRadarObject radar;
        protected float currentDamageAmount = 0;
        protected int currentHealth = 20;

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
            ResetHealth();
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

        public virtual void DamageToZombie(int damageAmount, Vector3 knockDirection, float force = 0f, bool useRagdoll = true)
        {
            DecreaseHealth(damageAmount);
            if (IsDead)
            {
                ToggleRadar(false);
                if (ragdollPrefab && useRagdoll && IsAllowUseRagdoll())
                {
                    var newRD = Instantiate(ragdollPrefab, this.transform.position, this.transform.rotation);
                    newRD.transform.Find("Hips").GetComponent<Rigidbody>().AddForce(knockDirection * force);
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
        }

        protected void ResetHealth()
        {
            currentHealth = health;
        }

        protected void DecreaseHealth(int amount)
        {
            currentHealth = Mathf.Clamp(currentHealth - amount, 0, health);
        }

        protected virtual bool IsAllowUseRagdoll()
        {
            return true;
        }

        public void DestroyZombie()
        {
            // Debug.Log("-- Destroy zombie");
            this.agent.enabled = false;
            this.gameObject.Despawn();
        }

        public virtual void DamagePlayer()
        {
            if (target != null)
            {
                target.HandleZombieHit(currentDamageAmount);
                // PlaySplatAudio();
            }
        }

        protected float DistanceToPlayer()
        {
            return target && !target.IsDead ? Vector3.Distance(target.transform.position, this.transform.position) : Mathf.Infinity;
        }

        public virtual bool CanAttack()
        {
            return true;
        }

        public virtual void TurnOffTriggers()
        {
            anim.SetBool(HashAnimatorWalk, false);
            anim.SetBool(HashAnimatorAttack, false);
            anim.SetBool(HashAnimatorRun, false);
            anim.SetBool(HashAnimatorDead, false);
        }

        public virtual void TriggerIdle()
        {
            // Debug.Log("-- Trigger idle");
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
                anim.SetBool(HashAnimatorWalk, true);
            }
        }

        public virtual void TriggerWanderRun()
        {
            // Debug.Log("-- Trigger run");
            if (!agent.hasPath)
            {
                float newX = this.transform.position.x + Random.Range(-6, 6);
                float newZ = this.transform.position.z + Random.Range(-6, 6);
                float newY = Terrain.activeTerrain.SampleHeight(new Vector3(newX, 0, newZ));
                Vector3 dest = new Vector3(newX, newY, newZ);
                agent.SetDestination(dest);
                agent.stoppingDistance = 0;
                // TurnOffTriggers();
                agent.speed = runningSpeed;
                anim.SetBool(HashAnimatorRun, true);
            }
        }

        public virtual void TriggerChase(GameObject target)
        {
            // Debug.Log("-- Trigger chase");
            agent.SetDestination(target.transform.position);
            agent.stoppingDistance = distanceToTarget;
            // TurnOffTriggers();
            agent.speed = runningSpeed;
            anim.SetBool(HashAnimatorRun, true);
        }

        public virtual void TriggerAttack(GameObject target) // Normal attack
        {
            anim.SetBool(HashAnimatorAttack, true);

            // this.transform.LookAt(target.transform);
            currentDamageAmount = damageAmount;
            Vector3 lookDirection = target.transform.position - transform.position;
            lookDirection.y = 0; // Prevent rotation on the Y-axis
            transform.rotation = Quaternion.LookRotation(lookDirection);
        }

        public virtual void TriggerDead(bool playAnim = true)
        {
            if (playAnim)
            {
                anim.SetBool(HashAnimatorDead, true);
            }
            currentHealth = 0;
        }

        public void ResetAgent()
        {
            agent.ResetPath();
        }
    }
}