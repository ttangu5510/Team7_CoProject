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
        
        public readonly Dictionary<string, DateTime> savedTime = new(); // 세이브 파일이 저장된 시간 딕셔너리
        public readonly Dictionary<string, SaveData> saveDataByName = new(); //세이브 객체를 이름으로 찾는 딕셔너리
        
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

#region 선수 영입, 은퇴, 방출, 업데이트
        public void RecruitAthlete(DomAthEntity entity) // 선수 영입 시 현재 세이브 객체에 선수세이브 추가
        {
            AthleteSave athlete = new(entity);
            curSave.athleteSaves.Add(athlete);
        }

        // 은퇴는 파라매터만 바뀌고, 저장됨
        public void RetireAthlete(DomAthEntity entity)
        {
            AthleteSave athlete = curSave.FindAthlete(entity);
            athlete.state = AthleteState.Retired;
        }
        public void OutAthlete(DomAthEntity entity) //선수 방출. 세이브 객체에서 삭제
        {
            AthleteSave athlete= curSave.FindAthlete(entity);
            curSave.athleteSaves.Remove(athlete);
        }
        public void UpdateAthleteEntity(DomAthEntity entity) // 선수 세이브 객체로 선수 객체를 최신화
        {
            AthleteSave save = curSave.FindAthlete(entity);
            if (save != null)
            {
                entity.UpdateFromSave(save);
            }
            else
            {
                Debug.Log($"선수 세이브 객체를 찾지 못함_{entity.entityName}");
            }
        }
#endregion



#region 코치 영입, 은퇴, 방출, 업데이트

        public void RecruitCoach(CoachEntity entity) // Repository에서 사용. 코치 세이브 객체 생성. 현재 세이브 객체에 추가
        {
            if (entity.grade == CoachGrade.Veteran) // 코치가 일반 등급이면, 세이브 객체를 생성 후 저장함.
            {
                CoachSave coach = new(entity); // 생성자로 코치 객체를 기준으로 생성
                curSave.coachSaves.Add(coach);
            }
            
            else // 후보 이상급 코치면 세이브 파일이 있는지 먼저 체크한 후 로직 진행함
            {
                CoachSave newCoach = curSave.FindCoach(entity);
                if (newCoach != null)
                {
                    newCoach.UpdateStatus(entity); // 세이브 파일이 있으면 업데이트
                }
                else
                {
                    CoachSave save = new(entity);
                    curSave.coachSaves.Add(save);
                }
            }
        }

        public void RetireCoach(CoachEntity entity) // Repository에서 사용. 코치 세이브 객체를 찾은 후, 은퇴로 상태 변경
        {
            curSave.FindCoach(entity).state = CoachState.Retired;
        }

        public void OutCoach(CoachEntity entity) // Repository 코치 방출에서 사용
        {
            if (entity.curAge >= entity.retireAge) // 예외처리. 은퇴 나이보다 높거나 같으므로 은퇴로 변경
            {
                RetireCoach(entity);
                Debug.Log($"은퇴나이를 넘음{entity.entityName}_현재:{entity.curAge}_은퇴:{entity.retireAge}");
                return;
            } 
            
            CoachSave coach = curSave.FindCoach(entity); // 코치 동적 객체 찾음
            // 후보급 이상에서 온 선수면, 나이가 무조건 28세로 돌아감.
            // 세이브 객체 삭제 안함(다시 영입하려면 Hidden 초기값을 피해야함)
            if (entity.grade == CoachGrade.Master)
            {
                coach.age = 28;
                coach.state = CoachState.Unrecruited;
            }
            
            // 일반급 코치는 그냥 세이브 데이터 삭제하면 됨.
            // 코치 객체에서는 Unrecruited로 있기 때문에, 문제없이 스카우트 센터에서 보임
            else
            {
                curSave.coachSaves.Remove(coach);
            }
        }

        public void UpdateCoachEntity(CoachEntity entity) // 세이브 객체를 통해 코치 동적 객체를 최신화 함
        {
            CoachSave save = curSave.FindCoach(entity); // 세이브 객체 찾기
            if (save != null)
            {
                entity.UpdateFromSave(save); // 세이브 객체를 통해 최신화
            }
            else
            {
                Debug.Log($"코치 세이브 객체가 없음{entity.entityName}");
            }
        }
        #endregion 
    }
}

