using System.Collections;
using System.Collections.Generic;
using JYL;
using UnityEngine;
using Zenject;



public class InfoUIController : MonoBehaviour
{
    [Inject]

    DomAthService _DomAthService;

    private void Awake()
    { 
        Debug.Log(PlayerNumber());


    }

    public string PlayerNumber()
    {
        _DomAthService.GetAllRecruitedAthleteList();



        return "야호~";
    }

    public string RetiredPlayerNumber()
    {

        return "";
    }

    public string RetiredCoachNumber()
    {

        return "";
    }

    public string MatchNumber()
    {

        return "";
    }

    public string EarnMedalNumber()
    {

        return "";
    }

    public string FameNumber()
    {

        return "";
    }

    public string AchievementNumber()
    {

        return "";
    }

}

