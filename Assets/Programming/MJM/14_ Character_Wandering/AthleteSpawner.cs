using UnityEngine;
using System.Collections.Generic;
using Zenject;
using UniRx;
using JYL;

namespace MMJ
{
    public class AthleteSpawner : MonoBehaviour
    {
        [Inject] private DomAthService domAthService;

        public List<PlayerPrefabData> playerPrefabs;
        private Dictionary<int, GameObject> prefabDict = new();
        private Dictionary<int, GameObject> activePlayers = new(); // 현재 필드에 배치된 선수(id→오브젝트)

        void Awake()
        {
            foreach (var data in playerPrefabs)
            {
                if (data.prefab != null)
                {
                    prefabDict[data.athleteId] = data.prefab;
                }
            }
        }

        void Start()
        {
            // 초기화: 이미 영입된 선수들 배치
            SyncAllRecruited();

            // 이벤트 구독
            MessageBroker.Default.Receive<AthleteRecruitedEvent>()
                .Subscribe(evt => SpawnAthlete(evt.athleteId))
                .AddTo(this);

            MessageBroker.Default.Receive<AthleteOutEvent>()
                .Subscribe(evt => DespawnAthlete(evt.athleteId))
                .AddTo(this);

            MessageBroker.Default.Receive<AthleteRetiredEvent>()
                .Subscribe(evt => DespawnAthleteByName(evt.athleteName)) // Retired 이벤트는 이름 기반이라면 id로 바꾸는 게 좋음
                .AddTo(this);
        }

        void SyncAllRecruited()
        {
            List<DomAthEntity> recruited = domAthService.GetAllRecruitedAthleteList();
            foreach (var athlete in recruited)
            {
                if (athlete.curState == AthleteState.Retired) continue;
                SpawnAthlete(athlete.id);
            }
        }

        void SpawnAthlete(int id)
        {
            if (activePlayers.ContainsKey(id)) return; // 이미 있음

            if (prefabDict.TryGetValue(id, out var prefab))
            {
                GameObject go = Instantiate(prefab);
                go.name = $"Athlete_{id}";

                Waypoint start = FindRandomBuilding();
                if (start != null)
                {
                    var wander = go.GetComponent<PlayerWander>();
                    if (wander != null) wander.currentWaypoint = start;
                }

                activePlayers[id] = go;
            }
            else
            {
                Debug.LogWarning($"프리팹이 없는 선수 id: {id}");
            }
        }

        void DespawnAthlete(int id)
        {
            if (activePlayers.TryGetValue(id, out var go))
            {
                Destroy(go);
                activePlayers.Remove(id);
            }
        }

        void DespawnAthleteByName(string name)
        {
            // 이름→id 매핑 필요, 아니면 DomAthService.FindByName(name).id 사용
            var entity = domAthService.GetAllAthleteList().Find(x => x.entityName == name);
            if (entity != null) DespawnAthlete(entity.id);
        }

        Waypoint FindRandomBuilding()
        {
            Waypoint[] all = GameObject.FindObjectsOfType<Waypoint>();
            List<Waypoint> buildings = new List<Waypoint>();
            foreach (var wp in all)
            {
                if (wp.isBuilding) buildings.Add(wp);
            }
            if (buildings.Count == 0) return null;
            return buildings[Random.Range(0, buildings.Count)];
        }
    }

    [System.Serializable]
    public class PlayerPrefabData
    {
        public int athleteId;
        public GameObject prefab;
    }

    // 이벤트 구조체 정의
    public struct AthleteRecruitedEvent
    {
        public int athleteId;
        public AthleteRecruitedEvent(int id) { athleteId = id; }
    }

    public struct AthleteOutEvent
    {
        public int athleteId;
        public AthleteOutEvent(int id) { athleteId = id; }
    }
}
