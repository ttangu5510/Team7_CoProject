using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Zenject;
using UniRx;
using JYL;
using SHG;

namespace JWS{
    public class TreatmentRoomTabView : MonoBehaviour
    {
        [SerializeField] private List<TreatmentSlotView> slots; // Slot A~H를 순서대로 드래그
        [SerializeField] private string lockedLabel = "사용 불가\n시설 업그레이드 필요";

        [Inject] private DomAthService athleteService;
        [Inject] private SHG.IFacilitiesController facilitiesController;

        private void OnEnable()
        {
            Refresh();
            SubscribeFacilityChanges(); // 시설 변화에 반응
        }

        public void Refresh()
        {
            // 1) 부상자만 뽑기
            var injured = athleteService
                .GetAllRecruitedAthleteList()
                .Where(a => a.curState == AthleteState.Injured)
                .ToList();

            // 2) 시설이 허용하는 사용 가능 슬롯 수
            int usableSlots = GetMedicalUsableSlots();
            usableSlots = Mathf.Clamp(usableSlots, 0, slots.Count);

            // 3) 슬롯 갱신(고정 자리 A~H)
            for (int i = 0; i < slots.Count; i++)
            {
                if (i < injured.Count && i < usableSlots)
                    slots[i].ShowAssigned(injured[i]);   // 선수 바인딩
                else if (i < usableSlots)
                    slots[i].ShowEmpty();                 // 빈 슬롯
                else
                    slots[i].ShowLocked();     // 잠금
            }
        }

        // --- 시설 연동 ---

        private int GetMedicalUsableSlots()
        {
            // 프로젝트 MedicalCenter에 실제로 존재하는 속성명으로 교체해.
            // 흔한 케이스들 대비: UsableSlots / OpenSlots / TreatableSlots / Level 기반 계산 등
            var mc = facilitiesController.MedicalCenter;

            // 예1) ReactiveProperty<int> UsableSlots
            // return mc.UsableSlots.Value;

            // 예2) ReactiveProperty<int> OpenSlots
            // return mc.OpenSlots.Value;

            // 예3) Level 기반 규칙(예: 레벨당 2칸)
            // return mc.Level.Value * 2;

            // 예4) 없다면 전부 오픈(임시)
            return slots.Count;
        }

        private void SubscribeFacilityChanges()
        {
            var mc = facilitiesController.MedicalCenter;

            // 여기도 실제 속성명으로 교체해서 구독해.
            // 예) mc.UsableSlots.Subscribe(_ => Refresh()).AddTo(this);
            // 예) mc.Level.Subscribe(_ => Refresh()).AddTo(this);

            // 시설 탭에서 선택만 바뀌어도 새로고침하고 싶으면:
            facilitiesController.SelectedFacilityStream
                .Subscribe(_ => Refresh())
                .AddTo(this);
        }
    }
}