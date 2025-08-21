using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JYL
{
    [System.Serializable]
    public class AthleteSave // json에 저장할 선수의 정보 클래스
    {
        public int id;
        public int age;
        
        public int health;
        public int quickness;
        public int flexibility;
        public int technic;
        public int speed;
        public int balance;
        public int fatigue;

        public AthleteState state;

        public AthleteSave(DomAthEntity entity) // 생성자. 선수 정보 초기화
        {
            id = entity.id;
            age = entity.curAge;
            
            health = entity.stats.health;
            quickness = entity.stats.quickness;
            flexibility = entity.stats.flexibility;
            technic = entity.stats.technic;
            speed = entity.stats.speed;
            balance = entity.stats.balance;
            fatigue = entity.stats.fatigue;

            state = entity.curState;
        }

        public void UpdateStatus(DomAthEntity entity) // 선수 정보 최신화
        {
            age = entity.curAge;
            
            health = entity.stats.health;
            quickness = entity.stats.quickness;
            flexibility = entity.stats.flexibility;
            technic = entity.stats.technic;
            speed = entity.stats.speed;
            balance = entity.stats.balance;
            fatigue = entity.stats.fatigue;
            
            state =  entity.curState;
        }
    }
}

