
namespace SHG
{
  public enum Season
  {
    Spring,
    Summer,
    Fall,
    Winter
  }

  [System.Serializable]
  public struct GameDate
  {
    /// <summary> 게임 시작부터의 연차 (1년차부터 해당) </summary>
    public int Year;
    /// <summary> 해당 연도의 주차 (1주차부터 해당) </summary>
    public int Week;

    public override bool Equals(object obj) {
      return base.Equals(obj);
    }

    public override int GetHashCode() {
      return (this.Year * 100 + this.Week);
    }

    public static int operator- (GameDate dateA, GameDate dateB)
    {
      int a = (dateA.Year * ITimeFlowController.WEEK_FOR_YEAR) + dateA.Week;
      int b = (dateB.Year * ITimeFlowController.WEEK_FOR_YEAR) + dateB.Week;
      return (a - b);
    }

    public override string ToString() {
      return ($"[{nameof(ITimeFlowController)}.{nameof(GameDate)}; {nameof(Year)}:{this.Year}; {nameof(Week)}:{this.Week}]");
    }

    public static bool operator== (GameDate dateA, GameDate dateB)
    {
      return (dateA.Year == dateB.Year && dateA.Week == dateB.Week);
    }

    public static bool operator!= (GameDate dateA, GameDate dateB)
    {
      return (!(dateA == dateB));
    }

    public static bool operator< (GameDate dateA, GameDate dateB)
    {
      if (dateA.Year == dateB.Year) {
        return (dateA.Week < dateB.Week);
      }
      return (dateA.Year < dateB.Year);
    }

    public static bool operator<= (GameDate dateA, GameDate dateB)
    {
      return (dateA < dateB || dateA == dateB);
    }

    public static bool operator>= (GameDate dateA, GameDate dateB)
    {
      return (dateA > dateB || dateA == dateB);
    }

    public static bool operator> (GameDate dateA, GameDate dateB)
    {
      if (dateA.Year == dateB.Year) {
        return (dateA.Week > dateB.Week);
      }
      return (dateA.Year > dateB.Year);
    }

    
    public static string GetKoreanNameOf(Season season) {
      switch (season) {
        case Season.Spring:
          return ("봄");
        case Season.Summer:
          return ("여름");
        case Season.Fall:
          return ("가을");
        case Season.Winter:
          return ("겨울");
      }
      return (string.Empty);
    }
  }

}
