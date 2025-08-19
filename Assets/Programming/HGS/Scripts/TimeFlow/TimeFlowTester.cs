using UnityEngine;
using EditorAttributes;
using Zenject;
using UniRx;

namespace SHG
{
  public class TimeFlowTester : MonoBehaviour
  {
    [Inject]
    ITimeFlowController timeFlowController;

    [SerializeField] [ReadOnly]
    int year;
    [SerializeField] [ReadOnly]
    int week;
    [SerializeField] [ReadOnly]
    ITimeFlowController.Season season;
    [SerializeField] [ReadOnly]
    string text;

    CompositeDisposable disposables;
  

    // Start is called before the first frame update
    void Start()
    {
      this.disposables = new();
      this.timeFlowController.Year
        .Subscribe(
        year => this.year = year)
        .AddTo(this.disposables);
      this.timeFlowController.WeekInYear
        .Subscribe(week => {
          this.week = week;
          })
        .AddTo(this.disposables);
      this.timeFlowController.CurrentSeason
        .Subscribe(season => this.season = season)
        .AddTo(this.disposables);
    }

    void OnDestroy()
    {
      this.disposables.Dispose();
    }

    [Button]
    void ProgressWeek()
    {
      this.timeFlowController.ProgressWeek();
    }

    [Button]
    void ProgressWeeks(int weeks)
    {
      this.timeFlowController.ProgressWeeks(weeks);
    }
  }

}
