using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Text;

public class EndScorePopup : MonoBehaviour
{
    [Header("Header")]
    [SerializeField] TMP_Text txtTitle;     // "플레이 점수"
    [Header("List")]
    [SerializeField] TMP_Text txtList;      // 세부 항목 줄바꿈 텍스트
    [Header("Total")]
    [SerializeField] TMP_Text txtTotal;     // "총합 9,999점"
    [Header("Buttons")]
    [SerializeField] Button btnConfirm;     // 확인(=다음 단계)


    [SerializeField] TMP_Text txtRank;      // "등급 S" 같은 표시
    [SerializeField] TMP_Text txtRankMsg;   // 등급 설명

    public System.Action onConfirm;

    public void Bind(EndingScoreData raw, EndingScorer.Breakdown bd)
    {
        if (txtTitle) txtTitle.text = "플레이 점수";

        var sb = new StringBuilder(256);
        sb.AppendLine($"보유 선수 수      {raw.playerOwnedCount,6} 명");
        sb.AppendLine($"은퇴시킨 선수 수  {raw.playerRetiredCount,6} 명");
        sb.AppendLine($"은퇴시킨 코치 수  {raw.coachRetiredCount,6} 명");
        sb.AppendLine($"경기 참여 횟수    {raw.matchCount,6} 회");
        sb.AppendLine($"획득한 메달 수    {raw.medalTotal,6} 점(가중)");
        sb.AppendLine($"시설 강화 횟수    {raw.facilityUpgrade,6} 회");
        sb.AppendLine($"획득한 골드     {raw.goldTotal,10:N0} G");
        sb.AppendLine($"달성 업적 수      {raw.achievementCount,6} 개");
        sb.AppendLine($"달성 명성         {raw.reputationTotal,6} 점");
        if (txtList) txtList.text = sb.ToString();

        if (txtTotal) txtTotal.text = $"총합  {bd.total:N0} 점";


        if (txtTotal) txtTotal.text = $"총합  {bd.total:N0} 점";
        if (txtRank) txtRank.text = $"등급  {bd.rank}";
        if (txtRankMsg) txtRankMsg.text = bd.rankMessage;


    }

    void Awake()
    {
        if (btnConfirm) btnConfirm.onClick.AddListener(() => onConfirm?.Invoke());
    }
}
