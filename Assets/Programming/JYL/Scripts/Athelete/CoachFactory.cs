using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JYL
{
    public class CoachFactory
    {
        public static CoachEntity CreateCoachEntity(int i)
        {
            CoachEntity entity = new(10222, $"coach_{i}", CoachGrade.Veteran, 32);
            return entity;
        }
        // TODO : CSVReader완성되면 사용
        // public static CoachEntity CreateCoachEntity(CsvData row)
        // {
        //     CoachEntity entity = new(...);
        //      return entity;
        // }
    }    
}

