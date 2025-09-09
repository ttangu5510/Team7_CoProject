using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndingResult
{
    public int totalScore;

    // 원천 데이터 스냅샷
    public EndingScoreData raw;

    // 메타 (원하면 실제 게임 시간/루프/회차로 교체)
    public string timestamp; // DateTime.UtcNow.ToString("s")
    public int loopYear;     // 예: 34 같은 값, 없으면 0
    public int olympicCount; // 예: 3 같은 값, 없으면 0
}
