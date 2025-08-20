using UnityEngine;
using StatefulUI.Runtime.References;
using StatefulUI.Runtime.Core;
using UniRx;
using Zenject;
using DG.Tweening;

namespace SHG
{
  [RequireComponent(typeof(StatefulComponent))]
  public class FinancialReportPresenter : MonoBehaviour
  {
    [Inject]
    IResourceController resourceController;
    [Inject]
    ITimeFlowController timeFlowController;

    StatefulComponent view;
    Transform container;

    void Awake()
    {
      this.view = this.GetComponent<StatefulComponent>();
      this.container = this.view.GetItem<ObjectReference>(
        (int)ObjectRole.Container).Object.transform;
    }

    void Start()
    {
      this.resourceController.LastSeasonReport
        .Subscribe(report => {
          if (report != null) {
          int yearSpan = this.timeFlowController.Year.Value - this.timeFlowController.Start.year;
          if (yearSpan == 0 &&
            this.timeFlowController.WeekInYear.Value == 1) {
            return;
          }
          this.GetReport(report.Value);
          this.view.SetState((int)StateRole.Shown);
          this.container.DOMoveY(
            endValue: 400f,
            duration:0.5f)
          .SetEase(Ease.InOutSine);
          }});
      this.view.GetItem<ButtonReference>(
        (int)ButtonRole.ConfirmButton).Button
        .OnClickAsObservable()
        .Subscribe(_ => {
          this.container.DOMoveY(
            endValue: -500f,
            duration: 0.5f)
          .SetEase(Ease.InOutSine)
          .OnComplete(() => this.view.SetState((int)StateRole.Hidden));
          });
    }

    void GetReport(SeasonFinanceData report) {
      int year = this.timeFlowController.Year.Value;
      var season = this.timeFlowController.CurrentSeason.Value;
      this.view.SetRawTextByRole(
        (int)TextRole.Title,
        this.GetTitleString(year, this.timeFlowController.CurrentSeason.Value));

      this.view.SetRawTextByRole(
        (int)TextRole.TotalIncome,
        $"{report.Income} G");
      this.view.SetRawTextByRole(
        (int)TextRole.TrainingGrant,
        $"{this.GetIncome(IncomeType.TrainingGrant, report)} G");
      this.view.SetRawTextByRole(
        (int)TextRole.CompetitionGrant,
        $"{this.GetIncome(IncomeType.CompetitionGrant, report)} G");
      this.view.SetRawTextByRole(
        (int)TextRole.PrizeIncome,
        $"{this.GetIncome(IncomeType.QuestPrizes, report)} G");

      this.view.SetRawTextByRole(
        (int)TextRole.TotalExpenses,
        $"{report.Expense} G");
      this.view.SetRawTextByRole(
        (int)TextRole.PersonelCost,
        $"{this.GetExpense(ExpensesType.PersonnelMaintainance, report)} G");  
      this.view.SetRawTextByRole(
        (int)TextRole.ScoutCost,
        $"{this.GetExpense(ExpensesType.Scout, report)} G");
      this.view.SetRawTextByRole(
        (int)TextRole.FacilityCost,
        $"{this.GetExpense(ExpensesType.FacilityMaintainance, report)} G");

      this.view.SetRawTextByRole(
        (int)TextRole.TotalAmount,
        $"{report.Total} G");
    }

    int GetIncome(IncomeType type, SeasonFinanceData report) 
    {
      int income = 0;
      report.Incomes.TryGetValue(type, out income);
      return (income);
    }

    int GetExpense(ExpensesType type, SeasonFinanceData report)
    {
      int expense = 0;
      report.Expenses.TryGetValue(type, out expense);
      return (expense);
    }

    string GetTitleString(int year, ITimeFlowController.Season season)
    {
      switch (season) {
        case ITimeFlowController.Season.Spring:
          return ($"{(year - 1) % 100}년 겨울 정산서");
        case ITimeFlowController.Season.Summer:
          return ($"{year % 100}년 봄 정산서");
        case ITimeFlowController.Season.Fall:
          return ($"{year % 100}년 여름 정산서");
        case ITimeFlowController.Season.Winter:
          return ($"{year % 100}년 가을 정산서"); 
      }
      return (string.Empty);
    }
  }
}
