using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JYL
{
    public abstract class BaseAthEntity : MonoBehaviour
    {
        public int id { get; protected set; } // 고유식별번호
        public string entityName { get; protected set; } // 이름
    
    }

    public enum AthleteAffiliation // 선수 소속
    {
        Regular, Prospect, National
    }

    public enum AthleteGrade // 선수 성장 등급
    {
        F,E,D,C,B,A
    }

    public enum Ability
    {
        Health,
        Quickness,
        Flexibility,
        Technic,
        Speed,
        Balance
    }
}
