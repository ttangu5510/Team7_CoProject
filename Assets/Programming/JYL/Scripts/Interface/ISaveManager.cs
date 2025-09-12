using System;
using System.Collections.Generic;
using Zenject;
using JWS;

namespace JYL
{
    public interface ISaveManager : IInitializable
    {
        // 세이브 
        void CreateSaveData(int slotNumber = 0);
        void CreateAutoSaveData(string playerName, string clanName, string uid = "TestAutoSave123");
        void AutoSave();
        void SaveProgress(SaveData save, int slotNumber);
        void SaveProgress(int slotNumber);

        // 로드
        // void AutoLoad();
        void LoadProgress(SaveData save);
        void LoadProgress(string fileName);
        
        // 파일 삭제
        void DeleteSaveFile(SaveData save, int inputIndex);
        
        // 선수
        void RecruitAthlete(DomAthEntity entity);
        void RetireAthlete(DomAthEntity entity);
        void OutAthlete(DomAthEntity entity);
        void UpdateAthleteEntity(DomAthEntity entity);
        
        // 코치
        void RecruitCoach(CoachEntity entity);
        void RetireCoach(CoachEntity entity);
        void OutCoach(CoachEntity entity);
        void UpdateCoachEntity(CoachEntity entity);
        int[] GetAssignedCoaches();
        void SetAssignedCoaches(int[] assignedCoaches);
        
        // 리스트 추출
        List<SaveData> GetAllSave();
        SaveData GetCurrentSave();
        void SetSlotIndex(int slotIndex);
        int GetCurrentSlotIndex();
        SaveData GetAutoSaveData();
    }
}
