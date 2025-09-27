using UnityEngine;
using UniRx;
using Zenject;
using EditorAttributes;
using Cysharp.Threading.Tasks;
using Unity.VisualScripting;

namespace SHG
{
    public class CameraController : MonoBehaviour
    {
        const float CAMERA_PAN_TIME_THESHOLD = 0.1f;
        const float CAMERA_PAN_RATIO = 0.001f;
        const float CAMERA_PAN_DIST_THRESHOLD = 100f;

        [Inject] TouchController touchController;

        [SerializeField][Required] Transform cameraFollow;
        [SerializeField] Camera mainCamera; // 확대/축소용 카메라 참조

        [Header("Pan Settings")]
        [SerializeField] Vector2 cameraMoveVelocity = new Vector2(1f, 1f);
        [SerializeField][Range(1f, 10f)] float cameraSlowDownRatio = 1f;

        [Header("Zoom Settings")]
        [SerializeField] float zoomSpeed = 0.1f;
        [SerializeField] float minZoom = 5f;
        [SerializeField] float maxZoom = 20f;

        Vector2 cameraMoveFactor;
        Rigidbody cameraFollowRb;
        (float time, Vector2 offset) lastPanning;

        void Start()
        {
            this.cameraFollowRb = this.cameraFollow.GetOrAddComponent<Rigidbody>();
            this.cameraMoveFactor = this.cameraMoveVelocity * CAMERA_PAN_RATIO;

            // 패닝 구독
            this.touchController.OnPanning
              .Subscribe(this.TouchController_OnPanning)
              .AddTo(this);
        }

        void Update()
        {
#if UNITY_EDITOR || UNITY_STANDALONE
            // 에디터/PC 환경에서는 마우스 휠로 확대/축소 테스트
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            if (Mathf.Abs(scroll) > 0.01f)
            {
                Debug.Log($"[CameraController] Mouse Scroll Input: {scroll}");
                ApplyZoom(scroll * 500f); // 감도 보정
            }
#endif

            // 모바일 환경 핀치 줌
            if (Input.touchCount == 2)
            {
                Touch touch0 = Input.GetTouch(0);
                Touch touch1 = Input.GetTouch(1);

                Vector2 prevPos0 = touch0.position - touch0.deltaPosition;
                Vector2 prevPos1 = touch1.position - touch1.deltaPosition;

                float prevMagnitude = (prevPos0 - prevPos1).magnitude;
                float currentMagnitude = (touch0.position - touch1.position).magnitude;

                float difference = currentMagnitude - prevMagnitude;

                Debug.Log($"[CameraController] Pinch Zoom Difference: {difference}");
                ApplyZoom(difference);
            }
        }

        void FixedUpdate()
        {
            if (Time.time - this.lastPanning.time < CAMERA_PAN_TIME_THESHOLD &&
                this.lastPanning.offset != Vector2.zero)
            {
                var velocity = this.lastPanning.offset;
                this.cameraFollowRb.AddForce(
                    new Vector3(
                      -velocity.x * this.cameraMoveFactor.x,
                       0f,
                      -velocity.y * this.cameraMoveFactor.y),
                    ForceMode.VelocityChange);
            }
            else
            {
                this.cameraFollowRb.velocity *= (1f - this.cameraSlowDownRatio * Time.deltaTime);
            }
        }

        void TouchController_OnPanning(PanGesture panGesture)
        {
            if (panGesture.CurrentState == PanGesture.State.Ended)
            {
                this.lastPanning.offset = Vector2.zero;
                return;
            }

            if (panGesture.Positions.Count > 1)
            {
                int count = panGesture.TimeStamps.Count;
                float deltaTime = panGesture.TimeStamps[count - 1] - panGesture.TimeStamps[count - 2];
                Vector2 deltaPos = panGesture.Positions[count - 1] - panGesture.Positions[count - 2];
                Vector2 velocity = deltaPos / deltaTime;

                if (velocity.magnitude < CAMERA_PAN_DIST_THRESHOLD)
                {
                    this.lastPanning.offset = Vector2.zero;
                    return;
                }
                this.lastPanning = (panGesture.TimeStamps[count - 1], velocity);
            }
            else
            {
                this.cameraFollowRb.velocity = Vector3.zero;
            }
        }

        // 공통 줌 적용 함수
        void ApplyZoom(float delta)
        {
            if (!mainCamera) return;

            if (mainCamera.orthographic)
            {
                mainCamera.orthographicSize = Mathf.Clamp(
                  mainCamera.orthographicSize - delta * zoomSpeed,
                  minZoom, maxZoom);
            }
            else
            {
                mainCamera.fieldOfView = Mathf.Clamp(
                  mainCamera.fieldOfView - delta * zoomSpeed,
                  minZoom, maxZoom);
            }
        }
    }
}
