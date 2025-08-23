using UniRx;

namespace SHG
{
  /// <summary>
  /// 시간에 흐름을 관리하는 인터페이스
  /// </summary>
  public interface ITimeFlowController 
  {

    /// <summary>  계절이 변경될 때 이벤트를 발생 (새로운 해는 겨울이 아닌 봄부터 시작한다)</summary>
    public ReactiveProperty<Season> CurrentSeason { get; }

    /// <summary> 연도가 변경될 때 이벤트를 발생 </summary>
    public ReactiveProperty<int> Year { get; }

    /// <summary> 주차가 변경될 때 이벤트를 발생 (새로운 해는 1주부터 시작한다) </summary>
    public ReactiveProperty<int> WeekInYear { get; }

    /// <summary> 현재 주를 포함해서 기본 엔딩까지 남은 시간을 알려주는 기능 </summary>
    public ReactiveCollection<GameDate> DateToEnd { get; }

    /// <summary> 1주의 시간을 흐르게 하는 역할 </summary>
    public void ProgressWeek();

    /// <summary> 원하는 만큼의 주의 시간을 흐르게 하는 역할 </summary>
    public void ProgressWeeks(int weeks);

    /// <summary> 게임을 시작한 시점 </summary>
    public (int year, int week) Start { get; }

    /// <summary> 게임을 시작한 이후로의 연차 (처음 시작시 1년차) </summary>
    public int YearPassedAfterStart { get; }

  }
}
