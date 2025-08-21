
namespace SHG
{
  public struct MatchData 
  {
    public MatchType MatchType;
    public SportType SportType; 
    public Country[] MemberContries;
    public (ResourceType type, int amount) Rewards;
    public (int year, int week) DateOfEvent;
  }

  public enum MatchType
  {
    SingleSport,
    Friendly,
    Domestic,
    International
  }

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

  public struct Country
  {
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
