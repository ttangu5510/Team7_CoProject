using System;
using JYL;

namespace SHG
{
  [Serializable]
  public struct AthleteData : IContender {
    public string Name;
    public string Grade;
    public float Fatigue;
    public int Id;
    public string CountryName;
    public AthleteStats stats;
    public AthleteStats Stats => this.stats;

    AthleteAffiliation IContender.Grade => (this.Grade switch {
      "일반 선수" => (AthleteAffiliation)0,
      "국가대표 후보" => (AthleteAffiliation)1,
      "국가대표" => (AthleteAffiliation)2,
      _ => throw (new ApplicationException())
      });

    public IGroup Team;
    string IContender.Name => (this.Name);
    int IContender.Id => this.Id;
    IGroup IContender.Group => (this.Team != null ? this.Team: new Country { Name = this.CountryName });
  }

  public class ConvertedDomesticAthlete : IContender {

    DomAthEntity athlete;

    public ConvertedDomesticAthlete(DomAthEntity athlete)
    {
      this.athlete = athlete;
    }

    public static IGroup USER_TEAM = new Team { Name = "user team" };
    public AthleteStats Stats => (this.athlete.stats);
    public AthleteAffiliation Grade => (this.athlete.affiliation);
    public IGroup Group => (USER_TEAM);
    public string Name => (this.athlete.entityName);

    public int Id => this.athlete.id;

    public bool IsSameWith(DomAthEntity athlete)
    {
      return (this.athlete == athlete);
    }

    public bool IsSameWith(IContender athlete)
    {
      if (athlete is ConvertedDomesticAthlete converted) {
        return (this.athlete == converted.athlete);
      }
      return (false);
    }
  }
}
