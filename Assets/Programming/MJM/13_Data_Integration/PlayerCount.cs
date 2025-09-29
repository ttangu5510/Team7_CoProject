using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JYL;
using TMPro;
using UnityEngine;
using Zenject;

namespace MMJ
{
    public class PlayerCount : MonoBehaviour
    {
        [Inject] private DomAthService domAthService;

        [SerializeField] TextMeshProUGUI AthleteCountText;

        private void OnEnable()
        {
            Refresh();
        }

        private void Refresh()
        {
            // 현재 보유 선수 수
            int athleteCount = domAthService.GetAllRecruitedAthleteList().Count(ath => ath.curState != AthleteState.Retired);
            AthleteCountText.text = $"{athleteCount}/20";
        }
    }
}