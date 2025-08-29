using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

namespace SJL
{
    public class MVPModel : MonoBehaviour
    {
        public ReactiveProperty<int> Count;    // ReactiveProperty : 값이 변경될 때마다 알림을 보내는 속성

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Count.Value++;   // ReactiveProperty의 값을 변경할 때는 .Value를 사용
            }
        }
    }
}
