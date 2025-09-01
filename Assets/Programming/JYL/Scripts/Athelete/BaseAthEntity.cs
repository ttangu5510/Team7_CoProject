using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JYL
{
    public abstract class BaseAthEntity
    {
        public int id { get; protected set; } // 고유식별번호
        public string entityName { get; protected set; } // 이름

    }

    public enum AthleteAffiliation // 선수 소속
    {
        일반선수,
        국가대표후보,
        국가대표
    }


    public enum AthleteGrade // 선수 성장 등급
    {
        F,
        E,
        D,
        C,
        B,
        A,
        S,
        SS,
        SSS
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

    public enum AthleteNation
    {
        그리스 = 12,
        노르웨이,
        독일,
        미국,
        일본,
        중국,
        헝가리,
        대한민국
    }
    
    [Serializable]
    public class AthleteStats // 선수들의 능력치를 담당하는 값 객체
    {
        public int health { get; private set; } // 체력
        public int quickness { get; private set; } // 순발력
        public int flexibility { get; private set; } // 유연성
        public int technic { get; private set; } // 기술
        public int speed { get; private set; } // 속도
        public int balance { get; private set; } // 균형감각
        public int fatigue { get; private set; } // 피로도

        public AthleteStats(int health, int quickness, int flexibility, int technic, int speed, int balance)
        {   // 생성자
            this.health = health;
            this.quickness = quickness;
            this.flexibility = flexibility;
            this.technic = technic;
            this.speed = speed;
            this.balance = balance;
            fatigue = 0;
        }

        public AthleteStats ApplyTrainValue(in Ability ability, int amount, int maxStat)
        {
            AthleteStats newStat = new(this.health,this.quickness,this.flexibility,this.technic,this.speed,this.balance);
            switch (ability)
            {
                case Ability.Health:
                    health += amount;
                    speed += amount;
                    if (health >= maxStat) health = maxStat;
                    if (speed >= maxStat) speed = maxStat;  
                    break;
                case Ability.Quickness:
                    quickness += amount;
                    health += amount;
                    if(quickness >= maxStat) quickness = maxStat;
                    if(health >= maxStat)  health = maxStat;
                    break;
                case Ability.Flexibility:
                    flexibility += amount;
                    technic += amount;
                    if (flexibility >= maxStat) flexibility = maxStat;
                    if (technic >= maxStat) technic = maxStat;
                    break;
                case Ability.Balance:
                    balance += amount;
                    speed += amount;
                    if (balance >= maxStat) balance = maxStat;
                    if (speed >= maxStat) speed = maxStat;
                    break;
                // case AthleteStatus.Technic:
                //     technic += amount;
                //     if (technic >= maxStat) technic = maxStat;
                //     break;
                // case AthleteStatus.Speed:
                //     speed += amount;
                //     if (speed >= maxStat) speed = maxStat;
                //     break;
            }
            return newStat;
        }

        public void SetFatigue(int amount)
        {
            fatigue += amount;
            if(fatigue > 100) fatigue = 100;
            else if(fatigue < 0) fatigue = 0;
        }
    } 
}
