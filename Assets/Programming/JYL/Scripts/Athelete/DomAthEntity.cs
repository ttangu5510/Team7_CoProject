using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace JYL
{ 
    // 캐릭터의 정보를 CSV에서 읽어와서 만드는 캐릭터 정보 객체
public class DomAthEntity : BaseAthEntity
{
    public AthleteGrade maxGrade { get; private set; } //최대 성장 가능성
    public AthleteAffiliation affiliation { get; private set; } // 선수 소속
    
    public int recruitAge { get; private set; } // 초기 나이
    public int retireAge { get; private set; } // 은퇴 나이
    public int curAge { get; private set; } // 현재 나이
    
    // 능력치
    public int health { get; private set; } // 체력
    public int quickness { get; private set; } // 순발력
    public int flexibility { get; private set; } // 유연성
    public int technic { get; private set; } // 기술
    public int speed { get; private set; } // 속도
    public int balance { get; private set; } // 균형감각
    public int fatigue { get; private set; } // 피로도
    
    // 현재 상태
    public bool isRecruited { get; private set; } // 영입됨?
    public bool isInjured { get; private set; } // 부상당함?
    public bool isRetired { get; private set; } // 은퇴했음?
    
    // 게임을 시작하면, 전체 리스트를 초기화 할 예정. 각 선수들의 초기화에만 쓰임. Service에서 수행함.
    public DomAthEntity Init(int id, string name, AthleteAffiliation affiliation, AthleteGrade maxGrade, int recruitAge,
        int health, int quickness, int flexibility, int technic, int speed, int balance,
        int retireAge = 28, int fatigue = 0, bool isRecruited = false, bool isInjured = false, bool isRetired = false)

    {
        // 여기서 CSV에서 데이터를 읽어와 Init으로 전체 파라매터를 채움
        this.id = id;
        this.name = name;
        this.affiliation = affiliation;
        this.maxGrade = maxGrade;
        this.recruitAge = recruitAge;
        curAge = recruitAge;
        this.retireAge = retireAge;
        this.health = health;
        this.quickness = quickness;
        this.flexibility = flexibility;
        this.technic = technic;
        this.speed = speed;
        this.balance = balance;
        this.fatigue = fatigue;
        this.isRecruited = isRecruited;
        this.isInjured = isInjured;
        this.isRetired = isRetired;

        return this;
    }

    
    
    public void Recruit() // 선수 영입할 때 쓰는 함수. 재화 나가는건 다른데서 처리해야 함
    {
        // TODO : 확률  시설 UI 쪽에서 확률 건드려야 함
        if (recruitAge < retireAge)
        {
            isRecruited = true;
        }
    }

    
    
    public bool Retire() // 선수 은퇴할 때 쓰는 함수. 코치 되는건 선수의 등급에 의해서 결정됨. true면 코치가 될 수 있는거임. TODO : 나이 값에 이벤트 걸어야 함
    {
        isRetired = true;
        bool canCoach = false;
        if (curAge >= retireAge)
        {
            if (affiliation == AthleteAffiliation.National)
            {
                canCoach =  true; // 코치 될 수 있음
            }
        }
        return canCoach; // UI에서 canCoach값에 따라, 영입 UI 띄우게 함.
        // TODO : 세이브 데이터 지움. 
    }

    
    
    public void OutAthlete() // 선수 퇴출할 때 쓰는 함수. 모든 능력치 및 나이가 초기화 된다 (회춘)
    {
        isRecruited = false;
        // TODO : 세이브 데이터 지워야 함. 서비스에서 보유 선수 리스트 최신화
    }

    
    
    // 선수를 훈련 시킬 때 사용하는 함수. 각 능력치가 변화하는 것에 이벤트를 걸 수 있다.
    public void TrainAthlete(in AthleteStatus status, int amount = 1, int coach = 0) 
    {
        // 부상 당하면 훈련 실패
        int rand = Random.Range(0, 100);

        if (fatigue >= 100)
        {
            fatigue = 100;
            if (rand > 10) isInjured = true;
        }
        else if (fatigue >= 80)
        {
            if (rand > 50) isInjured = true;
        }
        else if (fatigue >= 60)
        {
            if (rand > 80) isInjured = true;
        }
        if (isInjured)
        {
            Debug.Log("부상입음");
            return;
        }
        
        // 부상은 안당했고, 훈련 시작
        int maxStat = ((int)maxGrade + 1) * 100;
        switch (status)
        {
            case AthleteStatus.Health:
                health += amount;
                speed += amount;
                if (health >= maxStat) health = maxStat;
                if (speed >= maxStat) speed = maxStat;  
                break;
            case AthleteStatus.Quickness:
                quickness += amount;
                health += amount;
                if(quickness >= maxStat) quickness = maxStat;
                if(health >= maxStat)  health = maxStat;
                break;
            case AthleteStatus.Flexibility:
                flexibility += amount;
                technic += amount;
                if (flexibility >= maxStat) flexibility = maxStat;
                if (technic >= maxStat) technic = maxStat;
                break;
            case AthleteStatus.Balance:
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
        // 훈련 완료 후 피로도 증가. 코치가 있을 경우, 코치 버프만큼 감소
        fatigue += Random.Range(7, 12) - coach;
    }
}
}
