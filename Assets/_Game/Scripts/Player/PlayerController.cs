using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using AudioPlayer;

namespace ZombieShooter
{
    public class PlayerController : MonoBehaviour
    {
        // public static readonly int HashAnimatorIdle = Animator.StringToHash("Idle");
        public static readonly int HashAnimatorRun = Animator.StringToHash("Run");
        public static readonly int HashAnimatorAim = Animator.StringToHash("Aim");
        public static readonly int HashAnimatorFire = Animator.StringToHash("Fire");
        public static readonly int HashAnimatorReload = Animator.StringToHash("Reload");
        public static readonly int HashAnimatorDead = Animator.StringToHash("Dead");
        public static readonly int HashAnimatorDance = Animator.StringToHash("Dance");

#if UNITY_EDITOR
        public GameHeader headerEditor = new GameHeader() { header = "Components" };
#endif
        [SerializeField] private GameObject modelContainer;
        [SerializeField] private Animator animator;
        [SerializeField] private CharacterController charController;
        [SerializeField] private Transform shotDirection;
        [SerializeField] private CinemachineVirtualCamera playerCam;
        [SerializeField] private Inventory inventory;
        [SerializeField] private GameObject stevePrefab;

#if UNITY_EDITOR
        public GameHeader headerEditor1 = new GameHeader() { header = "Configs" };
#endif
        [SerializeField] private ShooterConfig config;

#if UNITY_EDITOR
        public GameHeader headerEditor2 = new GameHeader() { header = "Params" };
#endif
        
        [SerializeField] private float mouseXSensitivity = 5f;
        [SerializeField] private float mouseYSensitivity = 5f;
        [SerializeField] private float maxYAngle = 80f;

#if UNITY_EDITOR
        public GameHeader headerEditor3 = new GameHeader() { header = "Audio" };
#endif
        [SerializeField] private float timeBetweenFootStep = 0.5f;
        [SerializeField] private float timeBetweenShot = 0.5f;
        

        public bool IsPlaying;
        
        private float xCamRotation = 0f; // Vertical rotation of the camera
        private float yCamRotation = 0f; // Horizontal rotation of the camera
        private Vector3 velocity;

        private bool isGrounded;
        private bool isRunning = false;
        private bool isShooting = false;

        private bool activeLockCursor;
        private bool isCursorLocked = false;

        private float moveLR = 0f; // move left, right
        private float moveFB = 0f; // move forward, back

        private float reloadCountdown = 0f;

        private float timeBetweenOnGround = 0.3f;
        private float roundSoundCountdown = 0f;

        private float shotChargeTime = 0.1f;

        public void Initialize()
        {
            IsPlaying = false;
            isCursorLocked = true;
            isGrounded = false;
            isRunning = false;
            isShooting = false;
            inventory.Initialize(config);
            FillAmmo();
            ResetAudioTimer();
            ToggleCursorLock(true);
        }

        private void Update()
        {
            if (IsPlaying)
            {
                CheckOnGround();
                UpdateLockCursor();
                UpdateGravity();
                UpdateMovement();
                UpdateCamera();
                UpdateBehaviour();
                UpdateSoundLoop();
            }
        }

#region Player Physic/Input
        private void CheckOnGround()
        {
            var newCheck = IsGrounded();
            if (isGrounded != newCheck)
            {
                isGrounded = newCheck;
                if (isGrounded && roundSoundCountdown <= 0f)
                {
                    roundSoundCountdown = timeBetweenOnGround;
                    PlaySound(SoundID.SFX_ZS_PLAYER_LAND);
                }
            }

            if (!isGrounded)
            {
                if (roundSoundCountdown > 0f)
                {
                    roundSoundCountdown -= Time.deltaTime;
                }
            }
            else
            {
                roundSoundCountdown = timeBetweenOnGround;
            }
        }

        private void UpdateGravity()
        {
            if (isGrounded && velocity.y < 0f)
            {
                velocity.y = -2f;
            }

            // Jump
            if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
            {
                // Debug.Log("-- Press space");
                PlaySound(SoundID.SFX_ZS_PLAYER_JUMP);
                velocity.y = Mathf.Sqrt(config.JumpForce * -2.0f * config.Gravity);
            }

            velocity.y += config.Gravity * Time.deltaTime;
            charController.Move(velocity * Time.deltaTime);
        }

