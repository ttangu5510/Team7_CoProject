using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JYL
{
    [System.Serializable]
    public class CoachSave //코치 정보를 저장하는 세이브 객체
    {
        public int id;
        public int age;
        public CoachState state;

        public CoachSave(CoachEntity entity)
        {
            id = entity.id;
            age = entity.curAge;
            state = entity.curState;
        }

        public void UpdateStatus(CoachEntity entity)
        {
            age =  entity.curAge;
            state = entity.curState;
        }

        public void UpdateStatus(int age, CoachState state)
        {
            this.age = age;
            this.state = state;
        }
    }
}
