// using JYL;
// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.UI;
// using Zenject;
//
// namespace SJL
// {
//     public class CoachListPanel : MonoBehaviour
//     {
//         [Header("Buttons")]
//         [SerializeField] Button closeButton;
//         [SerializeField] private Button applyButton;
//         [SerializeField] private Button resetButton;
//         [Header("Set Content Transform")]
//         [SerializeField] private RectTransform popUpParent; // 선수 정보 팝업창의 부모
//         [SerializeField] Transform parentContent;   // 아이템들이 생성될 부모
//         [Header("Set Prefabs")]
//         [SerializeField] private AthleteTrainingItemUI athleteItem;
//
//         [Inject] private CoachService coachService;
//
//         private List<CoachEntity> list = new(); // 배치 가능한 전체 코치
//         
//         public void Awake()
//         {
//             applyButton.onClick.AddListener(OnClickApplyButton);
//             resetButton.onClick.AddListener(OnClickResetButton);
//             closeButton.onClick.AddListener(OnClickCloseButton);
//         }
//
//         public void OnClickApplyButton()
//         {
//             // TODO : 코치 배치 적용
//         }
//
//         public void OnClickResetButton()
//         {
//             // TODO : 코치 배치 리셋
//         }
//
//         public void OnClickCloseButton()
//         {
//             gameObject.SetActive(false);
//         }
//
//     }
// }