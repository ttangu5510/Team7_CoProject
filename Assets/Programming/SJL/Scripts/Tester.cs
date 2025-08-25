using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SJL
{
    public class Tester : MonoBehaviour
    {
        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Jump();
            }
        }

        public void Jump()
        {
            Debug.Log("점프!");
        }
    }
}
