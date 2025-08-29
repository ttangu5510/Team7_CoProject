using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;

namespace SJL
{
    public class MVPTester : MonoBehaviour
    {
        [SerializeField] TMP_Text textUI;

        public MVPModel model;

        public void Awake()
        {
            model.Count
                .Where(value => value % 2 == 0) // .Where() : 조건에 대한 정의
                .Subscribe(value => textUI.text = value.ToString()); // .Sebscribe() : 동작에 대한 정의 => 처음값을 바로 세팅
        }
    }
}
