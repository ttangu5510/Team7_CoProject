using UnityEngine;
using Zenject;
using JYL;

public class MedicalCenterOpener : MonoBehaviour
{
    [Inject] private DomAthService _ath;   // 읽기 전용으로만 사용
    [Inject] private ISaveManager _save;   // 읽기 전용으로만 사용

    [Header("Panels")]
    [SerializeField] private TreatmentRoomTabView treatmentRoomTabView; // Treatment Room Tab 오브젝트에 있는 컴포넌트
    [SerializeField] private InjuredListPanelView injuredListPanelView; // Injured List Panel 오브젝트에 있는 컴포넌트
    [SerializeField] private AthInfoPanelView    athInfoPanelView;     // Ath Info Panel 오브젝트에 있는 컴포넌트

    // 의료센터 UI 열 때 한 번 호출
    public void Open()
    {
        // 1) 슬롯 패널 (수용량/상태 계산은 내부에서 처리)
        treatmentRoomTabView?.Render();

        // 2) 부상 리스트
        injuredListPanelView?.Render();

        // 3) 상세 패널(선택 없으면 첫 부상 선수로 기본 표시)
        athInfoPanelView?.RenderDefaultIfNoneSelected();

        // 필요 시 패널 활성화
        if (!gameObject.activeSelf) gameObject.SetActive(true);
    }
}