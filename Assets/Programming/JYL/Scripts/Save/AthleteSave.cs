using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JYL
{
    [System.Serializable]
    public class AthleteSave // json에 저장할 선수의 정보 클래스
    {
        public int id;
        
        public int health;
        public int quickness;
        public int flexibility;
        public int technic;
        public int speed;
        public int balance;
        public int fatigue;
        
        public int leftInjury;
        public bool isRecruited;
        public bool isInjured;
        public bool isRetired;

        public AthleteSave(int id, int health, int quickness, int flexibility, int technic, int speed,int balance,int fatigue, 
            int leftInjury, bool isRecruited, bool isInjured, bool isRetired)
        {
            this.id = id;
            
            this.health = health;
            this.quickness = quickness;
            this.flexibility = flexibility;
            this.technic = technic;
            this.speed = speed;
            this.balance = balance;
            this.fatigue = fatigue;
            
            this.leftInjury = leftInjury;
            this.isRecruited = isRecruited;
            this.isInjured = isInjured;
            this.isRetired = isRetired;
        }
    }
}

