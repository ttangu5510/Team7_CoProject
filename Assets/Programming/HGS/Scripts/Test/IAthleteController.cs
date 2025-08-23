using UniRx;
using JYL;

namespace SHG
{
  public interface IAthleteController 
  {
    public ReactiveProperty<int> NumberOfGeneralAthlete { get; }
    public ReactiveProperty<int> NumberOfNationalAthleteCandidate { get; }
    public ReactiveProperty<int> NumberOfNationalAthlete { get; }
    public ReactiveProperty<int> NumberOfCoach { get; }
    public ReactiveCollection<DomAthEntity> Athletes { get; }
    public bool TryGetAthleteBy(int id, out DomAthEntity athlete);
  }
}
