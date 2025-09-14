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
        [SerializeField] private List<TreatmentSlotView> slots; // Slot A~H 순서대로
        [Inject] private DomAthService athleteService;
        [Inject] private SHG.IFacilitiesController facilitiesController;

        private void OnEnable()
        {
            Refresh();
            facilitiesController.SelectedFacilityStream
                .Subscribe(_ => Refresh())
                .AddTo(this);
            // 필요 시 MedicalCenter 레벨/슬롯 수 스트림도 구독
        }
        

        public void Refresh()
        {
            var injured = athleteService
                .GetAllRecruitedAthleteList()
                .Where(a => a.curState == AthleteState.Injured)
                .ToList();

            int usable = GetUsableSlots();
            usable = Mathf.Clamp(usable, 0, slots.Count);

            if (injured.Count == 0)
            {
                // 1) 배치 가능한 선수 없음
                for (int i = 0; i < slots.Count; i++)
                {
                    if (i == 0 && i < usable)      slots[i].ShowNoAvailable();
                    else if (i < usable)           slots[i].ShowEmpty();
                    else                           slots[i].ShowLocked();
                }
                return;
            }

            // 2) 부상자 존재
            int bind = Mathf.Min(injured.Count, usable);
            for (int i = 0; i < slots.Count; i++)
            {
                if (i < bind)                     slots[i].ShowAssigned(injured[i]);
                else if (i < usable)              slots[i].ShowEmpty();
                else                               slots[i].ShowLocked();
            }
        }

        private int GetUsableSlots()
        {
            var mc = facilitiesController.MedicalCenter;

            int level = mc.CurrentStage.Value;

            switch (level)
            {
                case 0: return 2; // 2개 개방
                case 1: return 4; // 4개 개방
                case 2: return 6; // 6개 개방
                default: return 8; // 전부 개방
            }
        }


    }
}