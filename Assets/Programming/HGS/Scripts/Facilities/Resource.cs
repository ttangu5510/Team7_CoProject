using System;
using UniRx;

namespace SHG
{
  public enum ResourceType
  {
    Money,
    Fame
  }

  public interface IResourceController
  {
    public ReactiveProperty<int> Money { get; }
    public ReactiveProperty<int> Fame { get; }

    public void Add(ResourceType type, int amount);
    public void SpendMoney(int amount);
  }

  public class ResourceController: IResourceController
  {
    
    public ReactiveProperty<int> Money { get; }
    public ReactiveProperty<int> Fame { get; }


    public void Add(ResourceType type, int amount)
    {
      if (amount <= 0) {
        throw (new ArgumentException($"{nameof(Add)}: {nameof(amount)} <= 0"));
      }
    }

    public void SpendMoney(int amount)
    {
      if (amount <= 0) {
        throw (new ArgumentException($"{nameof(SpendMoney)}: {nameof(amount)} <= 0"));
      }

    }
  }
}
