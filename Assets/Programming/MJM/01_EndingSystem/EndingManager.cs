using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EndingManager : MonoBehaviour
{
    [SerializeField] private EndingScoreFormula formula;

    // 실제 게임에서 올림픽 3회차 종료 시점에 이 메서드를 호출
    [ContextMenu("Run Ending Score (Dummy/Live)")]
    public void RunEndingScore()
    {
        if (formula == null)
        {
            Debug.LogError("[Ending] Formula 미할당");
            return;
        }

        // 지금은 엑셀이 우선 -> SO의 dummy 또는 이후 실데이터 바인딩
        EndingScoreData data = formula.useDummy ? formula.dummy : FetchLiveData();

        var bd = EndingScorer.Compute(data, formula);

        Debug.Log(
            $"[ENDING]\n" +
            $"- 보유선수:{data.playerOwnedCount} -> {bd.ownedScore}\n" +
            $"- 은퇴선수:{data.playerRetiredCount} -> {bd.retiredScore}\n" +
            $"- 은퇴코치:{data.coachRetiredCount} -> {bd.coachRetiredScore}\n" +
            $"- 경기참여:{data.matchCount} -> {bd.matchScore}\n" +
            $"- 메달총점:{data.medalTotal} -> {bd.medalScore}\n" +
            $"- 시설강화:{data.facilityUpgrade} -> {bd.facilityScore}\n" +
            $"- 골드:{data.goldTotal} -> {bd.goldScore}\n" +
            $"- 업적:{data.achievementCount} -> {bd.achievementScore}\n" +
            $"- 명성:{data.reputationTotal} -> {bd.reputationScore}\n" +
            $"= 총합(소수): {bd.totalF} / 총합(정수 반올림): {bd.total}"
        );

        // TODO: 엔딩 UI 패널에 bd를 바인딩해 표시
        // endingPanel.Bind(data, bd);
    }

    // 나중에 실데이터 연결 지점
    private EndingScoreData FetchLiveData()
    {
        // GameManager/SaveSystem/DB 등에서 실제 값을 읽어와 채워 넣기
        return new EndingScoreData();
    }
}
