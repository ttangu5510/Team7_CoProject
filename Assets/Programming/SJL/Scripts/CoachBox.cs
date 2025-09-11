using JYL;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Zenject;


namespace SJL
{
    public class CoachBox : MonoBehaviour
    {
        [Header("Designated coach image")] // 지정된 코치 이미지
        [SerializeField] private Button Routine1;
        [SerializeField] private Button Routine2;
        [SerializeField] private Button Routine3;
        [SerializeField] private Button Routine4;

        [Inject] private CoachService coachService;

        

    }
}
