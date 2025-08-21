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

        public CoachSave(int id, int age, CoachState state, CoachGrade grade)
        {
            this.id = id;
            this.age = age;
            this.state = state;
        }
    }
}
