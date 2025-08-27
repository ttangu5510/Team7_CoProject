using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.AI;

namespace JYL
{
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
    // 캐릭터의 정보를 CSV에서 읽어와서 만드는 캐릭터 정보 객체
public class DomAthEntity : BaseAthEntity
{
    public AthleteGrade maxGrade { get; private set; } //최대 성장 가능성
    public AthleteAffiliation affiliation { get; private set; } // 선수 소속
    
    public int recruitAge { get; private set; } // 초기 나이
    public int retireAge { get; private set; } // 은퇴 나이
    public ReactiveProperty<int> curAge { get; private set; } = new();// 현재 나이
    
    // 능력치
    public AthleteStats stats { get; private set; }
    
    // 현재 상태
    public AthleteState curState { get; private set; }
    public int leftInjury { get; private set; } // 회복에 필요한 남은 부상 턴
    
    // 게임을 시작하면, 전체 리스트를 초기화 할 예정. 각 선수들의 초기화에만 쓰임. Factory에서 수행함.
    public void Init(int id, string name, AthleteAffiliation affiliation, AthleteGrade maxGrade, int recruitAge,
        int health, int quickness, int flexibility, int technic, int speed, int balance,
        int retireAge = 28)

    {
        // 여기서 CSV에서 데이터를 읽어와 Init으로 전체 파라매터를 채움
        this.id = id;
        entityName = name;
        this.affiliation = affiliation;
        this.maxGrade = maxGrade;
        this.recruitAge = recruitAge;
        curAge.Value = recruitAge;
        this.retireAge = retireAge;
        stats = new AthleteStats(health, quickness, flexibility, technic, speed, balance);
        curState = AthleteState.Unrecruited;
    }

    public void UpdateFromSave(AthleteSave save) //세이브 파일로부터 업데이트
    {
        curAge.Value = save.age;

        stats = new AthleteStats(save.health, save.quickness, save.flexibility, save.technic, save.speed, save.balance);
        stats.SetFatigue(save.fatigue);
        curState = save.state;
    }
    
    public void Recruit() // 선수 영입할 때 쓰는 함수. 재화 나가는건 다른데서 처리해야 함
    {
        if (recruitAge < retireAge)
        {
            curState = AthleteState.Active;
        }
    }

    
    
    public void Retire() // 선수 은퇴 시 사용하는 함수
    {
        if (curAge.Value >= retireAge && curState !=  AthleteState.Unrecruited) // 은퇴 나이가 되었고, 영입이 된 상태라면
        {
            curState = AthleteState.Retired; // 은퇴 상태로 변경
        }
    }

    
    
    public void OutAthlete() // 선수 퇴출할 때 쓰는 함수. 모든 능력치 및 나이가 초기화 된다 (회춘)
    {
        curState = AthleteState.Unrecruited;
    }

    
    
    // 선수를 훈련 시킬 때 사용하는 함수. 각 능력치가 변화하는 것에 이벤트를 걸 수 있다.
    public void TrainAthlete(in Ability ability, int amount = 1, int coach = 0) 
    {
        // 부상 당하면 훈련 실패
        int rand = Random.Range(0, 100);

        if (stats.fatigue >= 100)
        {
            if (rand > 10) curState = AthleteState.Injured;
        }
        else if (stats.fatigue >= 80)
        {
            if (rand > 50) curState = AthleteState.Injured;
        }
        else if (stats.fatigue >= 60)
        {
            if (rand > 80) curState = AthleteState.Injured;
        }
        if (curState == AthleteState.Injured)
        {
            Debug.Log("부상입음");
            leftInjury = 2;
            return;
        }
        
        // 부상은 안당했고, 훈련 시작
        int maxStat = ((int)maxGrade + 1) * 100;
        stats.ApplyTrainValue(ability, amount, maxStat);
        
        // 훈련 완료 후 피로도 증가. 코치가 있을 경우, 코치 버프만큼 감소
        stats.SetFatigue(Random.Range(7, 12) - coach);
    }

    public void RecoverAthlete(int amount) // 선수 회복에 쓰이는 함수
    {
        leftInjury -= amount;
        if (leftInjury <= 0)
        {
            leftInjury = 0;
            curState = AthleteState.Active;
        }
    }

    public void GetAge()
    {
        curAge.Value++;
    }
}
[System.Serializable]
public enum AthleteState // 선수의 현재 상태
{
    Unrecruited,// 영입되지 않음
    Active,     // 영입됨
    Injured,    // 영입됨. 부상
    Retired     // 영입됨. 은퇴
}
}
