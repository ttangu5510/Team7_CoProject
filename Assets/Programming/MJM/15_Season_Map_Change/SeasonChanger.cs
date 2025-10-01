using UnityEngine;
using UniRx;
using JWS;

public class SeasonChanger : MonoBehaviour
{
    private int year = 1;
    private Season currentSeason = Season.Spring;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ChangeSeason(Season.Spring);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            ChangeSeason(Season.Summer);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            ChangeSeason(Season.Autumn);
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            ChangeSeason(Season.Winter);
        }
    }

    private void ChangeSeason(Season newSeason)
    {
        currentSeason = newSeason;
        MessageBroker.Default.Publish(new SeasonChangedEvent(year, newSeason));
        Debug.Log($"[SeasonChanger] {newSeason} 으로 변경됨");
    }
}
