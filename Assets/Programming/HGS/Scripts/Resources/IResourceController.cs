using System;
using UniRx;

namespace SHG
{
  public enum ResourceType
  {
    Money,
    Fame,
    Coin
  }

  public interface IResourceController
  {
    public ReactiveProperty<int> Money { get; }
    public ReactiveProperty<int> Fame { get; }
    public ReactiveProperty<int> Coin { get; }
    public ReactiveProperty<Nullable<SeasonFinanceData>> LastSeasonReport{ get; }

    public void AddFame(int amount);
    public void AddCoin(int amount);
    public void AddMoney(int amount, IncomeType type);
    public void SpendCoin(int amount);
    public void SpendMoney(int amount, ExpensesType type);
  }
}
