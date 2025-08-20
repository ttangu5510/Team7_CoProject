using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JYL
{
    public class DomAthService : MonoBehaviour
    {
        public DomAthRP rp;

        public void Init() // 초기화
        {
            rp = new DomAthRP();
            rp.Init();
        }
    }
}
