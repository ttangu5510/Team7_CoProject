using System;
using UnityEngine;

namespace SHG
{
    
  public interface IGroup
  {
    public enum GroupType 
    {
      Country,
      Team
    } 
    
    public GroupType Type { get; }
    public String Name { get; }

    public bool Equals(object obj) {
      if (obj is Country other) {
        return (this.Equals(other));
      }
      return (false);
    }

    public bool Equals(IGroup other) {
      if (other.Type != this.Type) {
        return (false);
      }
      if (this is Country thisCountry &&
        other is Country otherCountry) {
        return (thisCountry == otherCountry); 
      }
      if (this is Team thisTeam &&
        other is Team otherTeam) {
        return (thisTeam == otherTeam);
      }
      return (false);
    }
  }

  /// <summary>
  /// 경기에 출전하는 국가
  /// </summary>
  [Serializable]
  public struct Country: IGroup
  {
    [SerializeField]
    public string Name;
    string IGroup.Name => this.Name;

    public IGroup.GroupType Type => IGroup.GroupType.Country;

    public override bool Equals(object obj) {
      if (obj is Country other) {
        return (this == other);
      }
      return (false);
    }

    public static bool operator== (Country countryA, Country countryB) {
      return (countryA.Name == countryB.Name);
    }

    public static bool operator!= (Country countryA, Country countryB) {
      return (!(countryA == countryB));
    }

    public override int GetHashCode() {
      return (this.Name.GetHashCode());
    }

    public override string ToString() {
      return ($"[{nameof(Country)}; {nameof(Name)}: {this.Name};]");
    }
  }

  public struct Team: IGroup
  {
    [SerializeField]
    public string Name;

    public IGroup.GroupType Type => IGroup.GroupType.Team;

    string IGroup.Name => (this.Name);

    public override bool Equals(object obj) {
      if (obj is Team other) {
        return (this == other);
      }
      return (false);
    }

    public static bool operator== (Team teamA, Team teamB) {
      return (teamA.Name == teamB.Name);
    }

    public static bool operator!= (Team teamA, Team teamB) {
      return (!(teamA == teamB));
    }

    public override int GetHashCode() {
      return (this.Name.GetHashCode());
    }

    public override string ToString() {
      return ($"[{nameof(Team)}; {nameof(Name)}: {this.Name};]");
    }
  }
}
