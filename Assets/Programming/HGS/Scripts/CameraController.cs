using System;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Zenject;
using EditorAttributes;

namespace SHG
{
  public class CameraController : MonoBehaviour
  {
    [Inject]
    TouchController touchController;
    [SerializeField] [Required]
    Transform cameraFollow;
    [SerializeField]
    float cameraMoveSpeed;

    void Start()
    {
      this.touchController.OnTouchMove
        .Subscribe(this.MoveCamera);
    }

    void MoveCamera(Vector2 offset)
    {
      var targetPos = this.cameraFollow.position - new Vector3(
          offset.x, 0, offset.y);
      if (offset.magnitude < 0.1f) {
        this.cameraFollow.position = targetPos;
      }
      else {
        this.cameraFollow.position = Vector3.MoveTowards(
          this.cameraFollow.position, targetPos, this.cameraMoveSpeed);
      }
    }
  }
}
