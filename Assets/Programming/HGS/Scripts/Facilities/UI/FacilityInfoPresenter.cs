using System;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using StatefulUI.Runtime.Core;
using StatefulUI.Runtime.References;
using Zenject;
using System.Text;

namespace SHG
{
  using FacilityType = IFacility.FacilityType;
  [RequireComponent(typeof(StatefulComponent))]
  public class FacilityInfoPresenter : MonoBehaviour
  {
    [Inject]
    IFacilitiesController facilitiesController;
    [Inject]
    IResourceController resourceController;
    public StatefulComponent View;
    public ContainerView GradeSectionContainer;
    CompositeDisposable disposable;

    StatefulComponent popup;

    void Awake()
    {
      this.View = this.GetComponent<StatefulComponent>();
      this.GradeSectionContainer = this.View.GetItem<ContainerReference>(
        (int)ContainerRole.GradeSectionContainer).Container;
      this.popup = 
      this.View.GetItem<ObjectReference>(
        (int)ObjectRole.Popup).Object.GetComponent<StatefulComponent>();
      this.disposable = new ();
    }

    void Start()
    {
      this.facilitiesController.Selected
        .Subscribe(selected => {
          if (selected != null) {
          this.ShowInfo(selected.Value.facility);
          }})
        .AddTo(this.disposable);
      this.facilitiesController.SelectedFacilityStream
        .Subscribe(this.ShowInfo)
        .AddTo(this.disposable);
      this.OnDestroyAsObservable()
        .Subscribe(_ => this.disposable.Dispose());
      this.resourceController.Money
        .Merge(this.resourceController.Fame)
        .Where(_ => this.facilitiesController.Selected.Value != null)
        .Subscribe(_ => 
          this.FillGradeSectionContainer(
            this.facilitiesController.Selected.Value.Value.facility))
        .AddTo(this.disposable);
    }

    void ShowInfo(IFacility facility)
    {
      string gradeString = $"시설: {facility.CurrentStage.Value}단계";
      this.View.SetRawTextByRole(
        (int)TextRole.GradeLabel, gradeString);
      string[] infoTexts = this.GetFacilityInfoTexts(facility);
      string infoString1 = infoTexts.Length > 0 ?
        infoTexts[0]: string.Empty;
        this.View.SetRawTextByRole(
          (int)TextRole.GradeSectionLabel1,
          infoString1);
      string infoString2 = infoTexts.Length > 1 ?
        infoTexts[1]: string.Empty;
      this.View.SetRawTextByRole(
        (int)TextRole.GradeSectionLabel2,
        infoString2);
      this.FillGradeSectionContainer(facility);
    }

    string[] GetFacilityInfoTexts(IFacility facility) {
      switch (facility.Type) {
        case FacilityType.Accomodation:
          var accomodation = facility as Accomodation;
          return (new string[] {
            $"현재 수용 인원: {accomodation.NumberOfAthletes.Value}명",
            $"수용 가능 인원: {accomodation.MaxNumberOfAthletes.Value}명",
            });
        case FacilityType.Lounge:
          var lounge = facility as Lounge;
          return (new string[] {
            $"휴식 배치 가능한 인원:  {lounge.NumberOfAthletes.Value}명",
            $"휴식 진행시 피로도 {lounge.RecoveryAmount.Value}회복"
          });
        case FacilityType.TrainingCenter:
          var trainingCenter = facility as TrainingCenter;
          return (new string[] {
            $"훈련 스탯 추가 수치: +{trainingCenter.BonusStat.Value}",
            $"훈련 진행 시 피로도: {0} ~ {0} 증가"
          });
        case FacilityType.MedicalCenter:
          var medicalCenter = facility as MedicalCenter;
          return (new string[] {
            $"회복량: {medicalCenter.RecoveryAmount.Value}",
            $"수용 인원: {medicalCenter.NumberOfAthletes.Value}명"
          });
        case FacilityType.ScoutCenter:
          var scoutCenter = facility as ScoutCenter;
          return (new string[] {
            $"국가대표 등급 선수 등장 확률: {this.RoundNumber(scoutCenter.ChanceForNationalGradeAthlete.Value) }%",
            $"코치 영입 성공 확률: +{this.RoundNumber(scoutCenter.BonusChanceForRecruitCoach.Value)}%"
          });
          default: return new string[0];
      }
    }

    void FillGradeSectionContainer(IFacility facility)
    {
      this.GradeSectionContainer.Clear();
      for (int i = 0; i <= facility.MaxUpgradeStage; i++) {
        var instance = this.GradeSectionContainer.AddInstance(); 
        this.FillGradeSection(
          view: instance.GetComponent<StatefulComponent>(),
          grade: i,
          facility: facility);
      }
    }

