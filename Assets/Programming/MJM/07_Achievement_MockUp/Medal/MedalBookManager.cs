using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MedalBookManager : MonoBehaviour
{
    public GameObject medalItemPrefab;      // 메달 하나의 프리팹
    public Transform gridParent;            // 그리드 위치
    public Button prevButton;               // 이전 버튼
    public Button nextButton;               // 다음 버튼
    public Text pageText;                   // 현재 페이지 표시용

    private List<MedalData> allMedals = new List<MedalData>();  // 전체 메달 데이터 리스트
    private int currentPage = 0;                                // 현재 페이지
    private int itemsPerPage = 9;                               // 한 페이지당 보여줄 아이템 수

    public MedalDetailPopup popupPrefab;
    public Transform popupRoot; // 없으면 Canvas


    private void Awake()
    {
        prevButton.onClick.RemoveAllListeners();
        nextButton.onClick.RemoveAllListeners();

        prevButton.onClick.AddListener(PrevPage);
        nextButton.onClick.AddListener(NextPage);
    }


    void Start()
    {
        // 예시 데이터 로딩
        LoadMedals();
        UpdatePage();
    }


    /// <summary>
    /// 가짜로 99개 복사해서 만드는 기능~
    /// </summary>
    void LoadMedals()
    {
        // 실제 데이터로 대체 가능
        for (int i = 0; i < 99; i++)
        {
            allMedals.Add(new MedalData { title = $"메달 {i + 1}", icon = null }); // 아이콘은 나중에 설정
        }
    }

    void UpdatePage()
    {
        int count = allMedals?.Count ?? 0;
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
            var go = Instantiate(medalItemPrefab, gridParent);
            if (go.TryGetComponent<MedalItem>(out var item))
                item.Bind(allMedals[i], OnClickMedal);
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
        int totalPages = Mathf.Max(1, Mathf.CeilToInt(allMedals.Count / (float)itemsPerPage));
        if (currentPage < totalPages - 1)
        {
            currentPage++;
            UpdatePage();
        }
    }

    void OnClickMedal(MedalData data)
    {
        var root = popupRoot ? popupRoot : transform.root;
        var popup = Instantiate(popupPrefab, root);
        popup.Open(data);
    }

}

