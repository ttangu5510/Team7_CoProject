using UniRx;

namespace SHG
{
  public class DummyAthleteController : IAthleteController {
    public ReactiveProperty<int> NumberOfGeneralAthlete { get; private set; }

    public ReactiveProperty<int> NumberOfNationalAthleteCandidate { get; private set; }

    public ReactiveProperty<int> NumberOfNationalAthlete { get; private set; }
    public ReactiveProperty<int> NumberOfCoach { get; private set; }

    public DummyAthleteController()
    {
      this.NumberOfGeneralAthlete = new (5);
      this.NumberOfNationalAthleteCandidate= new (3);
      this.NumberOfNationalAthlete = new (2);
      this.NumberOfCoach = new (1);
    }
  }
}