    void FillGradeSection(StatefulComponent view, int grade, IFacility facility)
    {
      view.SetRawTextByRole(
        (int)TextRole.GradeLabel,
        $"{grade}단계");

      view.SetRawTextByRole(
        (int)TextRole.GradeSectionLabel1,
        this.GetEffectString(facility, grade));

      string resourceString = this.GetResourceText(facility, grade);
      if (!string.IsNullOrEmpty(resourceString)) {
        view.SetState((int)StateRole.ResourceShown);
        view.SetRawTextByRole(
          (int)TextRole.GradeSectionLabel2,
          resourceString);
      } else {
        view.SetState((int)StateRole.ResourceHidden);
      }
      if (facility.CurrentStage.Value >= grade) {
        view.SetState((int)StateRole.UpgradeCompleted); 
        view.SetRawTextByRole(
          (int)TextRole.UpgradeButtonLabel,
          "완료");
      }
      else if (facility.CurrentStage.Value + 1 == grade &&
        this.IsUpgradable(facility)) {
        view.SetState((int)StateRole.Upgradable);
        view.SetRawTextByRole(
          (int)TextRole.UpgradeButtonLabel,
          "업그레이드");
      }
      else {
        view.SetState((int)StateRole.UpgradeLocked);
        view.SetRawTextByRole(
          (int)TextRole.UpgradeButtonLabel, "");
      }
      view.GetItem<ButtonReference>(
        (int)ButtonRole.UpgradeButton).Button
        .OnClickAsObservable()
        .Subscribe(_ => {
          if (facility.CurrentStage.Value + 1 == grade &&
            facility.IsUpgradable) {
            this.popup.SetState((int)StateRole.Shown);
          }});
    }

    bool IsUpgradable(IFacility facility)
    {
      if (!facility.IsUpgradable) {
        return (false);
      }
      foreach (var (resourceType, amount) in facility.ResourcesNeeded.Value) {
        switch (resourceType) {
          case ResourceType.Money:
            if (this.resourceController.Money.Value < amount) {
              return (false);
            }
            break;
          case ResourceType.Fame:
            if (this.resourceController.Fame.Value < amount) {
              return (false);
            }
            break;
          default: 
            throw (new ApplicationException($"{nameof(IsUpgradable)}: {resourceType}"));
        }
      }
      return (true);
    }

    string GetEffectString(IFacility facility, int grade)
    {
      switch (facility.Type) {
        case FacilityType.Accomodation:
          var accomodation = facility as Accomodation;
          return ($"선수단 수용 가능 인원 {accomodation.Data.NumberOfAthletes[grade]}명");
        case FacilityType.Lounge:
          var lounge = facility as Lounge;
          return ($"휴식 배치 가능 인원 {lounge.Data.NumberOfAthletes[grade]}명"); 
        case FacilityType.TrainingCenter:
          var trainingCenter = facility as TrainingCenter;
          return ($"훈련 진행시 추가 스탯 +{trainingCenter.Data.BonusStats[grade]}");
        case FacilityType.MedicalCenter:
          var medicalCenter = facility as MedicalCenter;
          return ($"{medicalCenter.RecoveryAmount.Value} 회복");
        case FacilityType.ScoutCenter:
          var scoutCenter = facility as ScoutCenter;
          return ($"국가 대표 등급 선수 등장 확률 {this.RoundNumber(scoutCenter.Data.ChancesForNationalGradeAthlete[grade])}%\n코치 영입 성공 확률 +{this.RoundNumber(scoutCenter.Data.BonusChancesForRecruitCoach[grade])}%");
      }
      return (null);
    }

    string GetResourceText(IFacility facility, int grade) {
      if (facility.CurrentStage.Value >= grade) {
        return (null);
      }
      IFacilityData data = (facility.Type) switch {
        FacilityType.Accomodation => (facility as Accomodation).Data,
        FacilityType.Lounge => (facility as Lounge).Data,
        FacilityType.TrainingCenter => (facility as TrainingCenter).Data,
        FacilityType.MedicalCenter => (facility as MedicalCenter).Data,
        FacilityType.ScoutCenter => (facility as ScoutCenter).Data,
        _ => throw (new ApplicationException())
      };
      int fame = data.GetRequiredFameForUpgradeFrom(grade - 1);
      int money = data.GetUpgradeCostFrom(grade - 1);
      var builder = new StringBuilder();
      builder.Append("조건: ");
      if (fame > 0) {
        builder.Append($"<sprite=1> {fame}  ");
      }
      builder.Append($"<sprite=0> {money:n0}G");
      return (builder.ToString());
    }

    float RoundNumber(float number)
    {
      return (MathF.Round(number, 3, MidpointRounding.AwayFromZero));
    }
  }
}
