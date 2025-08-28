using System;
using StatefulUI.Runtime.Core;
using StatefulUI.Runtime.References;
using UniRx;

namespace SHG
{
  public class MatchViewResultScreen 
  {

    ReactiveProperty<MatchViewPresenter.ViewState> parentState;
    StatefulComponent view;

    public MatchViewResultScreen(
      ReactiveProperty<MatchViewPresenter.ViewState> parentState,
      StatefulComponent view)
    {
      this.parentState = parentState;
      this.view = view;

      this.view.GetItem<ButtonReference>(
        (int)ButtonRole.NextButton).Button
        .OnClickAsObservable()
        .Subscribe(_ => {
          if (this.parentState.Value == 
            MatchViewPresenter.ViewState.Result) {
          this.parentState.Value = MatchViewPresenter.ViewState.None;
          }});
    }
  }

}
