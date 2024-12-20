using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;
using AudioPlayer;
using DG.Tweening;
using Pooling;

namespace ZombieShooter
{
    public enum ePlayerBehaviour
    {
        Grounded,
        Moving,
        Running,
        Shooting,
        Reloading
    }

    public class PlayerBehaviourDict : Dictionary<ePlayerBehaviour, bool> { }

    public class ZSPlayerController : MonoBehaviour
    {
        // public static readonly int HashAnimatorIdle = Animator.StringToHash("Idle");
        public static readonly int HashAnimatorRun = Animator.StringToHash("Run");
        public static readonly int HashAnimatorAim = Animator.StringToHash("Aim");
        public static readonly int HashAnimatorFire = Animator.StringToHash("Fire");
        public static readonly int HashAnimatorReload = Animator.StringToHash("Reload");
        public static readonly int HashAnimatorDead = Animator.StringToHash("Death");
        public static readonly int HashAnimatorDance = Animator.StringToHash("Dance");

#if UNITY_EDITOR
        public GameHeader headerEditor = new GameHeader() { header = "Components" };
#endif
        [SerializeField] private GameObject modelContainer;
        [SerializeField] private GameObject playerBack;
        [SerializeField] private Animator animator;
        [SerializeField] private CharacterController charController;
        [SerializeField] private Transform shotContainer;
        [SerializeField] private ZSPlayerStatus status;
        [SerializeField] private GameObject stevePrefab;
        [SerializeField] private List<ZSWeaponController> weaponList;

#if UNITY_EDITOR
        public GameHeader headerEditor1 = new GameHeader() { header = "Configs" };
#endif
        [SerializeField] private ZSPlayerConfig config;

#if UNITY_EDITOR
        public GameHeader headerEditor2 = new GameHeader() { header = "Vfx" };
#endif
        
        [SerializeField] private GameObject bloodVfxPrefab;

#if UNITY_EDITOR
        public GameHeader headerEditor3 = new GameHeader() { header = "Params" };
#endif
        
        [SerializeField] private float mouseXSensitivity = 5f;
        [SerializeField] private float mouseYSensitivity = 5f;
        [SerializeField] private float maxYAngle = 80f;

#if UNITY_EDITOR
        public GameHeader headerEditor4 = new GameHeader() { header = "Audio" };
#endif
        [SerializeField] private float timeBetweenFootStep = 0.5f;
        // [SerializeField] private float timeBetweenShot = 0.5f;
        

        private bool isPlaying;
        public bool IsPlaying
        {
            get => isPlaying;
            set
            {
                isPlaying = value;
                ToggleCursorLock(value);
            }
        }

        public bool IsDead => status.Health <= 0;
        
        private float xCamRotation = 0f; // Vertical rotation of the camera
        private float yCamRotation = 0f; // Horizontal rotation of the camera
        private Vector3 velocity;
        float currentSpeed;

        private PlayerBehaviourDict behaviourDict;

        private bool activeLockCursor;
        private bool isCursorLocked = false;

        private float moveLR = 0f; // move left, right
        private float moveFB = 0f; // move forward, back

        private float timeBetweenOnGround = 0.3f;
        private float roundSoundCountdown = 0f;

        // private float shotChargeTime = 0.1f;

        // int currentAmmo = 0;

        private CinemachineVirtualCamera playerCam;
        private Camera mainCamera;
        
        private ZSWeaponController currentWeapon = null;

        public void Initialize()
        {
            playerCam = CameraManager.Instance.GetVirtualCamera(eVirtualCamera.PLAYER);
            mainCamera = CameraManager.Instance.GetOverlayCamera(eOverlayCamera.MAIN);
            behaviourDict = new PlayerBehaviourDict () {
                {ePlayerBehaviour.Grounded, false},
                {ePlayerBehaviour.Moving, false},
                {ePlayerBehaviour.Running, false},
                {ePlayerBehaviour.Shooting, false},
                {ePlayerBehaviour.Reloading, false},
            };
            IsPlaying = false;
            isCursorLocked = true;
            status.Initialize(config);
            InitWeapon();
            ResetAudioTimer();
            // ToggleCursorLock(true);
        }

        private void InitWeapon()
        {
            weaponList.ForEach(weapon => {
                weapon.Initialize(shotContainer);
            });
            currentWeapon = null;
            HandleSwitchWeapon();
        }

