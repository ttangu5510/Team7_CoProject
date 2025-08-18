using System;
using UniRx;
using UnityEngine;

namespace SHG
{
  using Season = ITimeFlowController.Season;

  /// <summary>
  /// 시간에 흐름을 관리하는 클래스 ,  ITimeFlowController를 참고
  /// </summary>
  public class TimeFlowController : ITimeFlowController {
    static int WEEK_FOR_SEASON = 10;
    static int WEEK_FOR_YEAR = 4 * WEEK_FOR_SEASON;
    static int START_YEAR = 2023;
    static int START_WEEK = 1;

    public ReactiveProperty<Season> CurrentSeason { get; private set; }
    public ReactiveProperty<int> WeekInYear { get; private set; }
    public ReactiveProperty<int> Year { get; private set; }
    (int year, int week) start;
    int week;

    public void SetDate(int year, int weekInYear) {
      this.Year.Value = year;
      this.week = weekInYear - 1;
      this.WeekInYear.Value = weekInYear;
      this.CurrentSeason.Value = this.GetSeason(this.week);
    }

    public TimeFlowController(): this(START_YEAR, START_WEEK)
    {
    }

    public TimeFlowController(int year, int week)
    {
      this.week = week - 1;
      this.start = (year, week);
      this.WeekInYear =  new (week);
      this.CurrentSeason = new (this.GetSeason(this.week));
      this.Year = new (year);
    }

    public void ProgressWeek()
    {
      this.ProgressWeeks(1);
    }

    public void ProgressWeeks(int weeks)
    {
      this.week += weeks;
      int yearToAdd = this.week / WEEK_FOR_YEAR;
      this.week = this.week % WEEK_FOR_YEAR;
      if (yearToAdd > 0) {
        this.Year.Value += yearToAdd;
      }
      this.WeekInYear.Value = this.week + 1;
      this.CurrentSeason.Value = this.GetSeason(this.week);
    }

    Season GetSeason(int week)
    {
      return (Season)(week / WEEK_FOR_SEASON);
    }
  }
}
