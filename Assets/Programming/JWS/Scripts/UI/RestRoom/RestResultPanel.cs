using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace JWS
{
    // Content 밑에 깔릴 아이템
    public class RestResultPanel : MonoBehaviour
    {
        [Header("Header")]
        [SerializeField] private TextMeshProUGUI timelineTitleText;

        [Header("List")]
        [SerializeField] private Transform content;                 // Grid Layout Group 달린 Transform
        [SerializeField] private RestResultItem resultItemPrefab; // "Result Item" 프리팹(더미를 프리팹으로 빼서 할당)

        [Header("Footer")]
        [SerializeField] private TextMeshProUGUI defaultRecoverAmountText; // "피로도 {n} 회복"
        [SerializeField] private TextMeshProUGUI defaultRecoverCountText;  // "{n}명"

        [Header("Confirm")]
        [SerializeField] private Button confirmButton;

        // 외부에서 호출
        public void Open(string yyyy, string season, string week, IReadOnlyList<RestResultData> results, int recoverAmount, System.Action onClose = null)
        {
            // 타이틀
            if (timelineTitleText)
                timelineTitleText.text = $"{yyyy}년 {season} {week}주차 휴식 정보";

            // 리스트 클리어
            for (int i = content.childCount - 1; i >= 0; i--)
                Destroy(content.GetChild(i).gameObject);

            // 아이템 생성
            if (results != null && resultItemPrefab)
            {
                foreach (var r in results)
                {
                    var item = Instantiate(resultItemPrefab, content);
                    item.Bind(r.portrait, r.name, r.reducedFatigue);
                }
            }

            // 하단 요약
            if (defaultRecoverAmountText) defaultRecoverAmountText.text = $"피로도 {recoverAmount} 회복";
            if (defaultRecoverCountText)  defaultRecoverCountText.text  = $"{(results?.Count ?? 0)}명";

            // 확인 버튼
            if (confirmButton)
            {
                confirmButton.onClick.RemoveAllListeners();
                confirmButton.onClick.AddListener(() =>
                {
                    onClose?.Invoke();
                    gameObject.SetActive(false);
                });
            }

            gameObject.SetActive(true);
        }
    }

    // 결과 전달용 DTO
    public struct RestResultData
    {
        public Sprite portrait;
        public string name;
        public int reducedFatigue; // 실제로 깎인 값 (min(현재피로, 회복량))
    }
}
