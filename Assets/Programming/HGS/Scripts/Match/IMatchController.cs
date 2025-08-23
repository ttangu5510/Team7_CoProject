using System;
using UniRx;

namespace SHG
{
  /// <summary>
  /// 경기를 관리하는 인터페이스
  /// </summary>
  public interface IMatchController
  {
    /// <summary>
    /// 현재 진행중이 경기
    /// </summary>
    public ReactiveProperty<Match> CurrentMatch { get; }
    /// <summary>
    /// 현재 진행중인 경기 또는 다음에 예정된 경기에 대한 정보
    /// </summary>
    public ReactiveProperty<Nullable<MatchData>> NextMatch { get; }
    /// <summary>
    /// 일자에 따라 예정되어 있는 경기 목록
    /// </summary>
    public ReactiveCollection<MatchData> ScheduledMatches { get; }

    /// <summary>
    /// 예정된 경기를 실행하는 기능 (현재 해당 경기를 실행할 수 있는지는 확인하지 않음)
    /// </summary>
    public void StartNextMatch();
  }
}
