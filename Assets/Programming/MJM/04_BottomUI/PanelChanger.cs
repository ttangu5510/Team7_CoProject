using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PanelChanger : MonoBehaviour
{
    [Header("처음에 열어둘 패널(없으면 None)")]
    [SerializeField] private PanelId defaultOpen = PanelId.None;

    [Header("버튼-패널 매핑 목록")]
    [SerializeField] private List<Entry> entries = new List<Entry>();

    // 현재 열린 패널
    public PanelId Current { get; private set; } = PanelId.None;

    [Serializable]
    public enum PanelId
    {
        None = 0,
        PanelA = 1,
        PanelB = 2,
        PanelC = 3,
        // 필요하면 더 추가해서 써도 됩니다.
    }

    [Serializable]
    public class Entry
    {
        public PanelId id;
        public Button button;
        public GameObject panel;
    }

    void Awake()
    {
        // 버튼에 클릭 이벤트 바인딩
        foreach (var e in entries)
        {
            if (e == null) continue;
            if (e.button != null)
            {
                var captured = e.id; // 클로저 캡쳐
                e.button.onClick.AddListener(() => Open(captured));
            }
        }

        // 초기 상태 정리
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
    /// 해당 패널만 열고 나머지는 닫음
    /// </summary>
    public void Open(PanelId target)
    {
        foreach (var e in entries)
        {
            if (e == null || e.panel == null) continue;
            bool active = (e.id == target);
            if (e.panel.activeSelf != active)
                e.panel.SetActive(active);
        }
        Current = target;
    }

    /// <summary>
    /// 전부 닫기
    /// </summary>
    public void HideAll()
    {
        foreach (var e in entries)
        {
            if (e?.panel) e.panel.SetActive(false);
        }
    }

    // 인스펙터에서 직접 연결하고 싶을 때 쓸 수 있는 헬퍼 (옵션)
    public void OpenPanelA() => Open(PanelId.PanelA);
    public void OpenPanelB() => Open(PanelId.PanelB);
    public void OpenPanelC() => Open(PanelId.PanelC);
}
