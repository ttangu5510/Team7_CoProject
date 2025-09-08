using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class TrophyBookManager : MonoBehaviour
{
    public GameObject trophyItemPrefab;     // 트로피 하나의 프리팹
    public Transform gridParent;            // 그리드 위치
    public Button prevButton;               // 이전 버튼
    public Button nextButton;               // 다음 버튼
    public Text pageText;                   // 현재 페이지 표시용

    private List<TrophyData> allTrophys = new List<TrophyData>();
    private int currentPage = 0;
    private int itemsPerPage = 9;

    void Awake()
    {
        prevButton.onClick.RemoveAllListeners();
        nextButton.onClick.RemoveAllListeners();

        prevButton.onClick.AddListener(PrevPage);
        nextButton.onClick.AddListener(NextPage);
    }

    void Start()
    {
        // 예시 데이터 로딩
        LoadTrophys();
        UpdatePage();
    }

    /// <summary>
    /// 가짜로 99개 복사해서 만드는 기능~
    /// </summary>
    void LoadTrophys()
    {
        allTrophys.Clear();
        for (int i = 0; i < 99; i++)
        {
            allTrophys.Add(new TrophyData { title = $"트로피 {i + 1}", icon = null });
        }
    }

    void UpdatePage()
    {
        int count = allTrophys?.Count ?? 0;
        int perPage = Mathf.Max(1, itemsPerPage); // 0 방어
        int totalPages = Mathf.Max(1, Mathf.CeilToInt(count / (float)perPage));

        currentPage = Mathf.Clamp(currentPage, 0, totalPages - 1);

        // 기존 아이템 제거 (역순)
        for (int i = gridParent.childCount - 1; i >= 0; i--)
            Destroy(gridParent.GetChild(i).gameObject);

        // 페이지 범위 계산
        int startIndex = currentPage * perPage;
        int endIndexExclusive = Mathf.Min(startIndex + perPage, count);

        // 새 아이템 생성
        for (int i = startIndex; i < endIndexExclusive; i++)
        {
            var go = Instantiate(trophyItemPrefab, gridParent);
            if (go.TryGetComponent<TrophyItem>(out var item))
                item.SetData(allTrophys[i].icon, allTrophys[i].title);
        }

        // 페이지 텍스트 (빈 목록이면 0/x 표기)
        int displayCurrent = (count == 0) ? 0 : (currentPage + 1);
        pageText.text = $"{displayCurrent} / {totalPages}";

        // 버튼 상태
        bool hasItems = count > 0;
        prevButton.interactable = hasItems && currentPage > 0;
        nextButton.interactable = hasItems && currentPage < totalPages - 1;
    }

    void PrevPage()
    {
        if (currentPage > 0)
        {
            currentPage--;
            UpdatePage();
        }
    }

    void NextPage()
    {
        int totalPages = Mathf.Max(1, Mathf.CeilToInt(allTrophys.Count / (float)itemsPerPage));
        if (currentPage < totalPages - 1)
        {
            currentPage++;
            UpdatePage();
        }
    }
}

public class TrophyData
{
    public string title;
    public Sprite icon;
}