        private void UpdateMovement()
        {
            moveLR = Input.GetAxis("Horizontal");
            moveFB = Input.GetAxis("Vertical");

            Vector3 move = this.transform.right * moveLR + this.transform.forward * moveFB;
            charController.Move(move * config.Speed * Time.deltaTime);
        }

        private void UpdateCamera()
        {
            // Camera
            if (playerCam && isCursorLocked)
            {
                float mouseX = Input.GetAxis("Mouse X") * mouseXSensitivity;
                float mouseY = Input.GetAxis("Mouse Y") * mouseYSensitivity;

                xCamRotation -= mouseY;
                xCamRotation = Mathf.Clamp(xCamRotation, -maxYAngle, maxYAngle);

                yCamRotation += mouseX;

                playerCam.transform.rotation = Quaternion.Euler(xCamRotation, yCamRotation, 0f);
                this.transform.rotation = Quaternion.Euler(0f, yCamRotation, 0f);
                modelContainer.transform.rotation = Quaternion.Euler(xCamRotation, yCamRotation, 0f);
                playerCam.transform.position = this.transform.position;
            }
        }

        private void UpdateLockCursor()
        {
            if (activeLockCursor)
            {
                if (Input.GetKeyDown(KeyCode.LeftAlt))
                {
                    isCursorLocked = false;
                }
                else if (Input.GetKeyUp(KeyCode.LeftAlt))
                {
                    isCursorLocked = true;
                }

                if (isCursorLocked)
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                }
                else
                {
                    Cursor.lockState = CursorLockMode.None;
                    Cursor.visible = true;
                }
            }
        }

        private void UpdateBehaviour()
        {
            // Aiming
            if (Input.GetKeyDown(KeyCode.T))
            {
                animator?.SetBool(HashAnimatorAim, !animator.GetBool(HashAnimatorAim));
            }
            
            // Shooting
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                ToggleShooting(true);
            }
            else if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                ToggleShooting(false);
            }
            if (isShooting && !IsShootingAvailable()) // Out of ammo
            {
                PlaySound(SoundID.SFX_ZS_PLAYER_EMPTY_AMMO);
                ToggleShooting(false);
            }

            // Reload
            if (reloadCountdown > 0f)
            {
                reloadCountdown -= Time.deltaTime;
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.R))
                {
                    HandleReload();
                }
            }
            

            // Running
            if (Mathf.Abs(moveLR) > 0f || Mathf.Abs(moveFB) > 0f)
            {
                ToggleRunning(true);
            }
            else
            {
                ToggleRunning(false);
            }
        }

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if (hit.gameObject.CompareTag(Utilities.ITEM_TAG))
            {
                if (hit.gameObject.TryGetComponent<ShooterItem>(out var item) && !item.Consumed)
                {
                    item.Consumed = true;
                    CheckItemEffect(item);
                }
            }
        }
#endregion

