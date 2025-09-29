using UnityEngine;
using System.Collections.Generic;
using Zenject;

namespace JYL
{
    public class AthleteSpawner : MonoBehaviour
    {
        [Inject] private DomAthService domAthService;

        // 선수 이름과 프리팹 연결 (Inspector에서 할당)
        public List<PlayerPrefabData> playerPrefabs;

        private Dictionary<string, GameObject> prefabDict = new();

        void Awake()
        {
            foreach (var data in playerPrefabs)
            {
                if (data.prefab != null && !string.IsNullOrEmpty(data.athleteName))
                {
                    prefabDict[data.athleteName] = data.prefab;
                }
            }
        }

        void Start()
        {
            SpawnRecruitedAthletes();
        }

        void SpawnRecruitedAthletes()
        {
            // DomAthService를 통해 영입된 선수 목록 가져오기
            List<DomAthEntity> recruited = domAthService.GetAllRecruitedAthleteList();

            foreach (var athlete in recruited)
            {
                // 은퇴 선수는 제외
                if (athlete.curState == AthleteState.Retired) continue;

                // 선수 이름으로 프리팹 찾기
                if (prefabDict.TryGetValue(athlete.entityName, out var prefab))
                {
                    // 프리팹 생성
                    GameObject go = Instantiate(prefab);
                    go.name = athlete.entityName;

                    // 시작 위치 = 랜덤 건물 웨이포인트
                    Waypoint start = FindRandomBuilding();
                    if (start != null)
                    {
                        var wander = go.GetComponent<PlayerWander>();
                        if (wander != null)
                        {
                            wander.currentWaypoint = start;
                        }
                        else
                        {
                            Debug.LogWarning($"{athlete.entityName} 프리팹에 PlayerWander 스크립트가 없습니다.");
                        }
                    }
                }
                else
                {
                    Debug.LogWarning($"프리팹이 없는 선수: {athlete.entityName}");
                }
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
        public string athleteName;
        public GameObject prefab;
    }
}
