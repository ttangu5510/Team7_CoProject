using System;
using System.Collections.Generic;
using Zenject;

namespace JYL
{
    public interface ISaveManager : IInitializable
    {
        // 세이브
        void CreateSaveData(string playerName);
        void AutoSave();
        void SaveProgress(SaveData save);

        // 로드
        void AutoLoad();
        void LoadProgress(SaveData save);
        void LoadProgress(string fileName);
        SaveData GetCurrentSave();
        
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
    }
}
