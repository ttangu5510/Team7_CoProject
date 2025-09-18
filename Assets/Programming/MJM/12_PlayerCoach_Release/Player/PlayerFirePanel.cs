using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerFirePanel : MonoBehaviour
{
    [SerializeField] Image playerImage;
    [SerializeField] Button cancleButton;
    [SerializeField] Button confirmButton;

    private void Start()
    {
        cancleButton.onClick.AddListener(PlayerFireCancle);
        confirmButton.onClick.AddListener(PlayerFireConfirm);

    }

    private void PlayerFireCancle()
    {
        gameObject.SetActive(false);

        Debug.Log("취소햇당");
        //todo 취소 로직
    }

    private void PlayerFireConfirm()
    {
        gameObject.SetActive(false);

        Debug.Log("확인햇당");
        //todo 확인 로직
    }

}
