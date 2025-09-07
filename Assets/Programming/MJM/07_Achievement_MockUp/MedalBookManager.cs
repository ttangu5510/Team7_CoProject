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

    void Start()
    {
        // 예시 데이터 로딩
        LoadMedals();

        prevButton.onClick.AddListener(PrevPage);
        nextButton.onClick.AddListener(NextPage);

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
        // 기존 아이템 제거
        foreach (Transform child in gridParent)
        {
            Destroy(child.gameObject);
        }

        // 새로운 아이템 생성
        int startIndex = currentPage * itemsPerPage;
        int endIndex = Mathf.Min(startIndex + itemsPerPage, allMedals.Count);

        for (int i = startIndex; i < endIndex; i++)
        {
            GameObject item = Instantiate(medalItemPrefab, gridParent);
            MedalItem medalItem = item.GetComponent<MedalItem>();
            medalItem.SetData(allMedals[i].icon, allMedals[i].title);
        }

        pageText.text = $"{currentPage + 1} / {Mathf.CeilToInt((float)allMedals.Count / itemsPerPage)}";

        prevButton.interactable = currentPage > 0;
        nextButton.interactable = endIndex < allMedals.Count;
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
        if ((currentPage + 1) * itemsPerPage < allMedals.Count)
        {
            currentPage++;
            UpdatePage();
        }
    }
}

public class MedalData
{
    public string title;
    public Sprite icon;
}
