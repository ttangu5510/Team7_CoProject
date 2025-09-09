using System;
using UniRx;

namespace SHG
{
  /// <summary>
  /// 재화의 종류
  /// </summary>
  public enum ResourceType
  {
    Money,
    Fame,
    Coin
  }

  /// <summary>
  /// 사용자의 재화를 관리하는 인터페이스
  /// </summary>
  public interface IResourceController
  {
    /// <summary>
    /// 현재 가진 돈, 가진 돈보다 많은 관리비가 지출될 때는 음수 값이 될 수 있음
    /// </summary>
    public ReactiveProperty<int> Money { get; }
    /// <summary>
    /// 현재 명성, 명성은 소모 되지 않음
    /// </summary>
    public ReactiveProperty<int> Fame { get; }
    /// <summary>
    /// 현재 가진 특훈 코인
    /// </summary>
    public ReactiveProperty<int> Coin { get; }
    /// <summary>
    /// 새로운 계절이 시작될 때 지난 계절의 손익계산 정보가 생성됨
    /// </summary>
    public ReactiveProperty<Nullable<SeasonFinanceData>> LastSeasonReport{ get; }

    /// <summary> 사용자의 명성을 증가시킴  </summary>
    public void AddFame(int amount);
    /// <summary> 사용자의 특훈 코인을 증가시킴 </summary>
    public void AddCoin(int amount);
    /// <summary> 사용자의 수익 방식에 따라 돈을 증가시킴 </summary>
    public void AddMoney(int amount, IncomeType type);
    /// <summary> 특훈 코인을 소비함 </summary>
    public void SpendCoin(int amount);
    /// <summary> 사용자의 지출 방식에 따라 비용을 지출함 </summary>
    public void SpendMoney(int amount, ExpensesType type);
  }
}
