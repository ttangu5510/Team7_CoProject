using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Loding_Panel : MonoBehaviour
{
    
    [SerializeField] Button TestLodingBtn;
    [SerializeField] Slider lodingSlider;
    [SerializeField] TMP_Text guideText;

    [TextArea]
    [SerializeField]
    private string[] guides =
    {
        "히든엔딩이 있을지도? ㅋ",
        "맘스터치 한정 치킨버거는 ㅇㅈ 입니다.",
        "치킨은 굽네 고추 바사삭이 1황입니다.",
        "맥도날드가 버거 1황입니다."
    };
    private void OnEnable()
    {
        ShowRandomGuide();
    }

    private void ShowRandomGuide()
    {
        if (guides.Length == 0 || guideText == null) return;

        int randomIndex = Random.Range(0, guides.Length);
        guideText.text = guides[randomIndex];
    }
}
