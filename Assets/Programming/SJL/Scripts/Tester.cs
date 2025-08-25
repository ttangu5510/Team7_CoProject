using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

namespace SJL
{
    public class Tester : MonoBehaviour
    {
        [SerializeField] Rigidbody2D rigid;
        [SerializeField] bool isGeound;

        [SerializeField] Button button;

        private void Awake()
        {
            button.OnPointerEnterAsObservable() // OnPointerEnterAsObservable() : 포인터가 버튼 위에 올라갔을 때 이벤트를 Observable로 변환
                .Subscribe(_ => Debug.Log("버튼 위에 포인터 올라감")); // .Sebscribe() : 동작에 대한 정의

            button.OnClickAsObservable() // OnClickAsObservable() : 버튼 클릭 이벤트를 Observable로 변환
                .Subscribe(_ => Debug.Log("버튼 클릭됨")); // .Sebscribe() : 동작에 대한 정의

            gameObject.UpdateAsObservable() // UpdateAsObservable() : Update() 함수를 Observable로 변환
                .Where(x => Input.GetKeyDown(KeyCode.Space))    // .Where() : 조건에 대한 정의
                .Where(x => isGeound == true)    // .Where() : 조건에 대한 정의
                .Subscribe(x => rigid.AddForce(Vector3.up * 10, ForceMode2D.Impulse));    // .Sebscribe() : 동작에 대한 정의

            //이동 -> 좋은 방법은 아님
            gameObject.UpdateAsObservable()
                .Where(x => Input.GetAxis("Horizontal") != 0)
                .Subscribe(x => transform.Translate(Vector3.right * Input.GetAxis("Horizontal") * Time.deltaTime));

            //gameObject.UpdateAsObservable()
            //    .Where(x => Input.GetAxis("Vertical") != 0)
            //    .Subscribe(x => transform.Translate(Vector3.up * Input.GetAxis("Vertical")));

            //땅 착지
            gameObject.OnCollisionEnter2DAsObservable() // OnCollisionEnter2DAsObservable() : OnCollisionEnter2D() 함수를 Observable로 변환
                .Where(collision => collision.collider.tag == "Ground")
                .Subscribe(x => isGeound = true); // .Sebscribe() : 동작에 대한 정의

            //gameObject.OnCollisionEnter2DAsObservable() // OnCollisionEnter2DAsObservable() : OnCollisionEnter2D() 함수를 Observable로 변환
            //    .Where(x => x.gameObject.CompareTag("Ground")) // .Where() : 조건에 대한 정의
            //    .Subscribe(x => isGeound = true); // .Sebscribe() : 동작에 대한 정의

            //땅 떨어짐
            gameObject.OnCollisionExit2DAsObservable() // OnCollisionExit2DAsObservable() : OnCollisionExit2D() 함수를 Observable로 변환
                .Where(collision => collision.collider.tag == "Ground")
                .Subscribe(x => isGeound = false); // .Sebscribe() : 동작에 대한 정의

            

        }

        
    }
}
