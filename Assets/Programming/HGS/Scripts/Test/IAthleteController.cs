using UniRx;

namespace SHG
{
  public interface IAthleteController 
  {
    public ReactiveProperty<int> NumberOfGeneralAthlete { get; }
    public ReactiveProperty<int> NumberOfNationalAthleteCandidate { get; }
    public ReactiveProperty<int> NumberOfNationalAthlete { get; }
    public ReactiveProperty<int> NumberOfCoach { get; }
  }
}
