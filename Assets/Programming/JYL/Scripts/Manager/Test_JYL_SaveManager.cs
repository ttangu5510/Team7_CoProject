using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Net;
using UnityEngine;

namespace JYL
{
    public class Test_JYL_SaveManager : MonoBehaviour // 테스트용 세이브 매니저. CSV에서 불러오거나 Json으로 저장하는 기능없음
    {
        #if UNITY_EDITOR
        private string savePath = Application.dataPath + "/Programming/JYL/Test_Save";
        #else
        private string savePath = Application.persistentDataPath + "/Save";
        #endif
        
        public List<SaveData> saves = new();
        public SaveData curSave;
        public Dictionary<string, DateTime> savedTime = new(); // 세이브 파일이 저장된 시간
        public Dictionary<string, SaveData> saveDataByName = new(); //세이브 객체를 이름으로 찾음

        public void Init() // 세이브 데이터를 전부 불러옴
        {
            // 경로에서 모든 세이브 파일 불러오기
            LoadAllSave();
        }
        public void CreateSaveData(string playerName) // 게임을 새로 시작할 때 사용함.
        {
            SaveData save = new SaveData();
            save.Init(playerName);
            saves.Add(save);
            curSave = save;
            SaveProgress(save);
        }

        public void AutoSave() // 자동 저장에 사용되는 함수
        {
            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }

            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss");
            string fileName = "AutoSave.json";
            savedTime[fileName] =  DateTime.Now;
            saveDataByName[fileName] = curSave;

            string path = Path.Combine(savePath, fileName);
            string json = JsonUtility.ToJson(curSave);
            File.WriteAllText(path, json);
            
            Debug.Log($"자동 저장됨{path}");
        }

        public void AutoLoad() // 자동 저장 된 파일들 중에서 자동 불러오기에 사용됨
        {
            if (saves.Count > 0)
            {
                curSave = saveDataByName["AutoSave.json"];
            }
            else
            {
                Debug.LogWarning("저장된 세이브 파일이 없음");
            }
        }

        public void SaveProgress(SaveData save) // 현재 사용중인 세이브 객체를 세이브 파일로 저장함.
        {
            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss");
            string fileName = $"Save_{save.playerName}_{timestamp}.json";
            savedTime[fileName] = DateTime.Now;
            saveDataByName[fileName] = save;
            string path = Path.Combine(savePath, fileName);
            
            string json = JsonUtility.ToJson(save,true);
            File.WriteAllText(path,json);
            
            Debug.Log($"세이브 파일 저장됨{path}");
        }

        private void LoadAllSave() // 모든 세이브 파일을 경로상에서 불러오고 리스트에 넣음
        {
            saves.Clear(); // 불러오기 전에 비우기
            savedTime.Clear();
            saveDataByName.Clear();

            if (!Directory.Exists(savePath))
            {
                Debug.LogWarning($"경로에 폴더가 없음{savePath}");
                return;
            }
            
            // 경로에 폴더가 있으면 들어옴. 모든 세이브 파일을 불러와야 함.
            string[] files = Directory.GetFiles(savePath,"*.json");
            
            foreach (var file in files)
            {
                SaveData save = JsonUtility.FromJson<SaveData>(File.ReadAllText(file));
                
                saves.Add(save);
                string fileName = Path.GetFileNameWithoutExtension(file);
                string fullName = $"{fileName}.json";
                savedTime[fullName] = File.GetCreationTime(Path.Combine(savePath, fullName));
                saveDataByName[fullName] = save;
            }
        }

        public void LoadProgress(SaveData save) // 현재 선택중인 세이브 파일을 변경함
        {
            curSave = save;
        }

        public void LoadProgress(string fileName) // 이름으로 불러올 수 있게 만듦. 어떤 걸 쓰게 될 지 모름.
        {
            curSave = saveDataByName[fileName];
        }
    }

    [Serializable]
    public class SaveData //Json으로 저장될 세이브 데이터 클래스
    {
        public string playerName;
        public int playerGold;
        public int playerFame;
        public int progressWeek;
        public string saveTime;
        public List<AthleteSave> athleteSaves;

        public void Init(string name) // 세이브 파일 최초 생성시에 사용
        {
            playerName = name;
            playerGold = 0;
            playerFame = 0;
            progressWeek = 0;
            saveTime = DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss");
            athleteSaves = new List<AthleteSave>();
        }
    }
}

