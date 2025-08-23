using System;

namespace SHG
{
  using Season = ITimeFlowController.Season;

  /// <summary>
  /// 한 계절 동안의 수익금
  /// </summary>
  [Serializable]
  public struct IncomeForSeason
  {
    public int[] Incomes;

    public int GetIncomeFor(Season season)
    {
      return (this.Incomes[(int)season]);
    }
  }

  /// <summary>
  /// 선수, 코치들의 대한 인력 유지비용
  /// </summary>
  [Serializable]
  public struct PersonnelMatainanceCost
  {
    /// <summary> 일반 선수 인원 수</summary>
    public int GeneralAthlete;
    /// <summary> 국가 대표 후보 선수  인원 수 </summary>
    public int NationalAthleteCandidate;
    /// <summary> 국가 대표 후보 인원 수</summary>
    public int NationalAthlete;
    /// <summary> 코치 인원 수</summary>
    public int Coach;

    /// <summary>
    /// 각 선수 종류, 코치 1명당 유지비를 적용한 총 유지 비용을 계산하는 기능
    /// </summary>
    /// <param name="generalAthlete">일반 선수 유지 비</param>
    /// <param name="nationalAthleteCandidate">국가 대표 후보 선수 유지비 </param>
    /// <param name="nationalAthlete">국가 대표 선수 유지비</param>
    /// <param name="coach">코치 유지비</param>
    public int GetTotal(int generalAthlete, int nationalAthleteCandidate, int nationalAthlete, int coach)
    {
      return (
        (this.GeneralAthlete * generalAthlete) +
        (this.NationalAthleteCandidate * nationalAthleteCandidate) +
        (this.NationalAthlete * nationalAthlete) +
        (this.Coach * coach)
        );
    }
  }

  /// <summary>
  /// 시설 유지 비용 
  /// </summary>
  [Serializable]
  public struct FacilityMaintainanceCost
  {
    /// <summary>
    /// 각 등급에 대한 유지 비용
    /// </summary>
    public int[] CostByStage; 

    public int GetCostFor(int stage)
    {
      return (this.CostByStage[stage]);
    }
  }

  /// <summary>
  /// 재화에 대한 기본 정보
  /// </summary>
  [Serializable]
  public struct ResourceData
  {
    /// <summary>
    /// 각 연차별로 계절에 대한 훈련 지원금
    /// </summary>
    public IncomeForSeason[] TrainingGrantByYears;
    /// <summary>
    /// 각 연차별로 계절에 대한 경기 지원금
    /// </summary>
    public IncomeForSeason[] CompetitionGrantByYears;
    /// <summary>
    /// 각 연차별로 계절에 대한 퀘스트 보상금
    /// </summary>
    public IncomeForSeason[] QuestPrizes;
    /// <summary>
    /// 인력에 대한 유지비
    /// </summary>
    public PersonnelMatainanceCost PersonnelCost;
    /// <summary>
    /// 시설에 대한 유지비
    /// </summary>
    public FacilityMaintainanceCost FacilityCost;
  }
}
