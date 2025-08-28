using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UniRx;
using UniRx.Triggers;
using Zenject;

namespace SHG
{
  /// <summary>
  /// 사용자의 터치에 따른 이벤트를 관리하는 역할
  /// UI를 표시하고 있는 동안에는 동작하지 않음
  /// </summary>
  public class TouchController : MonoBehaviour
  {
    [Inject]
    IFacilitiesController facilitiesController;

    /// <summary>
    /// 터치가 시작되었을 때 해당하는 fingerId와 position을 알려주는 기능
    /// </summary>
    public Subject<(int fingerId, Vector2 position)> OnTouchDown;
    /// <summary>
    /// 터치가 끝났을 때 해당하는 fingerId와 position을 알려주는 기능
    /// </summary>
    public Subject<(int fingerId, Vector2 position)> OnTouchUp;
    /// <summary>
    /// 터치한 상태에서 움직였을 때 움직임을 알려주는 기능
    /// </summary>
    public Subject<Vector2> OnTouchMove;
    Vector2 lastTouchPosition;
    int uiLayer;

    void Awake()
    {
      this.uiLayer = LayerMask.NameToLayer("UI");
      this.OnTouchUp = new Subject<(int fingerId, Vector2 position)>();
      this.OnTouchDown = new Subject<(int fingerId, Vector2 position)>();
      this.OnTouchMove = new Subject<Vector2>();
    }

    void Update()
    {
      if (Input.touchCount > 0 && !this.IsUiPresenting() && 
        !this.IsPointerOverUIObject()) {
        this.UpdateTouchDown();
        this.UpdateTouchUp();
        this.UpdateTouchMove();
      }
    }

    bool IsPointerOverUIObject() {
      PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
      eventDataCurrentPosition.position = Input.touches[0].position;
      List<RaycastResult> results = new List<RaycastResult>();
      EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
      foreach (var raycastResult in results) {
        if (raycastResult.gameObject.layer == this.uiLayer) {
          return (true);
        }
      }
      return (false);
    }

    void UpdateTouchDown()
    {
      if (Input.GetTouch(0).phase == TouchPhase.Began) {
        this.lastTouchPosition = Input.touches[0].position;
        this.OnTouchDown.OnNext((
            fingerId: Input.touches[0].fingerId,
            position: this.lastTouchPosition));
      }
    }

    void UpdateTouchUp()
    {
      if (Input.GetTouch(0).phase == TouchPhase.Ended) {
        this.OnTouchUp.OnNext((
            fingerId: Input.touches[0].fingerId,
            position: Input.touches[0].position));
      } 
    }

    void UpdateTouchMove()
    {
      var touch = Input.GetTouch(0);
      if (touch.phase == TouchPhase.Moved) {
        var position = Input.touches[0].position;
        var offset = position - this.lastTouchPosition;
        this.lastTouchPosition = position;
        this.OnTouchMove.OnNext(offset);
      }
    }

    // TODO: Check UI
    bool IsUiPresenting()
    {
      return (this.facilitiesController.Selected.Value != null);
    }

    void OnDestroy() {
      if (this.OnTouchDown != null) {
        this.OnTouchDown.OnCompleted();
      }
      if (this.OnTouchUp != null) {
        this.OnTouchUp.OnCompleted();
      }
    }
  }
}
