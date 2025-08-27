using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "EndingScoreFormula", menuName = "Ending/Score Formula")]
public class EndingScoreFormula : ScriptableObject
{
    [Header("엔딩 점수 가중치")]
    public float ownedMul = 1f;         // 보유선수 ×1
    public float retiredMul = 1f;       // 은퇴선수 ×1
    public float coachRetiredMul = 1f;  // 은퇴코치 ×1
    public float matchMul = 2f;         // 경기참여 ×2
    public float medalOuterMul = 3f;    // (금×3+은×2+동×1) × 3  ← medalTotal은 (금*3+은*2+동*1)의 합이라고 가정
    public float facilityMul = 1f;      // 시설강화 ×1
    public float goldMul = 0.1f;        // 골드 ×0.1
    public float achieveMul = 2f;       // 업적 ×2
    public float reputationMul = 1f;    // 명성 ×1




    [Header("임시 입력(실데이터 없을 때 사용)")]
    public bool useDummy = true;
    public EndingScoreData dummy = new EndingScoreData
    {
        playerOwnedCount = 8,
        playerRetiredCount = 2,
        coachRetiredCount = 1,
        matchCount = 24,
        medalTotal = 19,   // 예: 금3(=9) + 은4(=8) + 동2(=2) -> 합 19
        facilityUpgrade = 7,
        goldTotal = 35000,
        achievementCount = 4,
        reputationTotal = 120
    };




    [System.Serializable]
    public struct EndingRankDef
    {
        public string rankName;          // "C","B","A","S","SS" 등
        public int minScoreInclusive; // 이 점수 이상이면 해당 랭크
        [TextArea] public string message;
    }

    [Header("랭크 컷(점수별 등급)")]
    public List<EndingRankDef> rankThresholds = new List<EndingRankDef>()
{
    new EndingRankDef{ rankName="C",  minScoreInclusive=0,    message="Normal" },
    new EndingRankDef{ rankName="B",  minScoreInclusive=200,  message="Great" },
    new EndingRankDef{ rankName="A",  minScoreInclusive=400,  message="League of Legend" },
    new EndingRankDef{ rankName="S",  minScoreInclusive=700,  message="Mythic" },
   
};

}
