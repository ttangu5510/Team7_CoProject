using System;
using System.Collections.Generic;
using UnityEngine;

/// 전체 세이브 데이터 루트
[Serializable] public class SaveData
{
    public int version = 1;

    // 유저
    public string userId;                     // 유저 계정 고유 ID (예: "UID 72819210")

    // 시간
    public TimeStamp time = new();            // 인게임 시간 + 현실 플레이 타임

    // 재화
    public Currencies currencies = new();     // 골드, 명성, 특훈 코인

    // 건물
    public List<BuildingState> buildings = new(); // 건물 상태 목록

    // 선수
    public Roster roster = new();             // 보유/은퇴/퇴출 목록
    public List<PlayerState> playerStates = new(); // 개별 선수 현재 상태(능력치/나이/성별/부상 등)

    // 코치
    public List<CoachState> coaches = new();  // 코치 보유 + 영입 내역

    // 상대 선수(스카우팅/대전 등으로 파악한 정보)
    public List<OpponentRecord> opponentRecords = new();

    // 퀘스트
    public List<QuestState> quests = new();   // 전체 목록 + 진행/클리어 여부

    // 업적
    public List<AchievementState> achievements = new(); // 전체 목록 + 진행/달성

    // 도감
    public List<EncyclopediaState> encyclopedia = new(); // 전체 목록 + 트로피/메달 보유 여부
}

/* ========================= 시간/재화 ========================= */

[Serializable] public class TimeStamp
{
    // 인게임 시간 - @년차 @계절 @주차
    public int yearCycle;     // 년차
    public Season season;     // 계절
    public int week;          // 주차(1-based)

    // 현실 플레이 타임 - XX시간 XX분 (분 단위로 저장하고 표시할 때 변환 추천)
    public int realPlayMinutesTotal; // 누적 플레이 분
    // 선택: 마지막 저장 UTC ISO (로그/백업용)
    public string lastSaveUtcIso;
}

public enum Season { Spring, Summer, Autumn, Winter }

[Serializable] public class Currencies
{
    public int gold;
    public int fame;
    public int trainingCoin; // 특훈 코인
}

/* ========================= 건물 ========================= */

[Serializable] public class BuildingState
{
    public string buildingId;                 // 마스터 CSV의 건물 ID
    public bool isUnlocked;                   // 해방 여부
    public int level;                         // 건물 레벨

    public List<string> assignedPlayerIds = new(); // 건물에 배치된 선수
    public List<string> scoutedPlayerIds = new();  // 스카우트 건물에 등장한 선수
    public List<string> assignedCoachIds = new();  // 코치(리세마라 방지)
}

/* ========================= 선수 ========================= */

[Serializable] public class Roster
{
    public List<string> owned = new();        // 획득한 선수 ID
    public List<string> retired = new();      // 은퇴 선수 ID
    public List<string> dismissed = new();    // 퇴출 선수 ID
}

// 선수 개별 상태(세이브용) — 마스터 CSV(기본능력치) + 현재 상태(세이브) 분리 권장
[Serializable] public class PlayerState
{
    public string id;             // 선수 ID (마스터 CSV 키)
    public string sex;            // "M"/"F" 등 (필요 없으면 삭제)
    public int age;               // 현재 나이
    public bool isInjured;         // 부상 상태
    public int fatigue;       // 피로도

}

/* ========================= 코치 ========================= */

[Serializable] public class CoachState
{
    public string id;             // 코치 ID
    public string hiredDate;  // 영입 날짜(UTC ISO)
    public string hiredRoute;          // 영입 경로(예: "Scout", "Quest", "Shop")
}

/* ========================= 상대 선수 ========================= */

[Serializable] public class OpponentRecord
{
    public string country;        // 국적 (예: "KR", "JP")
    public string teamName;    // 소속
    public List<string> playerIds = new(); // 파악된 선수 ID 목록(마스터 참조)
}

/* ========================= 퀘스트 ========================= */

[Serializable] public class QuestState
{
    public string id;             // 퀘스트 ID(전체 목록 포함)
    public QuestProgress progress;
}

public enum QuestProgress
{
    NotStarted,   // 진행 전
    InProgress,   // 진행 중
    Completed     // 클리어
}

/* ========================= 업적 ========================= */

[Serializable] public class AchievementState
{
    public string id;             // 업적 ID(전체 목록 포함)
    public AchievementProgress progress;
}

public enum AchievementProgress
{
    Locked,        // 미진행/잠금
    InProgress,    // 진행 중
    Unlocked       // 달성
}

/* ========================= 도감 ========================= */

[Serializable] public class EncyclopediaState
{
    public string id;     // 도감 항목 ID(전체 목록 포함)
    public int obtainedCount;     // 누적 획득 개수
}