using System;
using UnityEngine;
using UnityEngine.UI;
using StatefulUI.Runtime.Core;
using StatefulUI.Runtime.References;
using UniRx;
using UniRx.Triggers;
using DG.Tweening;
using Zenject;

namespace SHG
{
  [RequireComponent(typeof(StatefulComponent))]
  public class UpgradePopupPresenter : MonoBehaviour
  {
    [Inject]
    IFacilitiesController facilitiesController;
    [Inject]
    IResourceController resourceController;
    [Inject]
    ITimeFlowController timeFlowController;

    StatefulComponent view;
    Transform container;
    Button confirmButton;

    void Awake()
    {
      this.view = this.GetComponent<StatefulComponent>();
      this.container = this.view.GetItem<ObjectReference>(
        (int)ObjectRole.Container).Object.transform;
      this.confirmButton = 
      this.view.GetItem<ButtonReference>(
        (int)ButtonRole.ConfirmButton).Button;
    }

    void Start()
    {
      this.container.OnEnableAsObservable()
        .Subscribe(_ => {
          this.FillContents();
          this.container.DOLocalMoveY(
            endValue: -100f,
            duration: 0.5f)
          .SetEase(Ease.InOutSine);
          });
      this.view.GetItem<ButtonReference>(
        (int)ButtonRole.CancelButton).Button
        .OnClickAsObservable()
        .Subscribe(_ => this.Hide());
      this.confirmButton
        .OnClickAsObservable()
        .Subscribe(_ => {
          this.confirmButton.interactable = false;
          if (this.facilitiesController.Selected.Value != null) {
            this.UpgradeFacility(this.facilitiesController.Selected.Value.Value.facility);
          }
          this.Hide();
          });
    }

    void UpgradeFacility(IFacility facility)
    {
      int cost = this.GetRequiredCost(facility);
      this.resourceController.SpendMoney(
        cost, ExpensesType.FacilityUpgrade);
      facility.Upgrade();
      this.timeFlowController.ProgressWeeks(
        facility.WeeksForUpgrade);
    }

    void Hide()
    {
      this.container.DOLocalMoveY(
        endValue: -300f,
        duration: 0.2f)
        .SetEase(Ease.OutSine)
        .OnComplete(() => this.view.SetState((int)StateRole.Hidden));
    }

    void FillContents()
    {
      var selected = this.facilitiesController.Selected.Value;
      if (selected == null) {
        #if UNITY_EDITOR 
        throw (new ApplicationException($"{nameof(UpgradePopupPresenter)}: {nameof(FillContents)}"));
        #else
        this.view.SetState((int)StateRole.Hidden);
        return; 
        #endif
      }
      IFacility facility = selected.Value.facility;
      int currentStage = facility.CurrentStage.Value;
      this.view.SetRawTextByRole(
        (int)TextRole.BeforeStageLabel,
        $"{currentStage}단계");
      this.view.SetRawTextByRole(
        (int)TextRole.AfterStageLabel,
        $"{currentStage + 1}단계");
      this.view.SetRawTextByRole(
        (int)TextRole.Description,
        this.GetDescriptionText(this.GetRequiredCost(facility), facility.WeeksForUpgrade));
    }

    int GetRequiredCost(IFacility facility)
    {
      var index = Array.FindIndex(
        facility.ResourcesNeeded.Value,
        resource => resource.type == ResourceType.Money);
      if (index != -1) {
        return (facility.ResourcesNeeded.Value[index].amount);
      }
      return (0);
    }

    string GetDescriptionText(int money, int week)
    {
      return (
        $"<color=#F86B6B>{money:n0}</color>G 소모하여 시설을 업그레이드하시겠습니까?\n\n 업그레이드 진행 시 <color=#F86B6B>{week}주가 소모</color>됩니다.");
    }
  }
}
