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

#if UNITY_EDITOR
        public GameHeader headerEditor = new GameHeader() { header = "Components" };
#endif

        [SerializeField] private GameObject modelContainer;
        [SerializeField] private Animator animator;
        [SerializeField] private CharacterController charController;
        [SerializeField] private CinemachineVirtualCamera playerCam;

#if UNITY_EDITOR
        public GameHeader headerEditor1 = new GameHeader() { header = "Params" };
#endif
        [SerializeField] private float speed = 10f;
        [SerializeField] private float jumpForce = 2f;
        [SerializeField] private float mouseXSensitivity = 5f;
        [SerializeField] private float mouseYSensitivity = 5f;
        [SerializeField] private float maxYAngle = 80f;
        [SerializeField] private float gravity = -30f;

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

        public void Initialize()
        {
            IsPlaying = false;
            isCursorLocked = true;
            isGrounded = false;
            isRunning = false;
            isShooting = false;
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
                UpdateAnimation();
                UpdateSoundLoop();
            }
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

#region Player Physic/Input
        private void CheckOnGround()
        {
            var newCheck = IsGrounded();
            if (isGrounded != newCheck)
            {
                isGrounded = newCheck;
                if (isGrounded)
                {
                    PlayLandSound();
                }
                else
                {
                    PlayJumpSound();
                }
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
                // mRigidbody?.AddForce(Vector3.up * jumpForce);
                velocity.y = Mathf.Sqrt(jumpForce * -2.0f * gravity);
            }

            velocity.y += gravity * Time.deltaTime;
            charController.Move(velocity * Time.deltaTime);
        }

        private void UpdateMovement()
        {
            moveLR = Input.GetAxis("Horizontal");
            moveFB = Input.GetAxis("Vertical");

            Vector3 move = this.transform.right * moveLR + this.transform.forward * moveFB;
            charController.Move(move * speed * Time.deltaTime);
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

        private void UpdateAnimation()
        {
            if (animator)
            {
                if (Input.GetKeyDown(KeyCode.T))
                {
                    animator.SetBool(HashAnimatorAim, !animator.GetBool(HashAnimatorAim));
                }
                
                if (Input.GetKeyDown(KeyCode.Mouse0))
                {
                    ToggleShooting(true);
                }
                else if (Input.GetKeyUp(KeyCode.Mouse0))
                {
                    ToggleShooting(false);
                }

                if (Input.GetKeyDown(KeyCode.R))
                {
                    animator.SetTrigger(HashAnimatorReload);
                }

                if (Mathf.Abs(moveLR) > 0f || Mathf.Abs(moveFB) > 0f)
                {
                    ToggleRunning(true);
                }
                else
                {
                    ToggleRunning(false);
                }
            }
        }

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if (hit.gameObject.CompareTag(Utilities.ITEM_TAG))
            {
                if (hit.gameObject.TryGetComponent<ShooterItem>(out var item))
                {
                    CheckItemEffect(item);
                    item.Consumed = true;
                }
            }
        }

        private void ToggleShooting(bool toggle)
        {
            isShooting = toggle;
            animator.SetBool(HashAnimatorFire, toggle);
            if (!toggle)
            {
                ResetShotTimer();
            }
        }

        private void ToggleRunning(bool toggle)
        {
            isRunning = toggle;
            animator.SetBool(HashAnimatorRun, toggle);
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
                break;
                case eItemType.Ammo_Normal:
                    var ammo = item as AmmoItem;
                break;
            }
        }

#endregion

#region Audio

#if UNITY_EDITOR
        public GameHeader headerEditor2 = new GameHeader() { header = "Audio" };
#endif
        [SerializeField] private float timeBetweenFootStep = 0.5f;
        [SerializeField] private float timeBetweenShot = 0.5f;

        private float currentTimeBetweenFootStep;
        private float currentTimeBetweenShot;

        private void PlayJumpSound()
        {
            SoundManager.Instance?.PlaySound(SoundID.SFX_ZS_JUMP);
        }

        private void PlayLandSound()
        {
            SoundManager.Instance?.PlaySound(SoundID.SFX_ZS_LAND);
        }

        private void PlayFootStepSound()
        {
            SoundManager.Instance?.PlaySound(SoundID.SFX_ZS_FOOTSTEP);
        }

        private void PlayShotSound()
        {
            SoundManager.Instance?.PlaySound(SoundID.SFX_ZS_SHOT);
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

        private void ResetShotTimer()
        {
            currentTimeBetweenShot = timeBetweenShot;
        }

        private void UpdateSoundLoop()
        {
            if (isShooting)
            {
                if (currentTimeBetweenShot <= 0)
                {
                    ResetShotTimer();
                    PlayShotSound();
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
                    PlayFootStepSound();
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