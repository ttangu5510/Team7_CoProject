using System;
using System.Collections.Generic;
using UniRx;
using Zenject;
using JYL;

namespace SHG
{

  /// <summary>
  /// 사용자의 재화를 관리하는 역할, IResourceController를 참고
  /// </summary>
  public class ResourceController: IResourceController
  {
    ITimeFlowController timeFlowController;
    IFacilitiesController facilitiesController;
    //IAthleteController athleteController;
    DomAthService domAthService;
    CoachService coachService;
    
    public ReactiveProperty<int> Money { get; private set; }
    public ReactiveProperty<int> Fame { get; private set; }
    public ReactiveProperty<int> Coin { get; private set;  }
    public ResourceData Data { get; private set; }
    public ReactiveProperty<SeasonFinanceData?> LastSeasonReport { get; private set; }
    Dictionary<IncomeType, int> incomes;
    Dictionary<ExpensesType, int> expenses;

    public ResourceController(
      ResourceData data, 
      int money = 0,
      int fame = 0, 
      int coin = 0)
    {
      this.LastSeasonReport = new (null);
      this.incomes = new ();
      this.expenses = new ();
      this.Data = data;
      this.Money = new (money);
      this.Fame = new (fame);
      this.Coin = new (coin);
    }

    public void SetData(
      ResourceData data, 
      int money = 0,
      int fame = 0, 
      int coin = 0)
    {
      this.Data = data;
      this.Money = new (money);
      this.Fame = new (fame);
      this.Coin = new (coin);
    }

    [Inject]
    public void Init(
      ITimeFlowController timeFlowController,
      IFacilitiesController facilitiesController,
      DomAthService domAthService,
      CoachService coachService
     // IAthleteController athleteController
      )
    {
      this.facilitiesController = facilitiesController;
      this.timeFlowController = timeFlowController;
      //this.athleteController = athleteController;
      this.domAthService = domAthService;
      this.coachService = coachService;
      this.timeFlowController.CurrentSeason
        .Subscribe(season => {
          int currentYear = this.timeFlowController.Year.Value;
          int yearSpan = currentYear - this.timeFlowController.Start.year ;
          this.AccountFinancialFor(season, yearSpan);
          });
    }

    public void AddFame(int amount)
    {
      #if UNITY_EDITOR
      if (amount <= 0) {
        throw (new ArgumentException($"{nameof(AddFame)}: {nameof(amount)} <= 0"));
      }
      #endif
      this.Fame.Value += amount;
    }

    public void AddCoin(int amount)
    {
      #if UNITY_EDITOR
      if (amount <= 0) {
        throw (new ArgumentException($"{nameof(AddCoin)}: {nameof(amount)} <= 0"));
      }
      #endif
      this.Coin.Value += amount;
    }

    public void AddMoney(int amount, IncomeType type)
    {
      #if UNITY_EDITOR
      if (amount <= 0) {
        throw (new ArgumentException($"{nameof(AddIncome)}: {nameof(amount)} <= 0"));
      }
      #endif
      this.AddIncome(amount, type);
      this.Money.Value += amount;
    }

    public void SpendMoney(int amount, ExpensesType type)
    {
      #if UNITY_EDITOR
      if (amount < 0) {
        throw (new ArgumentException($"{nameof(SpendMoney)}: {nameof(amount)} < 0"));
      }
      #endif
      this.AddExpense(amount, type);
      this.Money.Value -= amount;
    }

    public void SpendCoin(int amount)
    {
      #if UNITY_EDITOR
      if (amount < 0) {
        throw (new ArgumentException($"{nameof(SpendCoin)}: {nameof(amount)} < 0"));
      }
      #endif
      this.Coin.Value -= amount;
    }

    void AccountFinancialFor(Season season, int yearSpan)
    {
      int income = this.GetIncome(season, yearSpan);
      int expenses = this.GetExpenses(season, yearSpan);

      int totalIncome = 0;
      foreach (var (_, incomeAmount) in this.incomes) {
        totalIncome += incomeAmount; 
      }

      int totalExpenses = 0;
      foreach (var (type, expenseAmount) in this.expenses) {
        totalExpenses += expenseAmount; 
      }

      var financeData = new SeasonFinanceData {
        Incomes = new (this.incomes),
        Expenses = new (this.expenses),
        Income = totalIncome,
        Expense = totalExpenses,
        Total = totalIncome - totalExpenses
      };
      this.incomes.Clear();
      this.expenses.Clear();

      this.Money.Value += income - expenses;
      this.LastSeasonReport.Value = financeData;
    }

    int GetIncome(Season season, int yearSpan)
    {
      int trainingGrant = this.Data.TrainingGrantByYears[yearSpan].Incomes[(int)season];
      this.AddIncome(trainingGrant, IncomeType.TrainingGrant);
      int competitionGrant = this.Data.CompetitionGrantByYears[yearSpan].Incomes[(int)season];
      this.AddIncome(competitionGrant, IncomeType.CompetitionGrant);
      return (trainingGrant + competitionGrant);
    }

    int GetExpenses(Season season, int yearSpan)
    {
      //FIXME: Avoid null references
      if (season == Season.Spring && yearSpan == 0) {
        return (0);
      }
      this.CountAthletes(
        athletes: this.domAthService.GetAllRecruitedAthleteList(),
        generalAthletes: out int general,
        nationalAthleteCandidates: out int nationalCandidates,
        nationalAthletes: out int national
        );
      int personnelCost = this.Data.PersonnelCost.GetTotal(
        generalAthlete: general,
        nationalAthleteCandidate: nationalCandidates,
        nationalAthlete: national,
        coach: this.coachService.GetRecruitedCoaches().Count);

      this.AddExpense(personnelCost, ExpensesType.PersonnelMaintainance);
      int facilitiesCost = 0;
      foreach (var facility in this.facilitiesController.Facilities) {
        int stage = facility.CurrentStage.Value; 
        facilitiesCost += this.Data.FacilityCost.GetCostFor(stage);
      }
      this.AddExpense(facilitiesCost, ExpensesType.FacilityMaintainance);
      return (personnelCost + facilitiesCost);
    }

    void CountAthletes(
      IList<DomAthEntity> athletes, 
      out int generalAthletes,
      out int nationalAthleteCandidates, 
      out int nationalAthletes)
    {
      generalAthletes = 0; 
      nationalAthleteCandidates = 0;
      nationalAthletes = 0;
      foreach (var athlete in athletes) {
        switch ((int)athlete.affiliation) {
          case 0:
            generalAthletes++;
            break;
          case 1:
            nationalAthleteCandidates++;
            break;
          case 2:
            nationalAthletes++;
            break;
        }
      }
    }


    void AddIncome(int amount, IncomeType type)
    {
      if (this.incomes.TryGetValue(type, out int income)) {
        this.incomes[type] = income + amount;
      }
      else {
        this.incomes.Add(type, amount);
      }
    }

    void AddExpense(int amount, ExpensesType type)
    {
      if (this.expenses.TryGetValue(type, out int expense)) {
        this.expenses[type] = expense + amount;
      }
      else {
        this.expenses.Add(type, amount);
      }
    }
  }
}