        private void Update()
        {
            if (IsPlaying)
            {
                CheckOnGround();
                UpdateLockCursor();
                UpdateGravity();
                UpdateMovement();
                UpdateEnergy();
                UpdateCamera();
                UpdateBehaviour();
                UpdateSoundLoop();
            }
        }

#region Player Physic/Input
        private void CheckOnGround()
        {
            var newCheck = IsGrounded();
            if (behaviourDict[ePlayerBehaviour.Grounded] != newCheck)
            {
                behaviourDict[ePlayerBehaviour.Grounded] = newCheck;
                if (behaviourDict[ePlayerBehaviour.Grounded] && roundSoundCountdown <= 0f)
                {
                    roundSoundCountdown = timeBetweenOnGround;
                    PlaySound(SoundID.SFX_ZS_PLAYER_LAND);
                }
            }

            if (!behaviourDict[ePlayerBehaviour.Grounded])
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
            if (behaviourDict[ePlayerBehaviour.Grounded] && velocity.y < 0f)
            {
                velocity.y = -2f;
            }

            // Jump
            if (Input.GetKeyDown(KeyCode.Space) && behaviourDict[ePlayerBehaviour.Grounded])
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
            charController.Move(move * currentSpeed * Time.deltaTime);

            if (Input.GetKeyDown(KeyCode.LeftShift))
            {
                ToggleRunning(true);
            }
            else if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                ToggleRunning(false);
            }
        }

