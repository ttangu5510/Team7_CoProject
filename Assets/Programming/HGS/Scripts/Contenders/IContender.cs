using JYL;

namespace SHG
{
  /// <summary>
  /// 경기에서 사용자의 선수와 대결하는 상대 선수
  /// </summary>
  public interface IContender
  {
    /// <summary> 선수 능력치 </summary>
    public AthleteStats Stats { get; }
    /// <summary> 선수 등급 </summary>
    public AthleteAffiliation Grade { get; }
    /// <summary> 선수 소속 </summary>
    public IGroup Group { get; }
    /// <summary> 선수 이름 </summary>
    public string Name { get; }

    /// <summary> 선수 ID </summary>
    public int Id { get; }
  }
}
