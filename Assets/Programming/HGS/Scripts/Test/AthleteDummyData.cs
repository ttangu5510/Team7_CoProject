using System;
using System.IO;
using System.Collections.Generic;
using Defective.JSON;
using UnityEngine;
using JYL;

namespace SHG
{
  [Serializable]
  public struct ParsedAthleteData : IContenderAthlete {
    public string Name;
    public string Grade;
    public float Fatigue;
    public int Id;
    public string CountryName;
    public AthleteStats stats;
    public AthleteStats Stats => this.stats;

    public AthleteAffiliation Level => (this.Grade switch {
        "일반 선수" => (AthleteAffiliation)0,
        "국가대표 후보" => (AthleteAffiliation)1,
        "국가대표" => (AthleteAffiliation)2,
        _ => throw (new ApplicationException())
      });

    public Country Country => (new Country { Name = this.CountryName });

    string IContenderAthlete.Name => (this.Name);
    }

  public class AthleteDummyData {
    public Dictionary<Country, List<IContenderAthlete>> Althetes;
    const string DIRECTORY_PATH = "Programming/HGS/Scripts/Test/AthleteData";
    static string[] COUNTRY_NAMES = new string[]{
      "america", "china", "germany", "greece", "hungary", "japan", "norway"
    };

    public AthleteDummyData() {
      this.Althetes = new();
      var dir = $"{Application.dataPath}/{DIRECTORY_PATH}/";
      foreach (var countryName in COUNTRY_NAMES) {
        List<IContenderAthlete> countryAthletes = new();
        string json = File.ReadAllText($"{dir}/{countryName}.json");
        var jsonObject = new JSONObject(json);
        foreach (var athlete in jsonObject.list) {
          countryAthletes.Add(
            new ParsedAthleteData { 
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
        this.Althetes.Add(new Country { Name = countryName }, 
          countryAthletes);
      }
    }
  }
}
