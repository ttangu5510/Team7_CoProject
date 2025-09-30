using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using UnityEngine;
using JWS;
using Unity.VisualScripting;
using UnityEditor.Rendering;

namespace JYL
{
    public class SaveManager : ISaveManager // 세이브 매니저. 인터페이스를 상속함.
    {
        #if UNITY_EDITOR
        private static string savePath = Application.dataPath + "/Programming/JYL/Test_Save";
        #else
        private static string savePath = Application.persistentDataPath + "/Save";
        #endif

        private AchievementWrapper achievementWrapper;
        
        private List<SaveData> saves = new();
        private SaveData curSave;

        private readonly Dictionary<string, DateTime> savedTime = new(); // 세이브 파일이 저장된 시간 딕셔너리
        private readonly Dictionary<string, SaveData> saveDataByName = new(); //세이브 객체를 이름으로 찾는 딕셔너리

        private int slotIndex = -1; // 현재 선택중인 세이브데이터의 인덱스
        private long PlayTimeTick = 0; // 실제 플레이 타임 재는 타이머

        
        
        #region 초기화
        public void Initialize() // IInitializable 인터페이스 구현 함수
        {
            LoadAllSave();
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
            
            Debug.Log($"{files.Length} 불러온 파일 갯수");
            foreach (var file in files)
            {
                SaveData save = JsonUtility.FromJson<SaveData>(File.ReadAllText(file));
                
                saves.Add(save);
                string fileName = Path.GetFileName(file);
                savedTime[fileName] = File.GetCreationTime(file);
                saveDataByName[fileName] = save;
            }
        }
        #endregion

        #region 세이브
        public void CreateSaveData(int slotNumber) // 슬롯 넘버 기반 세이브파일 생성. 인게임 UI에서 사용함
        {
            if (slotNumber == 0)
            {
                Debug.LogWarning("세이브에 들어온 인덱스가 잘못됨. 0 이상이어야 함");
                return;
            }
            SaveProgress(curSave,slotNumber);
        }

        public void CreateAutoSaveData(string playerName, string clanName, string uid) // 게임 맨 처음 시작할 때, 이름 입력한 것으로 세이브파일 생성.
        {
            SaveData save = new SaveData();
            save.Init(uid,playerName,clanName);

            // (만준추가
            save.foundedUtcIso = DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss");

            saves.Add(save);
            curSave = save;
            achievementWrapper = new(curSave.achievementRecord);
            AutoSave();
        }

        public void AutoSave() // 자동 저장에 사용되는 함수.
                               // 턴 넘길 때마다 사용.
                               // 이벤트 순서에서 로직부분 맨 마지막에 추가.
                               // 이름 입력하면 맨 처음에 오토세이브 한 번 함
        {
            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }

            string timestamp = DateTime.UtcNow.ToString("o");
            string fileName = "AutoSave.json"; // 자동저장에 사용되는 파일은 하나 뿐
            
            // 세이브 데이터의 인덱스 최신화
            curSave.saveSlotIndex = 0; // 오토세이브 슬롯번호. 수동은 1부터 시작
            
            // 딕셔너리 최신화
            savedTime[fileName] =  DateTime.UtcNow;
            saveDataByName[fileName] = curSave;
            
            // 현재까지의 플레이 시간 저장
            DateTime lastSavedTime = DateTime.TryParse(curSave.time.lastSaveUtcIso, out DateTime lastSaved) ? lastSaved : DateTime.UtcNow;
            PlayTimeTick = DateTime.UtcNow.Ticks - lastSavedTime.Ticks;
            curSave.time.playTick =  PlayTimeTick;
            
            // 마지막으로 저장된 시간 갱신
            curSave.time.lastSaveUtcIso = timestamp;
           
            // 현재 상태 저장
            string path = Path.Combine(savePath, fileName);
            string json = JsonUtility.ToJson(curSave,true);
            File.WriteAllText(path, json);
            
            Debug.Log($"자동 저장됨{path}");
        }
        
        public void SaveProgress(int slotNumber) // 현재 사용중인 세이브 객체를 저장할 때 사용하는 함수
        {
            SaveProgress(curSave, slotNumber);
        }
        
        public void SaveProgress(SaveData save, int slotNumber) // 현재 사용중인 세이브 객체를 세이브 파일로 저장함. 슬롯번호 기준
        {
            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }
            
            // 저장될 파일 이름 설정
            string fileName = $"Save_{slotNumber}.json";

            // 입력받은 세이브로 세로운 record 세이브 객체 생성
            // TODO : 기술문서 작성( mutable, immutable, record, jsonUtility, with )
            SaveData newSave = save.CloneSave();
            
