using System.Text;
using UnityEngine;
using Zenject;
using UniRx;
using EditorAttributes;

namespace SHG
{
  public class ResourceTester : MonoBehaviour
  {
    [Inject]
    IResourceController resourceController;
    [Inject]
    ITimeFlowController timeFlowController;

    [SerializeField] [FoldoutGroup("LastSeasonReport", nameof(lastReportTimeText), nameof(total), nameof(income), nameof(incomeDetail), nameof(expenses), nameof(expensesDetail), nameof(clearButton))]
    Void reportGroup;
    [SerializeField] [ReadOnly] [HideInInspector]
    string lastReportTimeText;
    [SerializeField] [ReadOnly] [HideInInspector]
    int total;
    [SerializeField] [ReadOnly] [HideInInspector]
    int income;
    [SerializeField] [ReadOnly] [HideInInspector]
    string incomeDetail;
    [SerializeField] [ReadOnly] [HideInInspector]
    int expenses;
    [SerializeField] [ReadOnly] [HideInInspector]
    string expensesDetail;
    [SerializeField] [HideInInspector] [ButtonField(nameof(Clear))]
    Void clearButton;
    
    [SerializeField] [FoldoutGroup("Resources", nameof(currentMoney), nameof(currentCoin), nameof(currentFame))]
    Void resourceButton;
    [SerializeField] [ReadOnly] [HideInInspector]
    int currentMoney;
    [SerializeField] [ReadOnly] [HideInInspector]
    int currentFame;
    [SerializeField] [ReadOnly] [HideInInspector]
    int currentCoin;

    [SerializeField]
    int param;

    [SerializeField] [TabGroup(nameof(spendGroup), nameof(addGroup))]
    Void groupHolder;

    [SerializeField] [HideInInspector] [HorizontalGroup(true, nameof(spendCoinButton), nameof(spendScountFeeButton), nameof(spendFacilityUpgradeFeeButton))]
    Void spendGroup;
    [SerializeField] [HideInInspector] [ButtonField(nameof(SpendCoin))]
    Void spendCoinButton;
    [SerializeField] [HideInInspector] [ButtonField(nameof(SpendScout))]
    Void spendScountFeeButton;
    [SerializeField] [HideInInspector] [ButtonField(nameof(SpendUpgrade))]
    Void spendFacilityUpgradeFeeButton;

    [SerializeField] [HideInInspector] [HorizontalGroup(true, nameof(addQuestPrizeButton), nameof(addFameButton), nameof(addCoinButton))]
    Void addGroup;

    [SerializeField] [HideInInspector] [ButtonField(nameof(AddPrize))]
    Void addQuestPrizeButton;

    [SerializeField] [HideInInspector] [ButtonField(nameof(AddFame))]
    Void addFameButton;

    [SerializeField] [HideInInspector] [ButtonField(nameof(AddCoin))]
    Void addCoinButton;

    // Start is called before the first frame update
    void Start()
    {
      this.resourceController.LastSeasonReport
        .Subscribe(report => {
          if (report != null) {
            this.GetReport(report.Value);
          }});
      this.resourceController.Money
        .Subscribe(money => this.currentMoney = money);
      this.resourceController.Fame
        .Subscribe(fame => this.currentFame = fame);
      this.resourceController.Coin
        .Subscribe(coin => this.currentCoin = coin);
    }

    void Clear()
    {
      this.lastReportTimeText = string.Empty;
      this.total = 0;
      this.income = 0;
      this.incomeDetail = string.Empty;
      this.expenses = 0;
      this.expensesDetail = string.Empty;
    }

    void GetReport(SeasonFinanceData report)
    {
      int currentYear = this.timeFlowController.Year.Value;
      int yearSpan = currentYear - this.timeFlowController.Start.year + 1;
      this.lastReportTimeText = $"Year: {yearSpan}, season: {this.timeFlowController.CurrentSeason.Value}";

      this.total = report.Total;
      this.income = report.Income;
      this.expenses = report.Expense;
      var incomeString = new StringBuilder();
      foreach (var (incomeType, income)  in report.Incomes) {
        incomeString.Append($"[{incomeType}: {income}]; ");
      }
      this.incomeDetail = incomeString.ToString();
      var expensesString = new StringBuilder();
      foreach (var (expenseType, expense) in report.Expenses) {
        expensesString.Append($"[{expenseType}: {expense}]"); 
      }
      this.expensesDetail = expensesString.ToString();
    }

    void SpendScout()
    {
      this.resourceController.SpendMoney(this.param, ExpensesType.Scout);
      this.param = 0;
    }

    void SpendUpgrade()
    {
      this.resourceController.SpendMoney(this.param, ExpensesType.FacilityUpgrade);
      this.param = 0;
    }

    void SpendCoin()
    {
      this.resourceController.SpendCoin(this.param);
      this.param = 0;
    }

    void AddPrize()
    {
      this.resourceController.AddMoney(this.param, IncomeType.QuestPrizes);
      this.param = 0;
    }

    void AddCoin()
    {
      this.resourceController.AddCoin(this.param);
      this.param = 0;
    }

    void AddFame()
    {
      this.resourceController.AddFame(this.param);
      this.param = 0;
    }
  }
}
