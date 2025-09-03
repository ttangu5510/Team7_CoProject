using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PanelChangerTitle : MonoBehaviour
{
    [Header("처음에 열어둘 패널 (없으면 None)")]
    [SerializeField] private PanelId defaultOpen = PanelId.None;

    [Header("버튼-패널-제목 매핑 목록")]
    [SerializeField] private List<Entry> entries = new List<Entry>();

    [Header("제목 텍스트 (TMP_Text)")]
    [SerializeField] private TMP_Text titleText;

    // 현재 열린 패널
    public PanelId Current { get; private set; } = PanelId.None;

    [Serializable]
    public enum PanelId
    {
        None = 0,
        PanelA = 1,
        PanelB = 2,
        PanelC = 3,
        // 필요 시 추가
    }

    [Serializable]
    public class Entry
    {
        public PanelId id;
        public Button button;
        public GameObject panel;
        public string title; // 해당 패널에 보여줄 제목
    }

    private void Awake()
    {
        // 버튼 클릭 이벤트 연결
        foreach (var e in entries)
        {
            if (e?.button == null) continue;
            var capturedId = e.id; // 클로저 문제 방지
            e.button.onClick.AddListener(() => Open(capturedId));
        }

        // 초기 패널 열기
        if (defaultOpen == PanelId.None)
        {
            HideAll();
            Current = PanelId.None;
        }
        else
        {
            Open(defaultOpen);
        }
    }

    /// <summary>
    /// 지정한 패널만 열고 제목도 업데이트
    /// </summary>
    public void Open(PanelId target)
    {
        foreach (var e in entries)
        {
            if (e?.panel == null) continue;

            bool isTarget = (e.id == target);
            e.panel.SetActive(isTarget);

            if (isTarget && titleText != null)
            {
                titleText.text = e.title;
            }
        }

        Current = target;
    }

    /// <summary>
    /// 모든 패널 닫기
    /// </summary>
    public void HideAll()
    {
        foreach (var e in entries)
        {
            if (e?.panel != null)
                e.panel.SetActive(false);
        }
    }

    // 인스펙터 연결용 헬퍼 함수 (옵션)
    public void OpenPanelA() => Open(PanelId.PanelA);
    public void OpenPanelB() => Open(PanelId.PanelB);
    public void OpenPanelC() => Open(PanelId.PanelC);
}
