using System;
using System.Collections.Generic;

namespace SHG
{

  /// <summary>
  /// 수익 종류: 훈련 지원금, 경기 지원금, 퀘스트 보상금
  /// </summary>
  public enum IncomeType
  {
    TrainingGrant,
    CompetitionGrant,
    QuestPrizes
  }

  /// <summary>
  /// 지출 종류: 인력 유지비, 시설 유지비, 시설 업그레이드 비용, 스카우트 비용
  /// </summary>
  public enum ExpensesType
  {
    PersonnelMaintainance,
    FacilityMaintainance,
    FacilityUpgrade,
    Scout
  }

  /// <summary>
  /// 한 계절 동안의 손익에 대한 정보
  /// </summary>
  [Serializable]
  public struct SeasonFinanceData 
  {
    /// <summary>
    /// 각 수익 종류에 따른 금액
    /// </summary>
    public Dictionary<IncomeType, int> Incomes;
    /// <summary>
    /// 각 지출 종류에 따른 비용
    /// </summary>
    public Dictionary<ExpensesType, int> Expenses;
    /// <summary> 총 수익 </summary>
    public int Income;
    /// <summary> 총 지출 </summary>
    public int Expense;
    /// <summary> 손익 총합 </summary>
    public int Total;
  }
}
