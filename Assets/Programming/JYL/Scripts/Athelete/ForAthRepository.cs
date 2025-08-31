using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace JYL
{
    public interface IForAthRepository
    {
        ForAthEntity FindById(int id);
        ForAthEntity FindByName(string name);
        List<ForAthEntity> FindAll();
        Dictionary<string, List<ForAthEntity>> OpponentDictByString();
        Dictionary<AthleteNation, List<ForAthEntity>> OpponentDictByEnum();
        
    }
    
    public class ForAthRepository : IForAthRepository
    {
        private Dictionary<string, ForAthEntity> forAthDict { get; set; } = new();

        public ForAthRepository()
        {
            Init();
        }

        private void Init() // 선수 딕셔너리 초기화
        {
            forAthDict.Clear();

            var csvData = CsvReader.ReadOpponents("ForAthTable");
            foreach (var data in csvData)
            {
                var entity = ForAthFactory.CreateEntityFromCSV(data);
                if (!forAthDict.TryAdd(entity.entityName, entity))
                {
                    Debug.LogWarning($"이미 추가된 선수임{entity.entityName}");
                }
            }
        }
        
        public ForAthEntity FindById(int id)
        {
            return forAthDict.Values.Where(ent => ent.id == id) as ForAthEntity;
        }

        public ForAthEntity FindByName(string name)
        {
            return forAthDict[name];
        }

        public List<ForAthEntity> FindAll()
        {
            return forAthDict.Values.ToList();
        }

        public Dictionary<string, List<ForAthEntity>> OpponentDictByString()
        {
                return forAthDict.Values.GroupBy(ent => ent.nation)
                    .ToDictionary(group => group.Key.ToString(),entities => entities.ToList());
        }

        public Dictionary<AthleteNation, List<ForAthEntity>> OpponentDictByEnum()
        {
            return forAthDict.Values.GroupBy(ent => ent.nation).ToDictionary(k => k.Key, ent => ent.ToList());
        }
    } 
}

