using UnityEngine;
using UnityEngine.UI;

public class EndingAnimationView : MonoBehaviour
{
    [SerializeField] GameObject videoRoot;  // VideoPlayer나 타임라인 넣는 영역
    [SerializeField] Button btnSkip;        // 스킵 버튼(탭 제스처 가능)

    public System.Action onFinishedOrSkip;

    void Awake()
    {
        if (btnSkip) btnSkip.onClick.AddListener(() => EndNow());
    }

    // 컷신이 끝났을 때 외부/이벤트로 호출
    public void EndNow()
    {
        onFinishedOrSkip?.Invoke();
    }
}
