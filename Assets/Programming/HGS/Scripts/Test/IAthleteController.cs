using UniRx;
using JYL;

namespace SHG
{
  public interface IAthleteController 
  {
    public enum StatType 
    {
      Stamina,   //체력
      Quickness, // 순발력
      Flexibility,// 유연성
      Technic,    //기술
      Speed,      //속도
      Balance,    //균형감각
      Fatigue     //피로도
    }

    public static string GetNameof(StatType stat)
    {
      switch (stat) {
        case StatType.Stamina:
          return ("체력");
        case StatType.Quickness:
          return ("순발력");
        case StatType.Flexibility:
          return (" 유연성");
        case StatType.Technic:
          return ("기술");
        case StatType.Speed:
          return ("속도");
        case StatType.Balance:
          return ("균형감각");
        case StatType.Fatigue:
          return ("피로도");
        default: return ("");
      }
    }

    public static int GetValueOf(StatType statType, AthleteStats stats)
    {
      switch (statType) {
        case StatType.Stamina:
          return (stats.health);
        case StatType.Quickness:
          return (stats.quickness);
        case StatType.Flexibility:
          return (stats.flexibility);
        case StatType.Technic:
          return (stats.technic);
        case StatType.Speed:
          return (stats.speed);
        case StatType.Balance:
          return (stats.balance);
        case StatType.Fatigue:
          return (stats.fatigue);
        default: return (0);
      }
    }

    //TODO: Calc stats 
    public static string GetAverageStatTextOf(AthleteStats stat) {
      return ("B");
    }

    public static string GetAverageStatTextOf(StatType statType, AthleteStats stat) {
      return ("B");
    }

    public ReactiveProperty<int> NumberOfGeneralAthlete { get; }
    public ReactiveProperty<int> NumberOfNationalAthleteCandidate { get; }
    public ReactiveProperty<int> NumberOfNationalAthlete { get; }
    public ReactiveProperty<int> NumberOfCoach { get; }
    public ReactiveCollection<DomAthEntity> Athletes { get; }
    public bool TryGetAthleteBy(int id, out DomAthEntity athlete);
  }
}
