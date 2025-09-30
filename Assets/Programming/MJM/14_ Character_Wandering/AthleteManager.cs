using UnityEngine;
using System.Collections.Generic;
using Zenject;
using UniRx;
using JYL;

namespace MMJ
{
    public class AthleteManager : MonoBehaviour
    {
        [Header("Set Athlete Prefab")] 
        [SerializeField] private GameObject athletePrefabs;
        [Inject] private DomAthService domAthService;

        [System.Serializable]
        public class PlayerObjectData
        {
            public int athleteId;
            public GameObject playerObject;
        }

        public List<PlayerObjectData> playerObjects; // Inspector에서 할당
        private Dictionary<int, GameObject> playerDict = new();

        void Awake()
        {
            foreach (var data in playerObjects)
            {
                if (data.playerObject != null)
                {
                    playerDict[data.athleteId] = data.playerObject;
                    data.playerObject.SetActive(false); // 초기엔 모두 비활성화
                }
            }
        }

        void Start()
        {
            // 시작 시 현재 영입 상태 반영
            SyncAllRecruited();

            // 이벤트 구독
            MessageBroker.Default.Receive<AthleteRecruitedEvent>()
                .Subscribe(evt => ActivatePlayer(evt.athleteId))
                .AddTo(this);

            MessageBroker.Default.Receive<AthleteOutEvent>()
                .Subscribe(evt => DeactivatePlayer(evt.athleteId))
                .AddTo(this);

            MessageBroker.Default.Receive<AthleteRetiredEvent>()
                .Subscribe(evt => DeactivatePlayer(evt.athleteId))
                .AddTo(this);
        }

        void SyncAllRecruited()
        {
            var recruited = domAthService.GetAllRecruitedAthleteList();
            foreach (var athlete in recruited)
            {
                if (athlete.curState == AthleteState.Retired) continue;
                ActivatePlayer(athlete.id);
            }
        }

        void ActivatePlayer(int id)
        {
            if (playerDict.TryGetValue(id, out var obj))
            {
                obj.SetActive(true);
            }
        }

        void DeactivatePlayer(int id)
        {
            if (playerDict.TryGetValue(id, out var obj))
            {
                obj.SetActive(false);
            }
        }

        [ContextMenu("Set Athletes")]
        private void SetAthletePrefabs()
        {
            playerObjects.Clear();
            foreach (Rigidbody ath in athletePrefabs.GetComponentsInChildren<Rigidbody>())
            {
                playerObjects.Add(new PlayerObjectData() { athleteId = int.Parse(ath.gameObject.name), playerObject = ath.gameObject });
            }
        }
    }
}
