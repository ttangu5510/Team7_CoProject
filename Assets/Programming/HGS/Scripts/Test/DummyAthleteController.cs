using System;
using System.Collections.Generic;
using UnityEngine;
using Defective.JSON;
using JYL;
using UniRx;

namespace SHG
{
  public class DummyAthleteController : IAthleteController {

    public static IAthleteController Instance
    {
      get {
        if (instance == null) {
          instance = new DummyAthleteController();
        }
        return (instance);
      }
    }
    static IAthleteController instance;

    [Serializable]
    public struct ParsedUserAthleteData : IContender {
      public string Name;
      public string Grade;
      public float Fatigue;
      public int Id;
      public string CountryName;
      public AthleteStats stats;
      public AthleteStats Stats => this.stats;

      AthleteAffiliation IContender.Grade => (this.Grade switch {
        "일반 선수" => (AthleteAffiliation)0,
        "국가대표 후보" => (AthleteAffiliation)1,
        "국가대표" => (AthleteAffiliation)2,
        _ => throw (new ApplicationException())
        });

      public IGroup Group => (new Country { Name = this.CountryName });
      string IContender.Name => (this.Name);
      int IContender.Id =>  this.Id;

    }
    const string DATA_PATH = "AthleteData/korea";

    public ReactiveProperty<int> NumberOfGeneralAthlete { get; private set; }
    public ReactiveProperty<int> NumberOfNationalAthleteCandidate { get; private set; }
    public ReactiveProperty<int> NumberOfNationalAthlete { get; private set; }
    public ReactiveProperty<int> NumberOfCoach { get; private set; }
    public ReactiveCollection<DomAthEntity> Athletes { get; private set; }
    Dictionary<int, DomAthEntity> athleteTable;

    public DummyAthleteController()
    {
      this.athleteTable = new ();
      this.LoadData();
      this.CountAthletes();
      this.NumberOfCoach = new (1);
    }

    void CountAthletes()
    {
      int numberOfGeneralAthlete = 0;
      int numberOfNationalAthleteCandidate = 0;
      int numberOfNationalAthlete = 0;
      foreach (var athlete in this.Athletes) {
        switch ((int)athlete.affiliation) {
          case 0:
            numberOfGeneralAthlete++;
            break;
          case 1:
            numberOfNationalAthleteCandidate++;
            break;
          case 2:
            numberOfNationalAthlete++;
            break;
        } 
      }
      this.NumberOfGeneralAthlete = new (numberOfGeneralAthlete);
      this.NumberOfNationalAthleteCandidate  = new (numberOfNationalAthleteCandidate);
      this.NumberOfNationalAthlete = new (numberOfNationalAthlete);
    }

    void LoadData()
    {
      var path = $"{Application.dataPath}/{DATA_PATH}";
      List<DomAthEntity> athletes = new ();
      string json = Resources.Load<TextAsset>(DATA_PATH).text;
      var jsonObject = new JSONObject(json);
      foreach (var athleteData in jsonObject.list) {
        DomAthEntity athlete = new DomAthEntity();
        athlete.Init(
          id: athleteData["선수 ID"].intValue,
          name: athleteData["선수 이름"].stringValue,
          affiliation: this.GetAffliation(athleteData["선수 등급"].stringValue),
          maxGrade: AthleteGrade.A,
          recruitAge: 18,
          health: athleteData["체력"].intValue,
          quickness: athleteData["순발력"].intValue,
          flexibility: athleteData["유연성"].intValue,
          technic: athleteData["기술"].intValue,
          speed: athleteData["속도"].intValue,
          balance: athleteData["균형감각"].intValue
          );
        athletes.Add(athlete);
        this.athleteTable.Add(athlete.id, athlete);
      }
      this.Athletes = new (athletes);
    }

    public bool TryGetAthleteBy(int id, out DomAthEntity athlete)
    {
      return (this.athleteTable.TryGetValue(id, out athlete));
    }

    AthleteAffiliation GetAffliation(string grade)
    {
       return (grade switch {
        "일반 선수" => (AthleteAffiliation)0,
        "국가대표 후보" => (AthleteAffiliation)1,
        "국가대표" => (AthleteAffiliation)2,
        _ => throw (new ApplicationException())
        });
    }
  }
}
