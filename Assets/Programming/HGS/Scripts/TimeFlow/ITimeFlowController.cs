using UniRx;

namespace SHG
{
  /// <summary>
  /// 시간에 흐름을 관리하는 인터페이스
  /// </summary>
  public interface ITimeFlowController 
  {
    public enum Season
    {
      Spring,
      Summer,
      Fall,
      Winter
    }

    /// <summary>  계절이 변경될 때 이벤트를 발생 </summary>
    public ReactiveProperty<Season> CurrentSeason { get; }
    /// <summary> 연도가 변경될 때 이벤트를 발생 </summary>
    public ReactiveProperty<int> Year { get; }
    /// <summary> 주차가 변경될 때 이벤트를 발생 </summary>
    public ReactiveProperty<int> WeekInYear { get; }
    /// <summary> 1주의 시간을 흐르게 하는 역할 </summary>
    public void ProgressWeek();
    /// <summary> 원하는 만큼의 주의 시간을 흐르게 하는 역할 </summary>
    public void ProgressWeeks(int weeks);
  }
}
