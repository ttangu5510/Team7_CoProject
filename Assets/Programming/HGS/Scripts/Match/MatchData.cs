using System;
using UnityEngine;

namespace SHG
{

  /// <summary>
  /// 각 경기에 대한 정보
  /// </summary>
  [Serializable]
  public struct MatchData 
  {

    /// <summary>
    /// 기본 스포츠 종목: 스키점프, 스켈레톤, 피겨 스케이팅, 스피드 스케이팅
    /// </summary>
    public static SportType[] DefaultSports = new SportType[] {
      SHG.SportType.SkiJumping,
      SHG.SportType.Skeleton,
      SHG.SportType.FigureSkating,
      SHG.SportType.SpeedSkating
    };

    /// <summary> 단일 종목인 경우 해당 종목의 출전 선수 수 </summary>
    public static int NumberOfAthletesInSingleSport = 7;

    [SerializeField]
    public MatchType MatchType;

    /// <summary> 단일 종목인 경우의 해당 종목 </summary>
    [SerializeField]
    public SportType SportType; 
    [SerializeField]
    public Country[] MemberContries;
    [SerializeField]
    public (ResourceType type, int amount)[] Rewards;
    [SerializeField]
    public string Name;
    [SerializeField]
    public GameDate DateOfEvent;
    public bool IsMandatory => (this.MatchType != MatchType.Friendly);
    public bool IsSingleSport => (this.MatchType == MatchType.SingleSport);

    public static string GetSportTypeString(SportType sportType)
    {
      switch (sportType) {
        case SHG.SportType.SkiJumping:
          return ("스키점프");
        case SHG.SportType.Skeleton:
          return ("스켈레톤");
        case SHG.SportType.SpeedSkating:
          return ("스피드 스케이팅");
        case SHG.SportType.FigureSkating:
          return ("피겨 스케이팅");
        case SHG.SportType.Biathlon:
          return ("바이애슬론");			
        case SHG.SportType.IceHockey:
          return ("아이스 하키");
        case SHG.SportType.SnowBoard:
          return ("스노우 보드");			
        case SHG.SportType.AlpineSkiing:
          return ("알파인 스키");			
        default: 
          return (string.Empty);
      }
    }

    public override bool Equals(object obj) {
      if (obj is MatchData other) {
        return (this == other);
      }
      return (false);
    }

    public override int GetHashCode() {
      return (this.Name.GetHashCode() + this.DateOfEvent.GetHashCode());
    }

    public override string ToString() {
      return ($"[{nameof(MatchData)}; {nameof(Name)}: {this.Name}; {nameof(DateOfEvent)}: {this.DateOfEvent}; {nameof(SportType)}: {this.SportType}]");
    }

    public static bool operator== (MatchData matchA, MatchData matchB) {
      return (matchA.Name == matchB.Name &&
        matchA.DateOfEvent == matchB.DateOfEvent);
    }

    public static bool operator!= (MatchData matchA, MatchData matchB) {
      return (!(matchA == matchB));
    }
  }

  /// <summary>
  /// 경기 종류: 단일 종목 경기, 친선 경기, 국내 경기, 국제 경기
  /// </summary>
  public enum MatchType
  {
    SingleSport,
    Friendly,
    Domestic,
    International
  }

  /// <summary>
  /// 스포츠 종목: 스키점프, 스켈레톤, 피겨 스케이팅, 스피드 스케이팅 
  /// (+ 스노 보드, 알파인 스키, 바이애슬론, 아이스 하키)
  /// </summary>
  public enum SportType
  {
    SkiJumping,
    Skeleton,
    FigureSkating,
    SpeedSkating,
    SnowBoard,
    AlpineSkiing,
    Biathlon,
    IceHockey
  }

  /// <summary>
  /// 경기에 출전하는 국가
  /// </summary>
  [Serializable]
  public struct Country
  {
    [SerializeField]
    public string Name;

    public override bool Equals(object obj) {
      if (obj is Country other) {
        return (this == other);
      }
      return (false);
    }

    public static bool operator== (Country countryA, Country countryB) {
      return (countryA.Name == countryB.Name);
    }

    public static bool operator!= (Country countryA, Country countryB) {
      return (!(countryA == countryB));
    }

    public override int GetHashCode() {
      return (this.Name.GetHashCode());
    }

    public override string ToString() {
      return ($"[{nameof(Country)}; {nameof(Name)}: {this.Name};]");
    }
  }
}
