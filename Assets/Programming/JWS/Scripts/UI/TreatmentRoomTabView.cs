using System.Linq;
using TMPro;
using UnityEngine;
using Zenject;
using JYL;

namespace JWS
{
    /// 의료센터 탭: 그냥 바인딩만.
    public class TreatmentRoomTabView : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private TextMeshProUGUI totalCountText;
        [SerializeField] private TextMeshProUGUI injuredCountText;

        [Header("Slots (하이어라키 슬롯 오브젝트)")]
        [SerializeField] private PlayerSlotView[] slots;

        [Inject] private DomAthService domAthService;

        private void OnEnable()
        {
            Refresh();
        }

        public void Refresh()
        {
            // 1) 전체/부상자 리스트 가져오기
            var all = domAthService.GetAllRecruitedAthleteList();              // 프로젝트에 이미 있는 함수
            var injured = all.Where(a => a.curState == AthleteState.Injured)   // 상태 이름은 프로젝트에 맞춰 조정
                .ToList();

            // 2) 카운트 텍스트
            if (totalCountText)   totalCountText.text   = all.Count.ToString();
            if (injuredCountText) injuredCountText.text = injured.Count.ToString();

            // 3) 슬롯 채우기
            //    - 부상자 있으면 앞에서부터 순서대로 Set
            //    - 남는 슬롯은 Clear
            for (int i = 0; i < slots.Length; i++)
            {
                if (i < injured.Count && slots[i] != null)
                    slots[i].Set(injured[i]);
                else if (slots[i] != null)
                    slots[i].Clear();
            }
        }
    }
}