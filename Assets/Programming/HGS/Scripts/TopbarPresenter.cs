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
    [Inject]
    IResourceController resourceController;

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
      this.SubScribeTimeFlow();
      this.SubscribeResources();
      this.OnDestroyAsObservable().Subscribe(
        _ => this.disposable.Dispose());
    }

    void SetSeasonIcon(ITimeFlowController.Season season)
    {
      this.view.SetImageByRawRole(
        (int)ImageRole.SeasonIcon,
        this.seasonIcons[(int)season]);
    }

    void SubScribeTimeFlow()
    {
      this.timeFlowController.WeekInYear
        .Subscribe(
        week => this.SetLabel())
        .AddTo(this.disposable);
      this.timeFlowController.Year
        .Subscribe(
        year => this.SetLabel())
        .AddTo(this.disposable);
      this.timeFlowController.CurrentSeason
        .Subscribe(
        season => {
          this.SetLabel();
          this.SetSeasonIcon(season);
        })
        .AddTo(this.disposable);
    }

    void SubscribeResources()
    {
      this.resourceController.Fame
        .Subscribe(
          fame => this.view.SetRawTextByRole(
            (int)TextRole.FameLabel,
            fame.ToString()))
        .AddTo(this.disposable);
      this.resourceController.Coin
        .Subscribe(
          coin => this.view.SetRawTextByRole(
            (int)TextRole.CoinLabel,
            coin.ToString()))
        .AddTo(this.disposable);
      this.resourceController.Money
        .Subscribe(
          money => this.view.SetRawTextByRole(
            (int)TextRole.MoneyLabel,
            $"{money} G"))
        .AddTo(this.disposable);
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