        private void UpdateEnergy()
        {
            if (behaviourDict[ePlayerBehaviour.Running])
            {
                currentSpeed = config.RunSpeed;
                status.DecreaseEnergy(Time.deltaTime * config.DecreaseEnergySpeed);
                if (status.Energy <= 0f)
                {
                    ToggleRunning(false);
                }
            }
            else
            {
                currentSpeed = config.WalkSpeed;
                status.IncreaseEnergy(Time.deltaTime * config.IncreaseEnergySpeed);
            }
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
                    isCursorLocked = !isCursorLocked;
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
            // // Aiming
            // if (Input.GetKeyDown(KeyCode.T))
            // {
            //     animator?.SetBool(HashAnimatorAim, !animator.GetBool(HashAnimatorAim));
            // }
            
            // Shooting
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                ToggleShooting(true);
            }
            else if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                ToggleShooting(false);
            }
            if (behaviourDict[ePlayerBehaviour.Shooting] && !IsShootingAvailable()) // Out of ammo
            {
                PlaySound(SoundID.SFX_ZS_PLAYER_EMPTY_AMMO);
                ToggleShooting(false);
            }

            // Reload
            if (Input.GetKeyDown(KeyCode.R))
            {
                if (!behaviourDict[ePlayerBehaviour.Reloading])
                {
                    HandleReload();
                }
            }

            // Switch weapon
            if (Input.GetKeyDown(KeyCode.T) && !behaviourDict[ePlayerBehaviour.Reloading] && !behaviourDict[ePlayerBehaviour.Shooting])
            {
                HandleSwitchWeapon();
            }

            // Running
            if (Mathf.Abs(moveLR) > 0f || Mathf.Abs(moveFB) > 0f)
            {
                ToggleMoving(true);
            }
            else
            {
                ToggleMoving(false);
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
        private void HandleReload()
        {
            if (currentWeapon != null && currentWeapon.TryFillAmmoClip())
            {
                behaviourDict[ePlayerBehaviour.Reloading] = true;
                animator?.SetTrigger(HashAnimatorReload);
                PlaySound(SoundID.SFX_ZS_PLAYER_RELOAD);
                ZombieShooterUI.Instance.Crosshair.SwitchCrosshairState(GetCurrentCrosshairState(), 0.1f);
                DOVirtual.DelayedCall(config.ReloadTime, () => {
                    behaviourDict[ePlayerBehaviour.Reloading] = false;
                    ZombieShooterUI.Instance.Crosshair.SwitchCrosshairState(GetCurrentCrosshairState());
                });
            }
        }

        private void HandleSwitchWeapon()
        {
            if (weaponList != null && weaponList.Count > 0)
            {
                if (currentWeapon == null)
                {
                    currentWeapon = weaponList[0];
                    currentWeapon.OnWeaponActive();
                }
                else
                {
                    currentWeapon.OnWeaponSwitched();
                    int currentIndex = weaponList.IndexOf(currentWeapon);
                    int nextIndex = currentIndex + 1;
                    if (nextIndex < weaponList.Count)
                    {
                        currentWeapon = weaponList[nextIndex];
                    }
                    else
                    {
                        currentWeapon = weaponList[0];
                    }
                    currentWeapon.OnWeaponActive();
                }
            }
        }

        public void HandleZombieHit(float amount)
        {
            if (status.Health <= 0) return;
            PlaySound(SoundID.SFX_ZS_ZOMBIE_SPLAT);
            status.DecreaseHealth(Mathf.CeilToInt(amount));
            ZombieShooterUI.Instance.SpawnRandomSplatter();
            if (status.Health <= 0) // Dead
            {
                HandleDeath();
            }
        }

        private void HandleDeath()
        {
            PlaySound(SoundID.SFX_ZS_PLAYER_DEATH);
            IsPlaying = false;
            CameraManager.Instance.LiveVirtualCamera = eVirtualCamera.PLAYER_GAMEOVER;
            Vector3 pos = new Vector3(this.transform.position.x,
                                        Terrain.activeTerrain.SampleHeight(this.transform.position),
                                        this.transform.position.z);
            GameObject steve = Instantiate(stevePrefab, pos, this.transform.rotation, this.transform);
            steve.GetComponent<Animator>().SetTrigger(HashAnimatorDead);
            modelContainer.gameObject.SetActive(false);
            ActiveGameOverCamera();
            ZombieShooterManager.ON_END_GAME?.Invoke();
        }

        public void HandleWin()
        {
            IsPlaying = false;
            CameraManager.Instance.LiveVirtualCamera = eVirtualCamera.PLAYER_GAMEOVER;
            Vector3 pos = new Vector3(this.transform.position.x,
                                        Terrain.activeTerrain.SampleHeight(this.transform.position),
                                        this.transform.position.z);
            GameObject steve = Instantiate(stevePrefab, pos, this.transform.rotation, this.transform);
            steve.GetComponent<Animator>().SetTrigger(HashAnimatorDance);
            modelContainer.gameObject.SetActive(false);
            ActiveGameOverCamera();
        }

        private void ActiveGameOverCamera()
        {
            CameraManager.Instance.LiveVirtualCamera = eVirtualCamera.PLAYER_GAMEOVER;
            var gameOverCam = CameraManager.Instance.GetVirtualCamera(eVirtualCamera.PLAYER_GAMEOVER);
            gameOverCam.Follow = this.playerBack.transform;
            gameOverCam.LookAt = this.playerBack.transform;

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
            return currentWeapon != null && currentWeapon.IsShootAvailable() && !behaviourDict[ePlayerBehaviour.Reloading];
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

            behaviourDict[ePlayerBehaviour.Shooting] = toggle;
            animator?.SetBool(HashAnimatorFire, toggle);
            if (currentWeapon)
            {
                currentWeapon.IsShooting = toggle;
            }
            ZombieShooterUI.Instance.Crosshair.SwitchCrosshairState(GetCurrentCrosshairState());
        }

        private void ToggleMoving(bool toggle)
        {
            behaviourDict[ePlayerBehaviour.Moving] = toggle;
            animator?.SetBool(HashAnimatorRun, toggle);
            if (!toggle)
            {
                ResetFootStepTimer();
            }
            ZombieShooterUI.Instance.Crosshair.SwitchCrosshairState(GetCurrentCrosshairState());
        }

        private void ToggleRunning(bool toggle)
        {
            behaviourDict[ePlayerBehaviour.Running] = toggle;
        }

        private eCrosshairState GetCurrentCrosshairState()
        {
            if (behaviourDict[ePlayerBehaviour.Reloading])
            {
                return eCrosshairState.RELOAD;
            }
            else if (behaviourDict[ePlayerBehaviour.Shooting])
            {
                return eCrosshairState.SHOOT;
            }
            else if (behaviourDict[ePlayerBehaviour.Moving])
            {
                return eCrosshairState.RUN;
            }

            return eCrosshairState.IDLE;
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
                        status.IncreaseHealth(medKit.Value);
                    }
                break;
                case eItemType.Ammo:
                    var ammo = item as AmmoItem;
                    if (ammo)
                    {
                        FillWeaponListAmmo(ammo.AmmoType, ammo.Value);
                    }
                break;
            }
        }

        private void FillWeaponListAmmo(eWeaponType type, int value)
        {
            if (weaponList != null)
            {
                weaponList.ForEach(weapon => {
                    if (weapon.Type == type)
                    {
                        weapon.IncreaseAmmo(value);
                    }
                });
            }
        }
#endregion

#region Audio
        private float currentTimeBetweenFootStep;

        private void PlaySound(SoundID id)
        {
            SoundManager.Instance?.PlaySound(id);
        }

        private void ResetAudioTimer()
        {
            ResetFootStepTimer();
        }

        private void ResetFootStepTimer()
        {
            currentTimeBetweenFootStep = timeBetweenFootStep;
        }

        private void UpdateSoundLoop()
        {
            if (behaviourDict[ePlayerBehaviour.Moving] && behaviourDict[ePlayerBehaviour.Grounded])
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

#region Vfx
        private void PlayZombieBloodVfx(RaycastHit hitInfo)
        {
            if (bloodVfxPrefab)
            {
                GameObject blood = bloodVfxPrefab.Spawn(hitInfo.point, Quaternion.identity);
                blood.transform.LookAt(this.transform.position);
                DOVirtual.DelayedCall(1f, () => blood.Despawn());
            }
        }
#endregion
    }
}