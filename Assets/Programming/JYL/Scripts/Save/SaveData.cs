using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JYL
{
    [Serializable]
    public class SaveData //Json으로 저장될 세이브 데이터 클래스
    {
        public string playerName;
        public int playerGold;
        public int playerFame;
        public int progressWeek;
        public string saveTime;
        public List<AthleteSave> athleteSaves;
        public List<CoachSave> coachSaves;

        public void Init(string name) // 세이브 파일 최초 생성시에 사용
        {
            playerName = name;
            playerGold = 0;
            playerFame = 0;
            progressWeek = 0;
            saveTime = DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss");
            athleteSaves = new List<AthleteSave>();
        }

        public AthleteSave FindAthlete(DomAthEntity entity)
        {
            return athleteSaves.Find(x => x.id == entity.id);
        }

        public CoachSave FindCoach(CoachEntity entity)
        {
            return coachSaves.Find(x => x.id == entity.id);
        }
    }
}