#region Handle Behaviour
        private void HandleShot()
        {
            if (IsShootingAvailable())
            {
                HandleShootZombie();
                PlaySound(SoundID.SFX_ZS_PLAYER_SHOT);
                currentAmmo--;
            }
        }

        private void HandleDeath()
        {
            PlaySound(SoundID.SFX_ZS_PLAYER_DEATH);
            IsPlaying = false;
            ZombieShooterManager.ON_END_GAME?.Invoke();
        }

        int currentAmmo = 0;
        private void HandleReload()
        {
            if (inventory.TryFillAmmoClip(ref currentAmmo, config.AmmoClip))
            {
                reloadCountdown = config.ReloadTime;
                animator?.SetTrigger(HashAnimatorReload);
                PlaySound(SoundID.SFX_ZS_PLAYER_RELOAD);
            }
        }

        private void FillAmmo()
        {
            currentAmmo = config.AmmoClip;
        }

        private void HandleShootZombie()
        {
            RaycastHit hitInfo;
            if (Physics.Raycast(shotDirection.position, shotDirection.forward, out hitInfo, 200))
            {
                GameObject hitObj = hitInfo.collider.gameObject;
                if (hitObj.CompareTag("Zombie"))
                {
                    if (Random.Range(0, 10) < 5)
                    {
                        var rdPrefab = hitObj.GetComponent<ZombieController>().ragdoll;
                        var newRD = Instantiate(rdPrefab, hitObj.transform.position, hitObj.transform.rotation);
                        newRD.transform.Find("Hips").GetComponent<Rigidbody>().AddForce(shotDirection.forward * 10000);
                        Destroy(hitObj);
                    }
                    else
                    {
                        var zombie = hitObj.GetComponent<ZombieController>();
                        zombie.KillZombie();
                    }
                }
            }
        }

        public void HandleZombieHit(float amount)
        {
            inventory.DecreaseHealth(Mathf.CeilToInt(amount));
            if (inventory.Health <= 0)
            {
                Vector3 pos = new Vector3(this.transform.position.x,
                                            Terrain.activeTerrain.SampleHeight(this.transform.position),
                                            this.transform.position.z);
                GameObject steve = Instantiate(stevePrefab, pos, this.transform.rotation);
                steve.GetComponent<Animator>().SetTrigger(HashAnimatorDead);
                GameStats.gameOver = true;
                Destroy(this.gameObject);
            }
        }

        public void HandleWin()
        {
            Vector3 pos = new Vector3(this.transform.position.x,
                                        Terrain.activeTerrain.SampleHeight(this.transform.position),
                                        this.transform.position.z);
            GameObject steve = Instantiate(stevePrefab, pos, this.transform.rotation);
            steve.GetComponent<Animator>().SetTrigger(HashAnimatorDance);
            GameStats.gameOver = true;
            Destroy(this.gameObject);
        }

        private bool IsGrounded()
        {
            if (charController)
            {
                if (Physics.SphereCast(this.transform.position, charController.radius, Vector3.down, out var hitInfo, (charController.height / 2f) - charController.radius + 0.1f))
                {
                    return true;
                }
            }
            return false;
        }

        private bool IsShootingAvailable()
        {
            return currentAmmo > 0 && !IsReloading();
        }

        private bool IsReloading()
        {
            return reloadCountdown > 0f;
        }

        public void ToggleCursorLock(bool toggle)
        {
            activeLockCursor = toggle;
            if (!activeLockCursor)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
            UpdateLockCursor();
        }

        private void ToggleShooting(bool toggle)
        {
            if (toggle && !IsShootingAvailable())
            {
                PlaySound(SoundID.SFX_ZS_PLAYER_EMPTY_AMMO);
                return;
            }

            isShooting = toggle;
            animator?.SetBool(HashAnimatorFire, toggle);
            if (!toggle)
            {
                ResetShotTimer();
            }
        }

        private void ToggleRunning(bool toggle)
        {
            isRunning = toggle;
            animator?.SetBool(HashAnimatorRun, toggle);
            if (!toggle)
            {
                ResetFootStepTimer();
            }
        }
#endregion

#region Item
        private void CheckItemEffect(ShooterItem item)
        {
            switch (item.Type)
            {
                case eItemType.MedKit:
                    var medKit = item as MedKitItem;
                    if (medKit)
                    {
                        inventory.IncreaseHealth(medKit.Value);
                    }
                break;
                case eItemType.Ammo_Normal:
                    var ammo = item as AmmoItem;
                    if (ammo)
                    {
                        inventory.IncreaseAmmo(ammo.Value);
                    }
                break;
            }
        }

#endregion

#region Audio
        private float currentTimeBetweenFootStep;
        private float currentTimeBetweenShot;

        private void PlaySound(SoundID id)
        {
            SoundManager.Instance?.PlaySound(id);
        }

        private void ResetAudioTimer()
        {
            ResetFootStepTimer();
            ResetShotTimer();
        }

        private void ResetFootStepTimer()
        {
            currentTimeBetweenFootStep = timeBetweenFootStep;
        }

        private void ResetShotTimer(bool isFirstShot = true)
        {
            if (isFirstShot)
            {
                currentTimeBetweenShot = shotChargeTime;
            }
            else
            {
                currentTimeBetweenShot = timeBetweenShot;
            }
        }

        private void UpdateSoundLoop()
        {
            if (isShooting)
            {
                if (currentTimeBetweenShot <= 0)
                {
                    ResetShotTimer(false);
                    HandleShot();
                }
                else
                {
                    currentTimeBetweenShot -= Time.deltaTime;
                }
            }

            if (isRunning && isGrounded)
            {
                if (currentTimeBetweenFootStep <= 0)
                {
                    ResetFootStepTimer();
                    PlaySound(SoundID.SFX_ZS_PLAYER_FOOTSTEP);
                }
                else
                {
                    currentTimeBetweenFootStep -= Time.deltaTime;
                }
            }
        }

#endregion
    }
}