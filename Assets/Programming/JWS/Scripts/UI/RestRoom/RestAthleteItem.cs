using System;
using UniRx;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using JYL;

namespace JWS
{
    public class RestAthleteItem : MonoBehaviour
    {
         [Header("Root Button (아이템 전체)")]
        [SerializeField] private Button rootButton;           // 아이템 전체 클릭 → 상세보기

        [Header("Sub Buttons")]
        [SerializeField] private Button assignButton;         // 배치하기
        [SerializeField] private Button assignedButton;       // 배치됨 표시(토글/표시용)

        [Header("Texts & Image")]
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI fatigueText;
        [SerializeField] private Image profileImage;          // (옵션)

        public DomAthEntity RefAth => _ath;
        private DomAthEntity _ath;

        // 이벤트
        private readonly Subject<DomAthEntity> _onAssign = new();
        public IObservable<DomAthEntity> OnAssign => _onAssign;
        
        private readonly Subject<DomAthEntity> _onUnassign = new();
        public IObservable<DomAthEntity> OnUnassign => _onUnassign;


        private readonly Subject<DomAthEntity> _onOpenInfo = new();
        public IObservable<DomAthEntity> OnOpenInfo => _onOpenInfo;

        void Awake()
        {
            if (rootButton)
                rootButton.OnClickAsObservable()
                    .Subscribe(_ => { if (_ath != null) _onOpenInfo.OnNext(_ath); })
                    .AddTo(this);

            if (assignButton)
                assignButton.OnClickAsObservable()
                    .Subscribe(_ => { if (_ath != null) _onAssign.OnNext(_ath); })
                    .AddTo(this);

            if (assignedButton)
                assignedButton.OnClickAsObservable()
                    .Subscribe(_ => { if (_ath != null) _onUnassign.OnNext(_ath); }) // ← 해제 요청
                    .AddTo(this);
        }

        public void Bind(DomAthEntity ath, bool isAssigned)
        {
            _ath = ath;

            if (nameText)       nameText.text = $"{ath.entityName} ({ath.curAge.Value}세)";
            if (fatigueText) fatigueText.text = $"피로도 {ath.stats.fatigue}";

            // 프로필 이미지 있으면 여기서 설정
            // if (profileImage) profileImage.sprite = ...

            if (assignButton)   assignButton.gameObject.SetActive(!isAssigned);
            if (assignedButton) assignedButton.gameObject.SetActive(isAssigned);
        }

        public void SetAssigned(bool assigned)
        {
            if (assignButton)   assignButton.gameObject.SetActive(!assigned);
            if (assignedButton) assignedButton.gameObject.SetActive(assigned);
        }
        
        public void NudgeAssignButton()
        {
            var btn = assignButton ? assignButton.transform : transform;
            StartCoroutine(NudgeCo(btn));
        }

        private System.Collections.IEnumerator NudgeCo(Transform t)
        {
            var basePos = t.localPosition;
            float d = 6f, dur = 0.08f;
            for (int i = 0; i < 3; i++)
            {
                t.localPosition = basePos + Vector3.right * d; yield return new WaitForSeconds(dur);
                t.localPosition = basePos - Vector3.right * d; yield return new WaitForSeconds(dur);
            }
            t.localPosition = basePos;
        }
    }
}