using System;
using UnityEngine;
using UnityEngine.UI;
using StatefulUI.Runtime.Core;
using StatefulUI.Runtime.References;
using UniRx;
using UniRx.Triggers;
using Zenject;
using DG.Tweening;

namespace SHG
{
  using FacilityType = IFacility.FacilityType;

  [RequireComponent(typeof(StatefulComponent))]
  public class FacilityPresenter : MonoBehaviour
  {
    [Inject]
    IFacilitiesController facilitiesController;
    StatefulComponent view;
    FacilityInfoPresenter infoPresenter;
    IDisposable subscribeFacility;

    void Awake()
    {
      this.view = this.GetComponent<StatefulComponent>();
      this.infoPresenter = this.GetComponentInChildren<FacilityInfoPresenter>();
      var closeButton = this.view.GetItem<ButtonReference>(
        (int)ButtonRole.CloseButton).Button;
      closeButton.OnClickAsObservable()
        .Subscribe(_ => this.facilitiesController.UnSelectFacility());
      this.view.SetRawTextByRole((int)TextRole.CloseButtonLabel, "닫기");
    }

    void SubscribeFacility()
    {
      this.subscribeFacility = this.facilitiesController.Selected
        .Skip(1)
        .Subscribe(selected => {
          if (selected != null) {
            this.view.SetRawTextByRole(
              (int)TextRole.FacilityNameLabel, selected.Value.facility.Name);
            this.view.SetState((int)StateRole.Shown);
            this.transform.DOLocalMoveY(
              endValue: -300f,
              duration: 0.5f)
            .SetEase(Ease.InOutSine);
            StateRole state = selected.Value.facility.Type switch {
              FacilityType.Accomodation => StateRole.Accomodation,
              FacilityType.Lounge => StateRole.Lounge,
              FacilityType.TrainingCenter => StateRole.TrainingCenter,
              FacilityType.MedicalCenter => StateRole.MedicalCenter,
              FacilityType.ScoutCenter => StateRole.ScoutCenter,

            };
            this.view.SetState((int)state);
          }
          else { 
            this.transform.DOMoveY(
              endValue: -500f,
              duration: 0.5f)
            .SetEase(Ease.InOutSine)
            .OnComplete(() => this.view.SetState((int)StateRole.Hidden));
          }
          });
    }

    // Start is called before the first frame update
    void Start()
    {
      this.SubscribeFacility();
      this.OnDestroyAsObservable()
        .Subscribe(_ => this.subscribeFacility.Dispose());
    }
  }
}
