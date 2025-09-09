using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;
using Zenject;

namespace JYL
{
    [Serializable]
    public class ForAthService : MonoBehaviour
    {
        [Inject] private readonly IForAthRepository repository; // 외부에서 의존성 주입 DI
        private IDisposable subscription;
        
        // TODO : 에디터 확인용 테스트 리스트, 딕셔너리
        [SerializeField] public List<ForAthEntity> forAthList = new();
        [SerializeField] public Dictionary<string, List<ForAthEntity>> forAthDict = new();
        private void Awake()
        {
            Init();
        }

        public void Init()
        {
            forAthList.Clear();
            forAthDict.Clear();
            
            forAthList = repository.FindAll();
            forAthDict = repository.OpponentDictByString();
        }

        public List<ForAthEntity> GetAllAthleteList() // 모든 국적의 선수가 포함된 리스트
        {
            return repository.FindAll();
        }

        public List<ForAthEntity> GetAthleteListByNation(string nationName) // 특정 국가 이름(string)을 기준으로 리스트 반환
        {
            return repository.OpponentDictByString()[nationName];
        }

        public List<ForAthEntity> GetAllAthleteListByNation(AthleteNation nation) // 국가 이름(열거체) 기준으로 리스트 반환
        {
            return repository.FindAll().Where(ent => ent.nation == nation).ToList();
        }

        public Dictionary<string, List<ForAthEntity>> GetOpponentDict() // 국가별로 정리된 리스트 반환
        {
            return repository.OpponentDictByString();
        }

        public Dictionary<AthleteNation, List<ForAthEntity>> GetOpponentDictByEnum() // 국가별(열거체)로 정리된 리스트 반환
        {
            return repository.OpponentDictByEnum();
        }
        
    }
}

