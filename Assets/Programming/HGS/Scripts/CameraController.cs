using UnityEngine;
using UniRx;
using Zenject;
using EditorAttributes;
using Cysharp.Threading.Tasks;

namespace SHG
{
  public class CameraController : MonoBehaviour {
    const float CAMERA_PAN_TIME_THESHOLD = 0.1f;
    const float CAMERA_PAN_RATIO = 0.001f;
    const float CAMERA_PAN_DIST_THRESHOLD = 100f;
    [Inject]
    TouchController touchController;
    [SerializeField][Required]
    Transform cameraFollow;
    [SerializeField] 
    Vector2 cameraMoveVelocity = new Vector2(1f, 1f);
    [SerializeField] [Range(1f, 10f)]
    float cameraSlowDownRatio = 1f;
    Vector2 cameraMoveFactor;
    Rigidbody cameraFollowRb;
    (float time, Vector2 offset) lastPanning;

    void Start() {
      this.cameraFollowRb = this.cameraFollow.GetComponent<Rigidbody>();
      this.cameraMoveFactor = this.cameraMoveVelocity * CAMERA_PAN_RATIO;
      this.touchController.OnPanning.Subscribe(this.TouchController_OnPanning)
      .AddTo(this);
    }

    void FixedUpdate() {
      if (Time.time - this.lastPanning.time < CAMERA_PAN_TIME_THESHOLD && 
      this.lastPanning.offset != Vector2.zero) {
        var velocity = this.lastPanning.offset;
        this.cameraFollowRb.AddForce(
            new Vector3(
              -velocity.x * this.cameraMoveFactor.x,
             0f,
              -velocity.y * this.cameraMoveFactor.y),
            ForceMode.VelocityChange);
      } else {
          this.cameraFollowRb.velocity *= (1f - this.cameraSlowDownRatio * Time.deltaTime);
      }
    }

    void TouchController_OnPanning(PanGesture panGesture) {
      if (panGesture.CurrentState == PanGesture.State.Ended) {
          this.lastPanning.offset = Vector2.zero;
        return;
      }
      if (panGesture.Positions.Count > 1) {
        int count = panGesture.TimeStamps.Count;
        float deltaTime = panGesture.TimeStamps[count - 1] - panGesture.TimeStamps[count - 2];
        Vector2 deltaPos = panGesture.Positions[count - 1] - panGesture.Positions[count - 2];
        Vector2 velocity = deltaPos / deltaTime;
        if (velocity.magnitude < CAMERA_PAN_DIST_THRESHOLD) {
          this.lastPanning.offset = Vector2.zero;
          return ;
        }
        this.lastPanning = (panGesture.TimeStamps[count - 1], velocity);
      } else {
        this.cameraFollowRb.velocity = Vector3.zero;
      }
    }
  }
}
