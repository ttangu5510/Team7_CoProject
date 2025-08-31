using System.Collections.Generic;
using Defective.JSON;
using UnityEngine;
using JYL;

namespace SHG
{

  public class ContendersController : IContenderController
  {
    const int MIN_ATHLETE_COUNT_FOR_TEAM = 4;
    static readonly string[] COUNTRY_NAMES = new string[]{
      "america", "china", "germany", "greece", "hungary", "japan", "norway"
    };

    static string[] TEAM_NAMES = new string[] {
      "서울시청", "강원 루지", " 전남체육회", "춘천시청", "강릉시청", "횡성군청", "평창군청"
    };
    public static Dictionary<string, Sprite> FLAG_ICONS { get; private set; }
    const string FLAG_ICON_DIR = "Flags";

    static ContendersController()
    {
      FLAG_ICONS = new ();
      foreach (var countryName in ContendersController.COUNTRY_NAMES) {
        FLAG_ICONS.Add(countryName, Resources.Load<Sprite>(
            $"{FLAG_ICON_DIR}/{countryName}"));
      }
      FLAG_ICONS.Add("korea", Resources.Load<Sprite>($"{FLAG_ICON_DIR}/korea"));
    }


    public Dictionary<IGroup, List<IContender>> Althetes { get; private set; }

    public Team[] Teams => this.teams;
    [SerializeField]
    Team[] teams;


    public ContendersController() {
      this.Althetes = new();
      
      var dir = "AthleteData";
      foreach (var countryName in COUNTRY_NAMES) {
        List<IContender> countryAthletes = new();
        string json = Resources.Load<TextAsset>($"{dir}/{countryName}").text;
        var jsonObject = new JSONObject(json);
        foreach (var athlete in jsonObject.list) {
          countryAthletes.Add(
            new AthleteData { 
            CountryName = countryName,
            Name = athlete["선수 이름"].stringValue,
            Grade = athlete["선수 등급"].stringValue,
            Fatigue = athlete["피로도"].floatValue,
            Id = athlete["선수 ID"].intValue,
            stats = new AthleteStats (
              health: athlete["체력"].intValue,
              quickness: athlete["순발력"].intValue,
              flexibility: athlete["유연성"].intValue,
              technic: athlete["기술"].intValue,
              speed: athlete["속도"].intValue,
              balance: athlete["균형감각"].intValue
              )
            });
        }
        this.Althetes.Add(
          new Country { Name = countryName }, countryAthletes);
      }

      List<AthleteData> koreaAthletes = new ();
      string koreaJson = Resources.Load<TextAsset>($"{dir}/korea_contenders").text;
        var koreaJsonObject = new JSONObject(koreaJson);
        foreach (var athlete in koreaJsonObject.list) {
          koreaAthletes.Add( 
            new AthleteData { 
            CountryName = "korea",
            Name = athlete["선수 이름"].stringValue,
            Grade = athlete["선수 등급"].stringValue,
            Fatigue = athlete["피로도"].floatValue,
            Id = athlete["선수 ID"].intValue,
            stats = new AthleteStats (
              health: athlete["체력"].intValue,
              quickness: athlete["순발력"].intValue,
              flexibility: athlete["유연성"].intValue,
              technic: athlete["기술"].intValue,
              speed: athlete["속도"].intValue,
              balance: athlete["균형감각"].intValue
              )
            });
        }

        this.ShuffleAhtletes(koreaAthletes);
        int teamCount = 0;
        int teamAthletesCount = 0;
        for (teamCount = TEAM_NAMES.Length; teamCount > 0; teamCount--) {
          teamAthletesCount = koreaAthletes.Count / teamCount;
          if (teamAthletesCount >= MIN_ATHLETE_COUNT_FOR_TEAM) {
            break;
          } 
        }

        this.teams = new Team[teamCount];

        for (int i = 0; i < teamCount - 1; i++) {
          List<IContender> teamAthletes = new (teamAthletesCount);
          var team = new Team { Name = TEAM_NAMES[i] };
          this.teams[i] = team;
          for (int j = 0; j < teamAthletesCount; j++) {
            var index = koreaAthletes.Count - 1;
            var athlete = koreaAthletes[index];
            athlete.Team = team;
            teamAthletes.Add(athlete);
            koreaAthletes.RemoveAt(index);
          }
          this.Althetes.Add(team, teamAthletes);
        }

        var lastTeam = new Team { Name = TEAM_NAMES[teamCount - 1]};
        this.teams[this.teams.Length - 1] = lastTeam;
        var lastTeamAthletes = new List<IContender>();
        for (int i = 0; i < koreaAthletes.Count; i++) {
          var athlete = koreaAthletes[i];
          athlete.Team = lastTeam;
          lastTeamAthletes.Add(athlete);
        }
        this.Althetes.Add(
          new Team { Name = TEAM_NAMES[teamCount - 1]}, lastTeamAthletes);
    }

    void ShuffleAhtletes<T>(List<T> list) where T: IContender
    {
      System.Random random = new ();  
      int n = list.Count;  

      for (int i= list.Count - 1; i > 1; i--) {
        int rnd = random.Next(i + 1);  

        T temp = list[rnd];  
        list[rnd] = list[i];  
        list[i] = temp;
      }
    }
  }
}
