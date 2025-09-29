using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JYL;
using TMPro;
using UnityEngine;
using Zenject;

namespace MMJ
{
    public class CoachCount : MonoBehaviour
    {
        [Inject] private CoachService coachService;

        [SerializeField] TextMeshProUGUI coachCountText;

        private void OnEnable()
        {
            Refresh();
        }

        private void Refresh()
        {
            int coachCount = coachService.GetRecruitedCoaches().Count();
            coachCountText.text = $"{coachCount}/4";
        }
    }
}