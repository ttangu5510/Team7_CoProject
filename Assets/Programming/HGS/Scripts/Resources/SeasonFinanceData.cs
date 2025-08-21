using System;
using System.Collections.Generic;

namespace SHG
{

  public enum IncomeType
  {
    TrainingGrant,
    CompetitionGrant,
    QuestPrizes
  }

  public enum ExpensesType
  {
    PersonnelMaintainance,
    FacilityMaintainance,
    FacilityUpgrade,
    Scout
  }

  [Serializable]
  public struct SeasonFinanceData 
  {
    public Dictionary<IncomeType, int> Incomes;
    public Dictionary<ExpensesType, int> Expenses;
    public int Income;
    public int Expense;
    public int Total;
  }
}
