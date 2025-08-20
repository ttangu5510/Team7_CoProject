using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;

namespace JYL
{
    public class Test_JYL_SaveManager : MonoBehaviour // 테스트용 세이브 매니저. CSV에서 불러오거나 Json으로 저장하는 기능없음
    { // Zenject 사용시, MonoBehaviour를 탈피하고 IInitializable 상속해서 구현 가능
        #if UNITY_EDITOR
        private static string savePath = Application.dataPath + "/Programming/JYL/Test_Save";
        #else
        private static string savePath = Application.persistentDataPath + "/Save";
        #endif

        public static Test_JYL_SaveManager Instance;
        public List<SaveData> saves = new();
        public SaveData curSave;
        public Dictionary<string, DateTime> savedTime = new(); // 세이브 파일이 저장된 시간 딕셔너리
        public Dictionary<string, SaveData> saveDataByName = new(); //세이브 객체를 이름으로 찾는 딕셔너리
        
        public void Init() // 세이브 데이터를 전부 불러옴. Zenject 사용 시, 알아서 처리되게 할 수 있음
        {
            // 경로에서 모든 세이브 파일 불러오기
            LoadAllSave();
            Instance = this;
        }
        public void CreateSaveData(string playerName) // 게임을 새로 시작할 때 사용함. UI에서 사용할 함수
        {
            SaveData save = new SaveData();
            save.Init(playerName);
            saves.Add(save);
            curSave = save;
            SaveProgress(save);
        }

        public void AutoSave() // 자동 저장에 사용되는 함수. 턴 넘길 때마다 사용. 이벤트 순서에서 로직부분 맨 마지막에 추가
        {
            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }

            string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss");
            string fileName = "AutoSave.json"; // 자동저장에 사용되는 파일은 하나 뿐
            savedTime[fileName] =  DateTime.Now;
            saveDataByName[fileName] = curSave;

            string path = Path.Combine(savePath, fileName);
            string json = JsonUtility.ToJson(curSave);
            File.WriteAllText(path, json);
            
            Debug.Log($"자동 저장됨{path}");
        }

        public void AutoLoad() // 자동 저장 된 파일들 중에서 자동 불러오기에 사용됨
        {
            if (saveDataByName.TryGetValue("AutoSave.json", out var value))
            {
                curSave = value; // 전체 파일을 불러오는 과정이 선행되기 때문에 가능함
            }
            else
            {
                Debug.LogWarning("저장된 세이브 파일이 없음_AutoSave.json");
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
            save.saveTime = timestamp;
            
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
                string fileName = Path.GetFileName(file);
                savedTime[fileName] = File.GetCreationTime(file);
                saveDataByName[fileName] = save;
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

        public void UpdateAthleteEntity(DomAthEntity entity) // 선수 세이브 객체를 가지고 선수 객체 최신화
        {
            AthleteSave save = curSave.FindAthlete(entity);
            if (save != null)
            {
                entity.UpdateFromSave(save);
            }
            else
            {
                Debug.LogWarning($"선수 세이브 객체를 찾지 못함_{entity.name}");
            }
        }

        public void RecruitAthlete(DomAthEntity entity) // 선수 영입 시 현재 세이브 객체에 선수세이브 추가
        {
            AthleteSave athlete = new(entity);
            curSave.athleteSaves.Add(athlete);
        }

        public void OutAthlete(DomAthEntity entity) //선수 방출. 세이브 객체에서 삭제
        {
            int index= curSave.athleteSaves.FindIndex(x=>x.id == entity.id);
            if(index >=0) curSave.athleteSaves.RemoveAt(index);
            else Debug.LogWarning($"해당 선수의 세이브데이터가 존재하지 않음{entity.name}");
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

        public AthleteSave FindAthlete(DomAthEntity entity)
        {
            return athleteSaves.Find(x => x.id == entity.id);
        }
    }
}

