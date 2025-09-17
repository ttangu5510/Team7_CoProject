using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.Rendering.HDROutputUtils;

public class Loader : MonoBehaviour, IPointerClickHandler
{
    public Image loadingAnimImg;    // 애니메이션 이미지
    public Sprite[] loadingSprites; // 애니메이션 스프라이트 배열
    public TextMeshProUGUI guideText;   // 가이드 텍스트
    public TextMeshProUGUI percentText; // 로딩 퍼센트 텍스트
    public Slider percentSlider;   // 로딩 바

    float timer = 0f;   // 타이머
    int spriteIdx = 0;  // 현재 스프라이트 인덱스

    AsyncOperation operation;
    float progress = 0f;    // 0~1 (현재 로딩 진행률)
    bool loading = true;    // 로딩 중 여부
    private int guideIndex = 0; // 현재 가이드 텍스트 인덱스
    private System.Random random = new System.Random(); // 랜덤 객체
    string scenename = "JYL_MainScene"; // 로드할 씬 이름

    public void Start()
    {
        // 가이트 텍스트 랜덤 설정
        guideIndex = random.Next(guideTexts.Length);
        guideText.text = guideTexts[guideIndex];  // 초기 가이드 텍스트 설정

        // 애니메이션 스프라이트 설정
        spriteIdx = random.Next(loadingSprites.Length);
        if (loadingSprites.Length > 0)
            loadingAnimImg.sprite = loadingSprites[spriteIdx];

        // 초기화
        percentSlider.value = 0f;   // 로딩 바 초기값
        percentText.text = "로딩 0%"; // 로딩 텍스트 초기값

    }


    public void Update()
    {
        if (loading)
        {
            //진행률이 5초 동안 100%로 상승 (실제 사용시에는 씬 로딩 등과 연동)
            if (progress < 1f)
                progress += Time.deltaTime / 5f; // 2초간 0→1

            // 로딩 바, 텍스트 업데이트
            percentSlider.value = progress;
            percentText.text = $"로딩 {(int)(progress * 100)}%";

            // 1초마다 애니메이션 스프라이트 변경
            timer += Time.deltaTime;

            // 로딩 완료 처리 예시
            if (progress >= 1f)
            {
                loading = false;
                percentSlider.value = 1f;
                percentText.text = "로딩 100%";

                // 실제 씬 활성화
                SceneActivation(scenename);
            }
        }
    }

    // guideText를 클릭했을 때(터치 포함)
    public void OnPointerClick(PointerEventData eventData)
    {
        // 클릭된 게 guideText 오브젝트일 때만 변경
        if (eventData.pointerCurrentRaycast.gameObject == guideText.gameObject)
        {
            guideIndex = random.Next(guideTexts.Length);
            guideText.text = guideTexts[guideIndex];
        }
    }

    public void SceneActivation(string scenename)
    {
        operation = SceneManager.LoadSceneAsync(scenename);
        operation.allowSceneActivation = true;
    }

