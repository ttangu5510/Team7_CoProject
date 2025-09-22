using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using JWS;
using UniRx;
using UnityEngine;
using Zenject;

namespace JYL
{
    public class AchievementController
    {
        private Achievement achieve;
        private AchievementSave save;
        public ReactiveProperty<AchievementState> state { get; } = new();
        public ReactiveProperty<float> progress { get; } = new();
        
        private readonly CompositeDisposable disposables = new();

        public AchievementController(Achievement data, AchievementSave save)
        {
            achieve = data;
            this.save = save;
            progress.Value = save.progress;
            state.Value =  save.state;
            
            progress
                .CombineLatest(state, (p, s) => (p, s))
                .Where(x => x.s is AchievementState.Unlocked or AchievementState.Hidden)
                .Subscribe(x=>OnProgressChanged(x.p))
                .AddTo(disposables);

            state.Where(_ => state.Value is not AchievementState.Completed)
                .Subscribe(OnStateChanged)
                .AddTo(disposables);
        }

        private void OnStateChanged(AchievementState state)
        {
            switch (state)
            {
                // Locked 상태에서 Unlocked 상태로 변경하는 것은 Manager에서 구독 관리함.
                // 선행 업적의 상태가 Complete로 변경 시, Unlocked로 변경됨.
                case AchievementState.Unlocked:
                    OnProgressChanged(progress.Value); // 완료 가능한 상황인지 바로 체크함.
                    break;
                case AchievementState.CanComplete:
                    // TODO : 이 타이밍에 완료 알림 토스트 UI 띄움
                    break;
                case AchievementState.Completed:
                    // 상태 변화를 Manager쪽에서 참고한 다음, 완료 시 트로피 획득 처리 (추가 처리할 것은 없음).
                    break;
            }
            // 세이브의 상태도 변경
            save.state = state;
        }
        
        private void OnProgressChanged(float progressValue)
        {
            // 변경된 값 SaveData에 저장
            save.progress = progressValue;
            
            if (state.Value is AchievementState.Unlocked or AchievementState.Hidden && progress.Value >= achieve.CompleteNumber)
            {
                state.Value = AchievementState.CanComplete;
            }
        }
    }
}