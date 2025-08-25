using JYL;

namespace SHG
{
  /// <summary>
  /// 경기에서 사용자의 선수와 대결하는 상대 선수
  /// </summary>
  public interface IContenderAthlete
  {
    /// <summary> 선수 능력치 </summary>
    public AthleteStats Stats { get; }
    /// <summary> 선수 등급 </summary>
    public AthleteAffiliation Level { get; }
    /// <summary> 선수 국적 </summary>
    public Country Country { get; }
    /// <summary> 선수 이름</summary>
    public string Name { get; }
  }

  /// <summary>
  /// 국내 선수가 상대 선수로 등장할 때의 타입으로 변경하는 역할
  /// </summary>
  public class ConvertedDomesticAthlete : IContenderAthlete {

    /// <summary> 선수 정보 </summary>
    DomAthEntity athlete;

    public ConvertedDomesticAthlete(DomAthEntity athlete)
    {
      this.athlete = athlete;
    }

    static Country KOREA = new Country { Name = "korea" };
    public AthleteStats Stats => (this.athlete.stats);
    public AthleteAffiliation Level => (this.athlete.affiliation);
    public Country Country => (KOREA);
    public string Name => (this.athlete.entityName);
  }
}
