using UnityEngine;
using System.Collections.Generic;

public class AthleteManager : MonoBehaviour
{
    public List<GameObject> recruitedPlayers;

    public void RecruitPlayer(GameObject player)
    {
        if (!recruitedPlayers.Contains(player))
        {
            recruitedPlayers.Add(player);
            player.SetActive(true); // 맵 안에서 배회 시작
        }
    }
}