            // 세이브 슬롯 인덱스 저장
            newSave.saveSlotIndex = slotNumber;
            
            // 딕셔너리 최신화
            savedTime[fileName] = DateTime.UtcNow;
            saveDataByName[fileName] = newSave;
            
            // 현재까지의 플레이 시간 저장
            DateTime lastSavedTime = DateTime.TryParse(newSave.time.lastSaveUtcIso,null, DateTimeStyles.AdjustToUniversal, out DateTime lastSaved) ? lastSaved : DateTime.UtcNow;
            PlayTimeTick = DateTime.UtcNow.Ticks - lastSavedTime.Ticks;
            newSave.time.playTick +=  PlayTimeTick;
            
            // 현재시간 받아오기
            string timestamp = DateTime.UtcNow.ToString("o");
            // 마지막으로 저장된 시간 갱신
            newSave.time.lastSaveUtcIso = timestamp;
            
            // 세이브 파일 저장
            string path = Path.Combine(savePath, fileName);
            string json = JsonUtility.ToJson(newSave,true);
            File.WriteAllText(path,json);
            
            Debug.Log($"세이브 파일 저장됨{path}");
            
            // 세이브 객체 리스트에 새로운 객체 추가. 현재 사용중인 세이브 객체 변경
            saves.Remove(saves.Find(index => index.saveSlotIndex == slotNumber)); // 기존 동일한 세이브 슬롯의 객체는 제거
            saves.Add(newSave);
        }
        #endregion
        
        #region 로드
        public void LoadProgress(SaveData save) // 현재 선택중인 세이브 파일을 변경함
        {
            SaveData newSave = save.CloneSave();
            curSave = newSave;
            achievementWrapper = new AchievementWrapper(curSave.achievementRecord);
        }

        public void LoadProgress(string fileName) // 이름으로 불러올 수 있게 만듦. 어떤 걸 쓰게 될 지 모름.
        {
            curSave = saveDataByName[fileName];
            achievementWrapper = new AchievementWrapper(curSave.achievementRecord);
        }
        
        #endregion
        
        #region 삭제

        public void DeleteSaveFile(SaveData save, int inputIndex) // 파일을 삭제. 세이브 데이터 객체도 삭제 (로비화면이라 가능)
        {
            string filePath;
            if (inputIndex == 0)
            {
                filePath = Path.Combine(savePath, $"AutoSave.json");
                saveDataByName.Remove("AutoSave.json");
            }
            else
            {
                filePath = Path.Combine(savePath, $"Save_{inputIndex}.json");
                saveDataByName.Remove($"Save_{inputIndex}.json");
            }
            File.Delete(filePath);
            save.saveSlotIndex = -1; // 세이브 슬롯의 번호를 저장 안된 것으로 변경
        }
        #endregion
        
        #region 선수 영입, 은퇴, 방출, 업데이트
        public void RecruitAthlete(DomAthEntity entity) // 선수 영입 시 현재 세이브 객체에 선수세이브 추가
        {
            AthleteSave athlete = new(entity);
            curSave.athleteSaves.Add(athlete);
            achievementWrapper.AthleteRecruitCount.Value++;
        }

        // 은퇴는 파라매터만 바뀌고, 저장됨
        public void RetireAthlete(DomAthEntity entity)
        {
            AthleteSave athlete = curSave.FindAthlete(entity);
            athlete.state = AthleteState.Retired;
            achievementWrapper.AthleteRetireCount.Value++;
        }
        public void OutAthlete(DomAthEntity entity) //선수 방출. 세이브 객체에서 삭제
        {
            AthleteSave athlete= curSave.FindAthlete(entity);
            curSave.athleteSaves.Remove(athlete);
        }
        public void UpdateAthleteEntity(DomAthEntity entity) // 선수 세이브 객체로 선수 객체를 최신화
        {
            if (curSave == null) return;
            AthleteSave save;
            save = curSave.FindAthlete(entity);

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
            if (entity.grade == CoachGrade.스카우트센터) // 코치가 일반 등급이면, 세이브 객체를 생성 후 저장함.
            {
                CoachSave newCoach = new(entity); // 생성자로 코치 객체를 기준으로 생성
                curSave.coachSaves.Add(newCoach);
            }
            
            else if(entity.grade == CoachGrade.선수출신)// 후보 이상급 코치면 세이브 파일이 있는지 먼저 체크한 후 로직 진행함. 은퇴해야지만 Hidden에서 Unrecruited로 됨.
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
                // 업적 카운트 적용
                achievementWrapper.CoachRecruitCount.Value++;
            }
            else
            {
                Debug.LogWarning($"코치 그레이드가 잘못 적용됨{entity.entityName}__{entity.grade}");
            }
        }

        public void RetireCoach(CoachEntity entity) // Repository에서 사용. 코치 세이브 객체를 찾은 후, 은퇴로 상태 변경
        {
            curSave.FindCoach(entity).state = CoachState.Retired;
            // 배치 중이면 배치 해제
            for (int i = 0; i < curSave.coachAssign.Length; i++)
            {
                if (curSave.coachAssign[i] == entity.id)
                {
                    curSave.coachAssign[i] = -1;
                }
            }
        }

        public void OutCoach(CoachEntity entity) // Repository 코치 방출에서 사용
        {
            if (entity.curAge.Value >= entity.retireAge) // 예외처리. 은퇴 나이보다 높거나 같으므로 은퇴로 변경
            {
                RetireCoach(entity);
                Debug.Log($"은퇴나이를 넘음{entity.entityName}_현재:{entity.curAge}_은퇴:{entity.retireAge}");
                return;
            } 
            
            CoachSave coach = curSave.FindCoach(entity); // 코치 동적 객체 찾음
            // 후보급 이상에서 온 선수면, 나이가 무조건 28세로 돌아감.
            // 세이브 객체 삭제 안함(다시 영입하려면 Hidden 초기값을 피해야함)
            if (entity.grade == CoachGrade.선수출신)
            {
                coach.state = CoachState.Unrecruited;
            }
            
            // 일반급 코치는 그냥 세이브 데이터 삭제하면 됨.
            // 코치 객체에서는 Unrecruited로 있기 때문에, 문제없이 스카우트 센터에서 보임
            else
            {
                curSave.coachSaves.Remove(coach);
            }
            
            // 배치 중이면 배치 해제
            for (int i = 0; i < curSave.coachAssign.Length; i++)
            {
                Debug.Log($"엔티티 id : {entity.id} / 배열 id : {curSave.coachAssign[i]} / {curSave.coachAssign[i] == entity.id}");
                if (curSave.coachAssign[i] == entity.id)
                {
                    curSave.coachAssign[i] = -1;
                    Debug.Log($"변경 후 된 값 : {curSave.coachAssign[i]}");
                }
            }
        }

        public void UpdateCoachEntity(CoachEntity entity) // 세이브 객체를 통해 코치 동적 객체를 최신화 함
        {
            if (curSave == null) return;
            CoachSave save;
            save = curSave.FindCoach(entity); // 세이브 객체 찾기
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

        #region 코치 배치

        public int[] GetAssignedCoaches() // 현재 배치 중인 코치 배열
        {
            return (int[])curSave.coachAssign.Clone();
        }

        public void SetAssignedCoaches(int[] assignedCoaches) // 배치 중인 코치 배열 업데이트
        {
            curSave.coachAssign = (int[])assignedCoaches.Clone();
        }
        #endregion
        
        #region 치료실 배치
        /// 현재 치료실에 배치된 선수 ID 배열 반환
        public int[] GetAssignedTreatmentAthletes()
        {
            return (int[])curSave.treatmentAssign.Clone();
        }
        
        /// 치료실 슬롯 전체를 갱신
        public void SetAssignedTreatmentAthletes(int[] assignedAthletes)
        {
            curSave.treatmentAssign = (int[])assignedAthletes.Clone();
        }
        
        /// 치료실 전체 리셋 (모든 슬롯 해제)
        public void ResetTreatmentAssign()
        {
            for (int i = 0; i < curSave.treatmentAssign.Length; i++)
                curSave.treatmentAssign[i] = -1;
        }
        #endregion
        
        #region 리스트 추출
        // 세이브 데이터 리스트 반환
        public List<SaveData> GetAllSave()
        {
            return saves;
        }
        
        public SaveData GetCurrentSave()
        {
            return curSave;
        }

        public void SetSlotIndex(int index)
        {
            slotIndex = index;
        }

        public int GetCurrentSlotIndex()
        {
            return slotIndex;
        }

        public SaveData GetAutoSaveData()
        {
            if (!saveDataByName.TryGetValue("AutoSave.json", out SaveData autoSave))
            {
                Debug.Log("오토세이브 없음");
                return null;
            }
            return autoSave;
        }
        #endregion
        
        #region 업적
        public AchievementWrapper GetAchievementWrapper() // 업적 Reactive 전달.
        {
            return achievementWrapper;
        }
        #endregion
    }
}

