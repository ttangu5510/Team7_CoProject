// HiddenEndingCondition.cs
using UnityEngine;

public abstract class HiddenEndingCondition : ScriptableObject
{
    // 필요하면 EndingScoreData, 최종 점수 다 받는다
    public abstract bool IsMet(EndingScoreData data, EndingScorer.Breakdown bd);
}

// 예시: 특정 업적 수 이상 + S랭크 이상
[CreateAssetMenu(fileName = "Cond_AchieveAndRank", menuName = "Ending/Hidden/Cond - Achieve & Rank")]
public class Cond_AchieveAndRank : HiddenEndingCondition
{
    public int minAchievements = 5;
    public string requiredRank = "S"; // S 등

    public override bool IsMet(EndingScoreData data, EndingScorer.Breakdown bd)
    {
        if (data.achievementCount < minAchievements) return false;
        // 랭크 순위를 단순 비교할 경우: S>A>B>C 가정
        string order = "CSSSAB"; // 간단 예시: 비교 로직 깔끔히 하려면 Dictionary로 가중치를 주자
        int RankWeight(string r) => r == "S" ? 4 : r == "A" ? 3 : r == "B" ? 2 : 1;
        return RankWeight(bd.rank) >= RankWeight(requiredRank);
    }
}
