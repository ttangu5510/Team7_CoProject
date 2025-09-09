using UnityEngine;
using System.Collections.Generic;
using MMJ;

public class PlayerDexManager : MonoBehaviour
{
    public Transform contentParent;
    public GameObject playerItemPrefab;
    public Sprite silhouetteSprite;

    private List<PlayerData> allPlayers = new List<PlayerData>();

    void Start()
    {
        LoadPlayers();
        PopulateDex();
    }

    void LoadPlayers()
    {
        for (int i = 0; i < 99; i++)
        {
            bool met = Random.value > 0.5f;  // 예시: 랜덤하게 만난 선수
            allPlayers.Add(new PlayerData
            {
                playerName = $"선수 {i + 1}",
                image = GetSomePlayerSprite(i), // 이미지 로딩 함수
                isMet = met
            });
        }
    }

    void PopulateDex()
    {
        foreach (var data in allPlayers)
        {
            GameObject obj = Instantiate(playerItemPrefab, contentParent);
            var item = obj.GetComponent<PlayerItem>();
            item.silhouetteSprite = silhouetteSprite;
            item.SetData(data);
        }
    }

    Sprite GetSomePlayerSprite(int index)
    {
        // 실제 게임에서는 Resources.Load<Sprite>("...") 또는 Addressables 사용
        return null;
    }
}
