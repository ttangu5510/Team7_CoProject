using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JYL
{
    
    public class CoachEntity : BaseAthEntity
    {
        public void Recruit() // 코치 영입
        {
            
        }

        public void Retire() // 코치 은퇴
        {
            
        }

        public void OutCoach() // 코치 방출
        {
            
        }
    }

    [System.Serializable]
    public enum CoachState
    {
        Unrecruited,
        Recruited,
        Retired
    }

    [System.Serializable]
    public enum CoachGrade
    {
        Normal,
        National
    }
}

