using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using StatefulUI.Runtime.Core;
using StatefulUI.Runtime.References;
using UniRx;
using UniRx.Triggers;
using Zenject;
using DG.Tweening;
using JYL;

namespace SHG
{
  using FacilityType = IFacility.FacilityType;

  [RequireComponent(typeof(StatefulComponent))]
  public class FacilityPresenter : MonoBehaviour
  {
    const float SHOW_Y_OFFSET = 000f;
    const float HIDE_Y_OFFSET = 800f;

    [Inject]
    IFacilitiesController facilitiesController;
    StatefulComponent view;
    FacilityInfoPresenter infoPresenter;
    IDisposable subscribeFacility;
    ContainerView tabButtonContainer; 
    List<(Button button, StatefulComponent view)> tabButtons;
    StatefulComponent[] tabs;
    HashSet<Button> regiesteredButtons;
    ScrollRect scrollView;

    [Inject]
     IUiManager uIManager;

    void Awake()
    {
      this.view = this.GetComponent<StatefulComponent>();
      this.regiesteredButtons = new ();
      this.infoPresenter = this.GetComponentInChildren<FacilityInfoPresenter>();
      this.tabButtons = new ();
      var tabRoles = new InnerComponentRole[] {
        InnerComponentRole.FirstTab,
        InnerComponentRole.SecondTab,
        InnerComponentRole.ThirdTab
      };
      this.tabs = Array.ConvertAll(
        tabRoles,
        role => this.view.GetItem<InnerComponentReference>((int)role).InnerComponent);
      this.scrollView = this.view.GetItem<ObjectReference>(
        (int)ObjectRole.ScrollView).Object.GetComponent<ScrollRect>();
      var closeButton = this.view.GetItem<ButtonReference>(
        (int)ButtonRole.CloseButton).Button;
      this.tabButtonContainer = this.view.GetItem<ContainerReference>(
        (int)ContainerRole.TabButtonContainer).Container;
      closeButton.OnClickAsObservable()
        .Subscribe(_ => this.Hide());
    }

    void SubscribeFacility()
    {
      this.subscribeFacility = this.facilitiesController.Selected
        .Skip(1)
        .Subscribe(selected => {
          if (selected != null) {
            this.view.SetState((int)StateRole.FirstTab);
            this.view.SetRawTextByRole(
              (int)TextRole.FacilityNameLabel,
              selected.Value.facility.Name);
            this.Show();
            this.UpdateTabBar(selected.Value.type);
            this.scrollView.normalizedPosition = new (
              this.scrollView.normalizedPosition.x,
              1f);
            StateRole state = selected.Value.facility.Type switch {
              FacilityType.Accomodation => StateRole.Accomodation,
              FacilityType.Lounge => StateRole.Lounge,
              FacilityType.TrainingCenter => StateRole.TrainingCenter,
              FacilityType.MedicalCenter => StateRole.MedicalCenter,
              FacilityType.ScoutCenter => StateRole.ScoutCenter,
              };
            this.view.SetState((int)state);
          }});
    }

    // Start is called before the first frame update
    void Start()
    {
      this.SubscribeFacility();
      this.OnDestroyAsObservable()
        .Subscribe(_ => this.subscribeFacility.Dispose());
      var startPos = this.transform.localPosition;
      startPos.y = -HIDE_Y_OFFSET;
      this.transform.localPosition = startPos;
      this.view.SetState((int)StateRole.Hidden);
    }

    void Show()
    {
      this.view.SetState((int)StateRole.Shown);
      this.transform.DOLocalMoveY(
        endValue: -SHOW_Y_OFFSET,
        duration: 0.5f)
        .SetEase(Ease.InOutSine);
            uIManager.AddHashSet(this);
    }

    void Hide()
    {
      this.transform.DOLocalMoveY(
        endValue: -HIDE_Y_OFFSET,
        duration: 0.5f)
        .SetEase(Ease.InOutSine)
        .OnComplete(() => {
          this.view.SetState((int)StateRole.Hidden);
          this.facilitiesController.UnSelectFacility();
        });
            uIManager.RemoveHashSet(this);
    }

    void UpdateTabBar(FacilityType facility)
    {
      this.tabButtonContainer.Clear();
      this.tabButtons.Clear();
      if (FacilityUiConstants.TAB_BUTTON_TEXTS.TryGetValue(facility,
          out string[] texts)) {
        this.tabButtonContainer.FillWithItems(
          texts,
          (view, buttonText) => {
            var button = view.GetComponent<Button>();
            this.tabButtons.Add((button, view));
            view.SetRawTextByRole(
              (int)TextRole.ButtonLabel, buttonText);
            if (!this.regiesteredButtons.Contains(button)) {
              button.OnClickAsObservable()
              .Subscribe(_ => this.OnClickTabButton(button));
              this.regiesteredButtons.Add(button);
            }
          });
        this.OnClickTabButton(this.tabButtons[0].button);
      }
    }

    void OnClickTabButton(Button button)
    {
      var index = this.tabButtons.FindIndex(tab => tab.button == button);
      if (index == -1) {
        #if UNITY_EDITOR
        throw (new ApplicationException($"{nameof(OnClickTabButton)}: Fail to find {button} in {nameof(this.tabButtons)}"));
        #else
        return ;
        #endif
      }
      if (this.facilitiesController.Selected.Value == null) {
        #if UNITY_EDITOR
        throw (new ApplicationException($"{nameof(OnClickTabButton)}: {nameof(this.facilitiesController.Selected)} is null"));
        #else
        return ;
        #endif
      }
      foreach (var (tabButton, view) in this.tabButtons) {
        if (tabButton == button) {
          view.SetState((int)StateRole.Active);
        } 
        else {
          view.SetState((int)StateRole.InActive);
        }
      }
      if (index > 0) {
        StateRole tabState = this.facilitiesController.Selected.Value.Value.type
          switch  {
            FacilityType.Accomodation => StateRole.Accomodation,
              FacilityType.Lounge => StateRole.Lounge,
              FacilityType.TrainingCenter => StateRole.TrainingCenter,
              FacilityType.MedicalCenter => StateRole.MedicalCenter,
              FacilityType.ScoutCenter => StateRole.ScoutCenter
          };
        this.tabs[index].SetState((int)tabState);
      }
      switch (index) {
        case 0:
          this.view.SetState((int)StateRole.FirstTab);
          break;
        case 1:
          this.view.SetState((int)StateRole.SecondTab);
          break;
        case 2:
          this.view.SetState((int)StateRole.ThirdTab);
          break;
        default: 
          throw (new ApplicationException($"{nameof(OnClickTabButton)}: index is out of range {index}"));
      }
      this.scrollView.normalizedPosition = new (
        this.scrollView.normalizedPosition.x,
        1f);
      if (index == 0) {
        this.transform.DOLocalMoveY(
          endValue: -SHOW_Y_OFFSET,
          duration: 0.5f)
          .SetEase(Ease.InOutSine);
      }
      else {
        this.transform.DOLocalMoveY(
          endValue: 0,
          duration: 0.5f)
          .SetEase(Ease.InOutSine);
      } 
    }
  }
}
