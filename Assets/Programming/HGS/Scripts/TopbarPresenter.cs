using UnityEngine;
using StatefulUI.Runtime.Core;
using Zenject;
using UniRx;
using UniRx.Triggers;

namespace SHG
{
  public class TopbarPresenter : MonoBehaviour
  {
    [Inject]
    ITimeFlowController timeFlowController;
    StatefulComponent view;
    CompositeDisposable disposable; 
    [SerializeField]
    Sprite[] seasonIcons;

    void Awake()
    {
      this.disposable = new ();
      this.view = this.GetComponent<StatefulComponent>();
    }

    // Start is called before the first frame update
    void Start()
    {
      this.timeFlowController.WeekInYear.Subscribe(
        week => this.SetLabel())
        .AddTo(this.disposable);
      this.timeFlowController.Year.Subscribe(
        year => this.SetLabel())
        .AddTo(this.disposable);
      this.timeFlowController.CurrentSeason.Subscribe(
        season => {
          this.SetLabel();
          this.SetSeasonIcon(season);
        })
        .AddTo(this.disposable);
      this.OnDestroyAsObservable().Subscribe(
        _ => this.disposable.Dispose());
    }

    void SetSeasonIcon(ITimeFlowController.Season season)
    {
      this.view.SetImageByRawRole(
        (int)ImageRole.SeasonIcon,
        this.seasonIcons[(int)season]);
    }

    void SetLabel()
    {
      int year = this.timeFlowController.Year.Value;
      ITimeFlowController.Season season = this.timeFlowController.CurrentSeason.Value;
      int week = this.timeFlowController.WeekInYear.Value;
      string seasonString = season switch {
        ITimeFlowController.Season.Spring => "봄",
        ITimeFlowController.Season.Summer => "여름",
        ITimeFlowController.Season.Fall => "가을",
        ITimeFlowController.Season.Winter => "겨울",
        _ => ""
      };
      this.view.SetRawTextByRole(
        role: (int)TextRole.DateLabel,
        text: $"{year}년 {seasonString} {week}주차");
    }
  }
}
