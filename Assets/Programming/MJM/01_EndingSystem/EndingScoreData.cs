using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndingScoreData
{
    public int playerOwnedCount;    // 보유 선수 수
    public int playerRetiredCount;  // 은퇴 선수 수
    public int coachRetiredCount;   // 은퇴 코치 수
    public int matchCount;          // 경기 참여 수 
    public int medalTotal;          // 메달 총점 (금*3 + 은*2 + 동*1 의 합)
    public int facilityUpgrade;     // 시설 업글 수
    public int goldTotal;           // 획득 골드
    public int achievementCount;    // 달성 업적 수
    public int reputationTotal;     // 달성 명성
}
