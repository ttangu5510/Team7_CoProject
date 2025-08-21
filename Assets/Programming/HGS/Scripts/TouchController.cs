using System;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

namespace SHG
{
  /// <summary>
  /// 사용자의 터치에 따른 이벤트를 관리하는 역할
  /// </summary>
  public class TouchController : ObservableTriggerBase
  {
    /// <summary>
    /// 터치가 시작되었을 때 해당하는 fingerId와 position을 알려주는 기능
    /// </summary>
    public Subject<(int fingerId, Vector2 position)> OnTouchDown;
    /// <summary>
    /// 터치가 끝났을 때 해당하는 fingerId와 position을 알려주는 기능
    /// </summary>
    public Subject<(int fingerId, Vector2 position)> OnTouchUp;

    void Awake()
    {
      this.OnTouchUp = new Subject<(int fingerId, Vector2 position)>();
      this.OnTouchDown = new Subject<(int fingerId, Vector2 position)>();
    }

    void Update()
    {
      if (Input.touchCount > 0) {
        this.CheckTouchDown();
        this.CheckTouchUp();
      }
    }

    void CheckTouchDown()
    {
      var touchStartedIndex = Array.FindIndex(
        Input.touches, touch => touch.phase == TouchPhase.Began);
      if (touchStartedIndex != -1) {
        this.OnTouchDown.OnNext((
            fingerId: Input.touches[touchStartedIndex].fingerId,
            position: Input.touches[touchStartedIndex].position));
      }
    }

    void CheckTouchUp()
    {
      var touchEndedIndex = Array.FindIndex(
        Input.touches, touch => touch.phase == TouchPhase.Ended);
      if (touchEndedIndex != -1) {
        this.OnTouchUp.OnNext((
            fingerId: Input.touches[touchEndedIndex].fingerId,
            position: Input.touches[touchEndedIndex].position));
      } 
    }

    protected override void RaiseOnCompletedOnDestroy() {
      if (this.OnTouchDown != null) {
        this.OnTouchDown.OnCompleted();
      }
      if (this.OnTouchUp != null) {
        this.OnTouchUp.OnCompleted();
      }
    }
  }
}
