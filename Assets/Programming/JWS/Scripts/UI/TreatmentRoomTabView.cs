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

        // 슬롯별 배치 현황(세이브/로드 대상)
        private readonly DomAthEntity[] _assigned = new DomAthEntity[8]; 
        
        // 세이브/로드 용: 슬롯에 배치된 선수 id들 (A→H 순)
        private List<int> _savedAssignedIds = new();
        
        /// <summary>
        /// 세이브에서 불러온 선수 id 목록을 설정
        /// </summary>
        public void SetSavedAssignments(IEnumerable<int> ids)
        {
            _savedAssignedIds = ids?.ToList() ?? new List<int>();
        }
        
        /// <summary>
        /// 현재 슬롯별 선수 id 목록 반환 (세이브용)
        /// </summary>
        public IReadOnlyList<int> GetAssignedIdsForSave()
        {
            var list = new List<int>(_assigned.Length);
            foreach (var ent in _assigned)
                list.Add(ent?.id ?? -1); // 배치 안된 칸은 -1
            return list;
        }
        
        private void OnEnable()
        {
            Refresh();
            
            var mc = facilitiesController.MedicalCenter;
            
            // 업그레이드 단계나 슬롯 수가 변하면 즉시 UI 새로고침
            mc.CurrentStage.Subscribe(_ => Refresh()).AddTo(this);
            mc.NumberOfAthletes.Subscribe(_ => Refresh()).AddTo(this);
        }
        

        /// <summary>
        /// UI 전체 갱신
        /// </summary>
        public void Refresh()
        {
            var all = athleteService.GetAllRecruitedAthleteList();
            var byId = all.ToDictionary(a => a.id, a => a);
            var injuredAll = all.Where(a => a.curState == AthleteState.Injured).ToList();

            int usable = GetUsableSlots();
            usable = Mathf.Clamp(usable, 0, slots.Count);

            // 1) 뒤에서부터 NeedUpgrade
            for (int i = slots.Count - 1; i >= usable; i--)
            {
                slots[i].ShowLocked();
                _assigned[i] = null;
            }

            // 2) 저장된 배치 → 앞에서부터 Player 배치
            var picked = new List<DomAthEntity>();
            var seen = new HashSet<int>();
            foreach (var id in _savedAssignedIds)
            {
                if (picked.Count >= usable) break;
                if (!byId.TryGetValue(id, out var ent)) continue;
                if (ent.curState != AthleteState.Injured) continue;
                if (!seen.Add(id)) continue;
                picked.Add(ent);
            }

            int write = 0;
            for (; write < picked.Count && write < usable; write++)
            {
                var ent = picked[write];
                _assigned[write] = ent;
                slots[write].ShowAssigned(ent);
            }

            // 배치 안 된 부상자 수 계산
            int unassignedInjuredCount = injuredAll.Count - picked.Count;

            // 3) 남은 usable 슬롯 처리
            for (int i = write; i < usable; i++)
            {
                _assigned[i] = null;

                if (unassignedInjuredCount <= 0)
                    slots[i].ShowNoAvailable(); // 배치 가능한 부상자가 0명이면 전부 NoAvailable
                else
                    slots[i].ShowEmpty();       // 그 외에는 Empty
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