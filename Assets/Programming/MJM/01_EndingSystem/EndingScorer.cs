using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndingScorer
{
    public struct Breakdown
    {
        public float ownedScore;
        public float retiredScore;
        public float coachRetiredScore;
        public float matchScore;
        public float medalScore;       // medalTotal * medalOuterMul
        public float facilityScore;
        public float goldScore;
        public float achievementScore;
        public float reputationScore;

        public float totalF;   // 부동소수 총합
        public int total;    // 반올림 정수 총합


        public string rank;        
        public string rankMessage;
    }


    // Total = (보유×1)+(은퇴×1)+(은퇴코치×1)+(경기×2)+((금×3+은×2+동×1)×3)+(시설×1)+(골드×0.1)+(업적×2)+(명성×1)
    public static Breakdown Compute(EndingScoreData d, EndingScoreFormula f)
    {
        Breakdown b;
        b.ownedScore = d.playerOwnedCount * f.ownedMul;
        b.retiredScore = d.playerRetiredCount * f.retiredMul;
        b.coachRetiredScore = d.coachRetiredCount * f.coachRetiredMul;
        b.matchScore = d.matchCount * f.matchMul;
        b.medalScore = d.medalTotal * f.medalOuterMul;   // medalTotal은 (금*3+은*2+동*1)의 합이라고 가정
        b.facilityScore = d.facilityUpgrade * f.facilityMul;
        b.goldScore = d.goldTotal * f.goldMul;
        b.achievementScore = d.achievementCount * f.achieveMul;
        b.reputationScore = d.reputationTotal * f.reputationMul;

        b.totalF = b.ownedScore + b.retiredScore + b.coachRetiredScore + b.matchScore +
                   b.medalScore + b.facilityScore + b.goldScore + b.achievementScore + b.reputationScore;

        b.total = Mathf.RoundToInt(b.totalF);






        string best = "C"; string msg = ""; int cut = int.MinValue;
        foreach (var r in f.rankThresholds)
        {
            if (b.total >= r.minScoreInclusive && r.minScoreInclusive > cut)
            {
                cut = r.minScoreInclusive;
                best = string.IsNullOrEmpty(r.rankName) ? best : r.rankName;
                msg = r.message;
            }
        }
        b.rank = best;
        b.rankMessage = msg;

        return b;
    }

}
