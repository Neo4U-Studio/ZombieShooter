using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

namespace ZombieShooter
{
    public class PlayerController : MonoBehaviour
    {
        public static readonly int HashAnimatorIdle = Animator.StringToHash("Idle");
        public static readonly int HashAnimatorRun = Animator.StringToHash("Run");
        public static readonly int HashAnimatorAim = Animator.StringToHash("Aim");

        [SerializeField] private GameObject modelContainer;
        [SerializeField] private Animator animator;
        [SerializeField] private CharacterController charController;
        [SerializeField] private CinemachineVirtualCamera playerCam;

        [SerializeField] private float speed = 10f;
        [SerializeField] private float jumpForce = 2f;
        [SerializeField] private float mouseXSensitivity = 5f;
        [SerializeField] private float mouseYSensitivity = 5f;
        [SerializeField] private float maxYAngle = 80f;
        [SerializeField] private float gravity = -30f;
        
        private float xCamRotation = 0f; // Vertical rotation of the camera
        private float yCamRotation = 0f; // Horizontal rotation of the camera
        private Vector3 velocity;
        private bool isGrounded;

        private bool activeLockCursor;
        private bool isCursorLocked = false;


        private void Start() {
            isCursorLocked = true;
            ToggleCursorLock(true);
        }

        private void Update()
        {
            isGrounded = IsGrounded();
            UpdateLockCursor();
            UpdateGravity();
            UpdateMovement();
            UpdateCamera();
            UpdateAiming();
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
            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");

            Vector3 move = this.transform.right * x + this.transform.forward * z;
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

        private void UpdateAiming()
        {
            if (Input.GetKeyDown(KeyCode.Mouse0))
            {
                animator?.SetBool(HashAnimatorAim, true);
            }
            else if (Input.GetKeyUp(KeyCode.Mouse0))
            {
                animator?.SetBool(HashAnimatorAim, false);
            }
        }
#endregion
    }
}