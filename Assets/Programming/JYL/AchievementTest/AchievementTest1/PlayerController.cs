using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JYL.AchievementTest01
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float moveSpeed;
    
        private Rigidbody rigid;
        private Collider col;

        private void Awake()
        {
            rigid = GetComponent<Rigidbody>();
            col = GetComponent<Collider>();
            rigid.constraints = RigidbodyConstraints.FreezeRotation;
        }

        void Update()
        {
            Move();
        }

        private void Move()
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            Vector3 moveDirection = new Vector3(horizontal, 0, vertical);
            Debug.Log($"{moveDirection.x}_ {moveDirection.y}_ {moveDirection.z}");
            if(moveDirection.magnitude <= 0.1f) moveDirection = Vector3.zero;
    
            rigid.velocity = moveDirection * moveSpeed;
        }
    }
}
