using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class EndingPanel : MonoBehaviour
{
    [Header("Texts")]
    [SerializeField] private TMP_Text txtTitle;
    [SerializeField] private TMP_Text txtTotal;
    [SerializeField] private TMP_Text txtBreakdown;

    [Header("Buttons")]
    [SerializeField] private Button btnContinue; // 엔딩 이후 이어하기
    [SerializeField] private Button btnRestart;  // 처음부터
    [SerializeField] private Button btnToTitle;  // 타이틀로

    private System.Action onContinue;
    private System.Action onRestart;
    private System.Action onToTitle;

    public void Bind(EndingScoreData raw, EndingScorer.Breakdown bd,
                     System.Action onContinue, System.Action onRestart, System.Action onToTitle)
    {
        this.onContinue = onContinue;
        this.onRestart = onRestart;
        this.onToTitle = onToTitle;

        if (txtTitle) txtTitle.text = "엔딩 스코어";
        if (txtTotal) txtTotal.text = $"총합: {bd.total}";

        if (txtBreakdown)
        {
            txtBreakdown.text =
                $"보유선수 x1: {raw.playerOwnedCount} → {bd.ownedScore}\n" +
                $"은퇴선수 x1: {raw.playerRetiredCount} → {bd.retiredScore}\n" +
                $"은퇴코치 x1: {raw.coachRetiredCount} → {bd.coachRetiredScore}\n" +
                $"경기참여 x2: {raw.matchCount} → {bd.matchScore}\n" +
                $"메달합 x3: {raw.medalTotal} → {bd.medalScore}\n" +
                $"시설강화 x1: {raw.facilityUpgrade} → {bd.facilityScore}\n" +
                $"골드 x0.1: {raw.goldTotal} → {bd.goldScore}\n" +
                $"업적 x2: {raw.achievementCount} → {bd.achievementScore}\n" +
                $"명성 x1: {raw.reputationTotal} → {bd.reputationScore}";
        }

        if (btnContinue) btnContinue.onClick.AddListener(() => this.onContinue?.Invoke());
        if (btnRestart) btnRestart.onClick.AddListener(() => this.onRestart?.Invoke());
        if (btnToTitle) btnToTitle.onClick.AddListener(() => this.onToTitle?.Invoke());
    }
}
