using System;
using System.Collections.Generic;
using UniRx;

namespace SHG
{

  /// <summary>
  /// 시간에 흐름을 관리하는 클래스 ,  ITimeFlowController를 참고
  /// </summary>
  public class TimeFlowController : ITimeFlowController {

    public ReactiveProperty<Season> CurrentSeason { get; private set; }
    public ReactiveProperty<int> WeekInYear { get; private set; }
    public ReactiveProperty<int> Year { get; private set; }
    public ReactiveCollection<GameDate> DateToEnd { get; private set; }
    public (int year, int week) Start { get; private set; }
    public int YearPassedAfterStart => (this.Year.Value - this.Start.year  + 1);
    int week;
    public Action BeforeProgress { get; set; }

    public void SetDate(int year, int weekInYear) {
      this.Year.Value = year;
      this.week = weekInYear - 1;
      this.WeekInYear.Value = weekInYear;
      this.CurrentSeason.Value = this.GetSeason(this.week);
    }

    public TimeFlowController(
      int year = ITimeFlowController.START_YEAR, 
      int week = ITimeFlowController.START_WEEK)
    {
      this.week = week;
      this.Start = (year, week);
      this.WeekInYear =  new (week + 1);
      this.CurrentSeason = new (this.GetSeason(this.week));
      this.Year = new (year);
      this.DateToEnd = new (this.GetDateToEnd());
    }

    public void ProgressWeek()
    {
      this.ProgressWeeks(1);
    }

    public void ProgressWeeks(int weeks)
    {
      this.BeforeProgress?.Invoke();
      this.week += weeks;
      int yearToAdd = this.week / ITimeFlowController.WEEK_FOR_YEAR;
      this.week = this.week % ITimeFlowController.WEEK_FOR_YEAR;
      if (yearToAdd > 0) {
        this.Year.Value += yearToAdd;
      }
      this.WeekInYear.Value = this.week + 1;
      this.CurrentSeason.Value = this.GetSeason(this.week);
      for (int i = 0; i < weeks; i++) {
        this.DateToEnd.RemoveAt(0);
      }
    }

    GameDate[] GetDateToEnd()
    {
      int count = 0;
      int weeksLeftThisYear = ITimeFlowController.WEEK_FOR_YEAR - this.WeekInYear.Value + 1;
      count += weeksLeftThisYear;
      int yearsLeft = ITimeFlowController.END_YEAR - this.Year.Value;
      count += yearsLeft * ITimeFlowController.WEEK_FOR_YEAR;

      var allGameDate = new List<GameDate>(count);
      var yearAfterStart = this.YearPassedAfterStart;
      var weekInYear = this.WeekInYear.Value;
      for (int i = 0; i < weeksLeftThisYear; i++, weekInYear++) {
        allGameDate.Add(new GameDate { Year = yearAfterStart, Week = weekInYear });
      }

      for (int year = 1; year <= yearsLeft; ++year) {
        for (int i = 0; i < ITimeFlowController.WEEK_FOR_YEAR; i++) {
          allGameDate.Add(new GameDate {
            Year = yearAfterStart + year,
            Week = i + 1
          });
        }
      }

      return (allGameDate.ToArray());
    }

    Season GetSeason(int week)
    {
      return (Season)(week / ITimeFlowController.WEEK_FOR_SEASON);
    }

  }
}
