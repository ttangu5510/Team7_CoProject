using TMPro;
using UnityEngine;
using UnityEngine.UI;
using JYL;

namespace JWS
{
    public class PlayerSlotView : MonoBehaviour
    {
        // [SerializeField] private Image portrait;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI stateText;
        
        private DomAthEntity bound;

        public void Set(DomAthEntity e)
        {
            bound = e;
            if (nameText)  nameText.text  = e.entityName;   // 필드명 프로젝트에 맞게
            if (stateText) stateText.text = "부상";                    
            // if (portrait) portrait.sprite = e.portraitSprite;       // 있으면 매핑
            gameObject.SetActive(true);
        }

        public void Clear()
        {
            bound = null;
            if (nameText)  nameText.text  = "-";
            if (stateText) stateText.text = "";
            // if (portrait) portrait.sprite = null;
            // gameObject.SetActive(true); // 빈 슬롯도 보이게 유지(숨기려면 false)
        }
    }
}