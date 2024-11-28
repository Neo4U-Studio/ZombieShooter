using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ZombieShooter
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private Rigidbody rigibody;

        [SerializeField] private float speed = 0.1f;

        // Start is called before the first frame update
        void Start()
        {
            
        }

        // Update is called once per frame
        void Update()
        {
            
        }

        private void FixedUpdate() {
            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");

            this.transform.position += new Vector3(x * speed, 0, z * speed);
        }
    }
}