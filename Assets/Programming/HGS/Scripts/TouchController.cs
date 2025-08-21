using System;
using UnityEngine;
using UniRx;
using UniRx.Triggers;

namespace SHG
{
  public class TouchController : ObservableTriggerBase
  {
    public Subject<(int fingerId, Vector2 position)> OnTouchDown;
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
