using MMJ;
using UnityEngine;
using UnityEngine.UI;

namespace MMJ
{
    public class PlayerItem : MonoBehaviour
    {
        public Image playerImage;
        public Text playerNameText;
        public Sprite silhouetteSprite;  // 까만 실루엣 이미지

        public void SetData(PlayerData data)
        {
            if (data.isMet)
            {
                playerImage.sprite = data.image;
                playerNameText.text = data.playerName;
            }
            else
            {
                playerImage.sprite = silhouetteSprite;
                playerNameText.text = "???";
            }
        }
    }
}
