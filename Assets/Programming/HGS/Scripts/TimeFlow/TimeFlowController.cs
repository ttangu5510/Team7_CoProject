using System;
using UniRx;

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
    public ReactiveProperty<int> Month { get; private set; }
    (int year, int week) start;
    int week;

    public void SetDate(int year, int weekInYear) {
      this.Year.Value = year;
      this.Month.Value = weekInYear / WEEK_FOR_SEASON + 1;
      this.WeekInYear.Value = weekInYear;
      this.CurrentSeason.Value = this.GetSeason(this.Month.Value);
    }

    public TimeFlowController(): this(START_YEAR, START_WEEK)
    {
    }

    public TimeFlowController(int year, int week)
    {
      this.week = week - 1;
      this.start = (year, week);
      this.WeekInYear =  new (week);
      this.Month = new (this.week / 4 + 1);
      this.CurrentSeason = new (this.GetSeason(this.Month.Value));
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
      this.Month.Value = this.week / WEEK_FOR_SEASON + 1;
      this.CurrentSeason.Value = this.GetSeason(this.Month.Value);
    }

    Season GetSeason(in int month)
    {
      switch (month) {
        case 12:
        case 1:
        case 2:
          return (Season.Winter);
        case 3:
        case 4:
        case 5:
          return (Season.Spring);
        case 6:
        case 7:
        case 8:
          return (Season.Summer);
        case 9:
        case 10:
        case 11:
          return (Season.Fall);
        default: 
          throw new ApplicationException($"{nameof(GetSeason)}: {month}");
      }
    }
  }
}
