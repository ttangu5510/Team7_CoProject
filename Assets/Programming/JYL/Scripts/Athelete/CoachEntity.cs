using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace JYL
{
    [Serializable]
    public class CoachEntity : BaseAthEntity
    {
        public CoachState curState { get; private set; }
        public CoachGrade grade { get; private set; }
        public int retireAge { get; private set; }
        public ReactiveProperty<int> curAge { get; private set; } = new();

        public CoachEntity(int id, string name, CoachGrade grade, int age = 28)
        {
            this.id = id;
            entityName = name;
            this.grade = grade;
            curState = grade == CoachGrade.선수출신 ? CoachState.Hidden : CoachState.Unrecruited;
            retireAge = 40;
            curAge.Value = age;
        }

        public void UpdateFromSave(CoachSave save) // 코치 업데이트. 세이브 객체로 업데이트함. Repository에서 수행.
        {
            curState = save.state;
            if (curState == CoachState.Recruited)
            {
                curAge.Value = save.age;
            }
        }
        public void Recruit() // 코치 영입 . Repository에서 수행
        {
            curState = CoachState.Recruited;
        }

        public void Retire() // 코치 은퇴. TODO : 나이 변화에 이벤트 필요
        {
            curState = CoachState.Retired;
        }

        public void OutCoach() // 코치 방출. Repository에서 수행.
        {
            curState = CoachState.Unrecruited;
        }

        public void AthleteToCoach() // 선수가 코치로 전환
        {
            if (curState == CoachState.Hidden)
            {
                curState = CoachState.Unrecruited;
            }
        }
    }

    [System.Serializable]
    public enum CoachState
    {
        Unrecruited,
        Recruited,
        Retired,
        Hidden,
    }

    [System.Serializable]
    public enum CoachGrade
    {
        스카우트센터 = 1,
        선수출신
    }
}

