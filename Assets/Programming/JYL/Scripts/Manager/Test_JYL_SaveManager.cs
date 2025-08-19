using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;

namespace JYL
{
    public class Test_JYL_SaveManager : MonoBehaviour // 테스트용 세이브 매니저. CSV에서 불러오거나 Json으로 저장하는 기능없음
    {
        private string savePath;
        public List<SaveData> saveData;
        public SaveData curSave;

        public void Init() // 세이브 데이터를 전부 불러옴
        {
            // 경로에서 모든 세이브 파일 불러오기
        }
        public void CreateSaveData(string name) // 세이브 파일 생성에 사용
        {
            SaveData saveData = new SaveData();
            saveData.Init("Tester1");
            // 선수 숫자만큼 세이브 만들어서 넣기
        }

        public void SaveProgress()
        {
            string json = JsonUtility.ToJson(curSave);
            File.WriteAllText($"Assets/Programming/JYL/Test_Save/{curSave.playerName}",json);
        }
        public void LoadProgress(){ }
        
    }

    [System.Serializable]
    public class SaveData //Json으로 저장될 세이브 데이터 클래스
    {
        public string playerName;
        public int playerGold;
        public int playerFame;
        public List<AthleteSave> athleteSaves;

        public void Init(string name)
        {
            playerName = name;
            playerGold = 0;
            playerFame = 0;
            athleteSaves = new List<AthleteSave>();
        }
    }
}

