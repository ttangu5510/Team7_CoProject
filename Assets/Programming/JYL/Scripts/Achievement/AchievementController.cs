using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using JWS;
using UniRx;
using UnityEngine;
using Zenject;

namespace JYL
{
    [Serializable]
    public class AchievementController
    {
        [Inject] private ISaveManager saveManager;
        public Achievement achieve { get; }
        private AchievementSave save;
        public ReactiveProperty<AchievementState> state { get; } = new();
        public ReactiveProperty<int> progress { get; } = new();
        
        private readonly CompositeDisposable disposables = new();

        public AchievementController(Achievement data, AchievementSave save)
        {
            achieve = data;
            this.save = save;
            if (save != null)
            {
                progress.Value = save.progress;
                state.Value =  save.state;
            }
            else
            {
                Debug.Log($"세이브가 null임_{data.ID}_{save == null}");
                progress.Value = 0;
            }
            
            progress
                .CombineLatest(state, (p, s) => (p, s))
                .Skip(1)
                .Where(x => x.s is AchievementState.Unlocked or AchievementState.Hidden)
                .Subscribe(x=>OnProgressChanged(x.p))
                .AddTo(disposables);

            state.Where(_ => state.Value is not AchievementState.Completed)
                .Skip(1)
                .Subscribe(OnStateChanged)
                .AddTo(disposables);
        }

        public void UnlockAchievement()
        {
            Debug.Log("업적 언락으로 들어옴");
            state.Value = AchievementState.Unlocked;
            OnProgressChanged(progress.Value);
        }

        private void OnStateChanged(AchievementState state)
        {
            switch (state)
            {
                // Locked 상태에서 Unlocked 상태로 변경하는 것은 Manager에서 구독 관리함.
                // 선행 업적의 상태가 Complete로 변경 시, Unlocked로 변경됨.
                case AchievementState.Unlocked:
                    OnProgressChanged(progress.Value); // 완료 가능한 상황인지 바로 체크함.
                    Debug.Log($"업적 언락상태로 전환 ");
                    break;
                case AchievementState.CanComplete:
                    Debug.Log("업적 완료가능상태로 전환");
                    break;
                case AchievementState.Completed:
                    Debug.Log("업적 완료상태로 전환");
                    // 상태 변화를 Manager쪽에서 참고한 다음, 완료 시 트로피 획득 처리 (추가 처리할 것은 없음).
                    break;
            }
            // 세이브의 상태도 변경
            save.state = state;
        }
        
        // progress 값이 변할 때 체크 함.
        private void OnProgressChanged(int progressValue)
        {
            // 변경된 값 SaveData에 저장
            save.progress = progressValue;
            
            // 언락됐거나, 히든일 때만 조건 체크함.
            if (state.Value is AchievementState.Unlocked or AchievementState.Hidden && progress.Value >= achieve.CompleteNumber)
            {
                state.Value = AchievementState.CanComplete;
                Debug.Log($"완료가능 업적이름: {achieve.AchName}");
            }
        }

        public void OnDestroy()
        {
            disposables.Dispose();
            disposables.Clear();
        }
    }
}