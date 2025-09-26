using System.Linq;
using JWS;

public static class DataIntegration
{
    /// <summary>
    /// 선수단 이름 반환
    /// </summary>
    public static string GetClanName(SaveData data) => data.clanName;

    /// <summary>
    /// 선수단 창립일을 문자열로 반환
    /// 예: "1년차 Spring 1주차"
    /// </summary>
    public static string GetFoundedDate(SaveData data) =>
        $"{data.time.yearCycle}년차 {data.time.season} {data.time.week}주차";

    /// <summary>
    /// 현재 보유 중인 선수 수 반환
    /// </summary>
    public static int GetOwnedAthleteCount(SaveData data) => data.athleteSaves.Count;

    /// <summary>
    /// 은퇴시킨 선수 수 반환
    /// SaveData의 AchievementRecord에서 관리됨
    /// </summary>
    public static int GetRetiredAthleteCount(SaveData data) => data.achievementRecord.athleteRetireCount;

    /// <summary>
    /// 경기 참가 횟수 반환
    /// SaveData의 AchievementRecord에서 관리됨
    /// </summary>
    public static int GetMatchEntryCount(SaveData data) => data.achievementRecord.matchEntryCount;

    /// <summary>
    /// 획득한 메달 수 반환
    /// 도감(encyclopedia)에 기록된 obtainedCount 합산
    /// </summary>
    public static int GetMedalCount(SaveData data) => data.encyclopedia.Sum(e => e.obtainedCount);

    /// <summary>
    /// 현재 보유 중인 명성 값 반환
    /// SaveData의 currencies에서 관리됨
    /// </summary>
    public static int GetFame(SaveData data) => data.currencies.fame;

    /// <summary>
    /// 완료한 업적 개수 반환
    /// achievements 리스트에서 Completed 상태인 것만 카운트
    /// </summary>
    public static int GetCompletedAchievementCount(SaveData data) =>
        data.achievements.Count(a => a.state == AchievementState.Completed);
}