    private string[] guideTexts = {
         "<size=42><b>권 지혁 TMI</b></size>\n\n 권 지혁은 조그마한 토끼 인형을 안고 잡니다.",
         "<size=42><b>송 우진 TMI</b></size>\n\n 송 우진은 어렸을 때 할아버지라고 놀림을 받아\n 자신의 흰머리를 싫어합니다.",
         "<size=42><b>강 윤호 TMI</b></size>\n\n 강 윤호는 언제나 여유로워 보이지만,\n 뒤에선 누구보다 노력하는 노력파입니다.",
         "<size=42><b>안 민우 TMI</b></size>\n\n 안 민우의 머리는 원래 검은 색이었으나\n 바다를 좋아하여 파란색으로 염색한 것입니다.",
         "<size=42><b>배 정훈 TMI</b></size>\n\n 배 정훈은 깔 맞춤을 좋아하여 복장과 색을 맞추기 위해\n 머리를 노란색으로 염색했습니다.",
         "<size=42><b>송 태석 TMI</b></size>\n\n 송 태석은 부모에게 버림받은 후 오랜 길거리 생활로 인해\n 숙소보다 길거리에서 자는 것을 좋아합니다.",
         "<size=42><b>이 성재 TMI</b></size>\n\n 이 성재는 송 태석의 비협조적인 모습에 호기심을 느껴\n 그의 과거를 몰래 알아본 적이 있습니다.",
         "<size=42><b>성 진우 TMI</b></size>\n\n 성 우진은 매일 훈련과 게임을 병행하는\n 최 민서의 일정을 보고 경악한 적이 있습니다.",
         "<size=42><b>박 지안 TMI</b></size>\n\n 박 지안은 답답한 것을 싫어하여\n 몰래 숙소를 나가려다 들킨 적이 있습니다.",
         "<size=42><b>정 수연 TMI</b></size>\n\n 정 수연은 자신이 혼혈이라는 것을\n 완벽하게 숨겼다고 생각합니다.",
         "<size=42><b>장 지민 TMI</b></size>\n\n 장 지민의 활기찬 성격은 훈련에 지친 동료들에게\n 큰 힘이 되곤 합니다.",
         "<size=42><b>한 소율 TMI</b></size>\n\n 한 소율은 어렸을 때 봤던 동계 올림픽에서 감명을 받고\n 국가대표를 목표로 달려가고 있습니다.",
         "<size=42><b>오 유진 TMI</b></size>\n\n 오 유진의 오른팔 안쪽에는 작은 점이 있습니다.",
         "<size=42><b>조 민아 TMI</b></size>\n\n 조 민아는 사진을 찍을 때 본능적으로\n 두 팔을 앞으로 뻗는 습관이 있습니다.",
         "<size=42><b>송 하늘 TMI</b></size>\n\n 송 하늘의 옷장에는 노란색 옷과 갈색 바지로 가득 차 있습니다.",
         "<size=42><b>김 서린 TMI</b></size>\n\n 김 서린은 자신의 열정을 표현하기 위해\n 언제나 붉은 옷을 입고 다닙니다.",
         "<size=42><b>이 지안 TMI</b></size>\n\n 이 지안은 남과 어울리지 못하고 모든 사람을 경계하기 때문에\n 언제나 눈을 찌푸린 채 검은 모자와 마스크를 쓰고 있습니다.",
         "<size=42><b>박 하늘 TMI</b></size>\n\n 박 하늘은 자신을 경계하는 이 지안에게 흥미를 느껴\n 언제나 그녀가 보이면 먼저 다가가고 있습니다.",
         "<size=42><b>최 민서 TMI</b></size>\n\n 최 민서은 게임만 하는 자신이 왜 국가대표로 뽑혔는지\n 이해하지 못하고 있습니다.",
         "<size=42><b>정 서연 TMI</b></size>\n\n 정 서연은 재력가로서가 아닌 국가대표로서\n 모든 사람에게 인정받기를 원합니다.",
         "<size=42><b>훈련 센터 팁</b></size>\n\n 훈련 센터에서 훈련 루틴에 따라 상승하는 능력치가 달라집니다.",
         "<size=42><b>스카우터 센터 팁</b></size>\n\n 스카우터 센터에 선수가 마음에 들지 않으면\n 다시 선수를 갱신하여 선수를 뽑을 수 있습니다.",
         "<size=42><b>경기 팁</b></size>\n\n 경기를 우승하기 위해서는 특정 스택을 상승시켜야 합니다!",
         "<size=42><b>경기 일정 팁</b></size>\n\n 자율 경기는 경기 일정을 통해 참여 및 비참여를 선택할 수 있습니다.",
         "<size=42><b>업그레이드 팁</b></size>\n\n 특정 단계부터는 골드뿐만 아니라 명성이 필요합니다.",
         "<size=42><b>의료 센터 팁</b></size>\n\n 의료 센터에 부상에 걸린 선수를 배치하면\n 2 턴 후 자동으로 부상을 회복합니다.",
         "<size=42><b>휴게실 팁</b></size>\n\n 휴게실에서 휴식을 진행하면 턴을 소모시키고\n 피로도를 회복할 수 있습니다.",
         "<size=42><b>피로도 팁</b></size>\n\n 피로도가 일정 수치를 넘으면 훈련 실패 확률이 증가합니다.",
         "<size=42><b>퀘스트 팁</b></size>\n\n 퀘스트를 클리어하면 골드와 명성 획득할 수 있습니다.",
         "<size=42><b>업적 팁</b></size>\n\n 모든 업적을 클리어하면 특별한 보상을 받을 수 있습니다.",
    };


}
