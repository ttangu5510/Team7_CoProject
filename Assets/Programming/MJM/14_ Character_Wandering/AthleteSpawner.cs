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
        private Dictionary<int, GameObject> activePlayers = new();

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
            // 초기화: 현재 영입된 선수들 모두 배치
            SyncAllRecruited();

            // 이벤트 구독
            MessageBroker.Default.Receive<AthleteRecruitedEvent>()
                .Subscribe(evt => SpawnAthlete(evt.athleteId))
                .AddTo(this);

            MessageBroker.Default.Receive<AthleteOutEvent>()
                .Subscribe(evt => DespawnAthlete(evt.athleteId))
                .AddTo(this);

            MessageBroker.Default.Receive<AthleteRetiredEvent>()
                .Subscribe(evt => DespawnAthlete(evt.athleteId))
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
            if (activePlayers.ContainsKey(id)) return; // 이미 존재

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
}
