using System;
using System.Collections.Generic;

namespace SHG
{

  public static class MatchDummyData 
  {
    static System.Random RAND = new ();
    public static Country[] DEFAULT_COUNTRIES = new Country[] {
      new Country { Name = "korea" },
      new Country { Name = "norway" },
      new Country { Name = "germany" },
      new Country { Name = "america" },
      new Country { Name = "china" },
      new Country { Name = "japan" },
      new Country { Name = "greece" },
      new Country { Name = "hungary" },
    };
    static Country[] DOMESTIC = new Country[] {
      new Country { Name = "korea" }
    };

    public static string[] AUTONOMY_MATCH_NAMES = new string[] {
        "전국 선수단 친선 대회",
        "동계 연맹 친선 대회",
        "윈터 챌린지 컵",
        "전국 동계 올림피아드",
        "대한 동계 체전",
        "빙설 전국 대회",
        "한마음 동계 친선전",
        "동계 스포츠 페스티벌 대회",
        "동계 그랑프리",
        "윈터 게임즈 대회",
        "무궁화 동계 체전",
        "윈터 클래식 대회",
        "동계 선수권 친선전",
        "태백 동계 친선전",
        "평창 동계 친선전",
    };

    static string DOMESTIC_WINTER_CUP = "전국 동계 스포츠 대회";
    static string PRESIDENT_CUP = "대통령배 전국대회";
    static string WINTER_ASIAN_CUP = "동계 아시안 대회";
    static string WORLD_CHAMPIONSHIP = "세계 선수권 대회";
    static string INTERNATIONAL_WINTER_SPORT = "국제 동계 스포츠 대회";

    public static string GetRandomAutonomyName()
    {
      var index = RAND.Next(0, AUTONOMY_MATCH_NAMES.Length);
      return (AUTONOMY_MATCH_NAMES[index]);
    }
    
    static (ResourceType type, int amount)[] EMPTY_REWARDS = new (ResourceType, int)[0];

    public static MatchData[] DummyData { get; private set; }

    static MatchDummyData()
    {
      HashSet<string> usedAutonomyNames = new ();  
      MatchData[] firstYear = GetFirstYear();
      MatchData[] secondYear = GetSecondYear(usedAutonomyNames);
      MatchData[] thirdYear = GetThirdYear(usedAutonomyNames);
      MatchData[] forthYear = GetForthYear(usedAutonomyNames);
      DummyData = new MatchData[firstYear.Length + secondYear.Length + thirdYear.Length + forthYear.Length];
      Array.Copy(sourceArray: firstYear,
        sourceIndex: 0,
        destinationArray: DummyData,
        destinationIndex: 0,
        length: firstYear.Length);  
      Array.Copy(sourceArray: secondYear,
        sourceIndex: 0,
        destinationArray: DummyData,
        destinationIndex: firstYear.Length, 
        length: secondYear.Length);
      Array.Copy(sourceArray: thirdYear,
        sourceIndex: 0,
        destinationArray: DummyData, 
        destinationIndex: firstYear.Length + secondYear.Length,
        length: thirdYear.Length);
      Array.Copy(sourceArray: forthYear,
        sourceIndex: 0,
        destinationArray: DummyData,
        destinationIndex: firstYear.Length + secondYear.Length + thirdYear.Length,
        length: forthYear.Length);
    }

    public static MatchData[] GetFirstYear()
    {
      var singleMatches = new (SportType type, int week)[] {
        (SportType.SpeedSkating, 8),
        (SportType.Skeleton, 16),
        (SportType.SkiJumping, 24),
        (SportType.FigureSkating, 31)
      };
      var matches = new MatchData[singleMatches.Length + 1];
      for (int i = 0; i < singleMatches.Length; i++) {
        matches[i] = new MatchData {
          Name = MatchData.GetSportTypeString(singleMatches[i].type),
          MatchType = MatchType.SingleSport,
          SportType = singleMatches[i].type,
          MemberContries = DOMESTIC,
          Rewards = EMPTY_REWARDS,
          DateOfEvent = new GameDate {
            Year = 1, Week = singleMatches[i].week }
        }; 
      }
      matches[matches.Length - 1] = new MatchData {
        Name = DOMESTIC_WINTER_CUP,
        MatchType = MatchType.Domestic,
        MemberContries = DOMESTIC,
        Rewards = EMPTY_REWARDS,
        DateOfEvent = new GameDate{ Year = 1, Week = 39 }
      };
      return (matches);
    }
    
    public static MatchData[] GetSecondYear(HashSet<string> usedAutonomyNames)
    {
      var autonomyWeeks = new int[] {
        5, 11, 25, 33
      };
      var matches = new MatchData[autonomyWeeks.Length + 2];
      FillAutnomyCups(matches, 2, autonomyWeeks, usedAutonomyNames);
      FillMandatoryMatches(matches, 2, WINTER_ASIAN_CUP);
      return (matches);
    }

    public static MatchData[] GetThirdYear(HashSet<string> usedAutonomyNames)
    {
      var autonomyWeeks = new int[] {
        6, 13, 25, 31
      };
      var matches = new MatchData[autonomyWeeks.Length + 2];
      FillAutnomyCups(matches, 3, autonomyWeeks, usedAutonomyNames);
      FillMandatoryMatches(matches, 3, WORLD_CHAMPIONSHIP);
      return (matches);
    }

    public static MatchData[] GetForthYear(HashSet<string> usedAutonomyNames)
    {
      var autonomyWeeks = new int[] {
        6, 12, 26, 32
      };
      var matches = new MatchData[autonomyWeeks.Length + 2];
      FillAutnomyCups(matches, 4, autonomyWeeks, usedAutonomyNames);
      FillMandatoryMatches(matches, 4, INTERNATIONAL_WINTER_SPORT);
      return (matches);
    }

    static void FillMandatoryMatches(MatchData[] matches, int year, string lastMatch)
    {
      matches[matches.Length - 2] = new MatchData {
        Name = PRESIDENT_CUP,
        MemberContries = DOMESTIC,
        MatchType = MatchType.Domestic,
        Rewards = EMPTY_REWARDS,
        DateOfEvent = new GameDate { Year = year, Week = 19 }
      };
      matches[matches.Length - 1] = new MatchData {
        Name = lastMatch,
        MatchType = MatchType.International,
        MemberContries = DEFAULT_COUNTRIES,
        Rewards = EMPTY_REWARDS,
        DateOfEvent = new GameDate { Year = year, Week = 39 }
      };

    }

    static void FillAutnomyCups(MatchData[] matches, int year, int[] autonomyWeeks, HashSet<string> usedNames)
    {
      for (int i = 0; i < autonomyWeeks.Length; i++) {
        string name = GetRandomAutonomyName();
        while (usedNames.Contains(name)) {
          name = GetRandomAutonomyName();
        }
        matches[i] = new MatchData {
          Name = name,
          MatchType = MatchType.Friendly,
          MemberContries = DOMESTIC,
          Rewards = EMPTY_REWARDS,
          DateOfEvent = new GameDate { 
            Year = year, Week =  autonomyWeeks[i] }
        }; 
      }
    }
  }
}
