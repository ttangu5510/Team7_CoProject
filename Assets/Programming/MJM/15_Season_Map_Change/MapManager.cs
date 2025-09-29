using UnityEngine;
using UniRx;
using JWS; // Season enum 정의

public class MapManager : MonoBehaviour
{
    [Header("계절별 맵 오브젝트")]
    [SerializeField] private GameObject springSummerMap; // 봄/여름 공용
    [SerializeField] private GameObject autumnMap;
    [SerializeField] private GameObject winterMap;

    private void OnEnable()
    {
        MessageBroker.Default.Receive<SeasonChangedEvent>()
            .Subscribe(evt => UpdateMap(evt.NewSeason))
            .AddTo(this);
    }

    private void Start()
    {
        // 시작 시 봄/여름 맵 활성화
        UpdateMap(Season.Spring);
    }

    private void UpdateMap(Season season)
    {
        springSummerMap.SetActive(season == Season.Spring || season == Season.Summer);
        autumnMap.SetActive(season == Season.Autumn);
        winterMap.SetActive(season == Season.Winter);

        Debug.Log($"[MapManager] {season} 맵으로 전환됨");
    }
}
