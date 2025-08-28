using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UniRx;
using UniRx.Triggers;

namespace SJL
{
    public class PlayerListPanel : MonoBehaviour
    {
        [SerializeField] Image image;
        [SerializeField] TMP_Text text;
        [SerializeField] Button Button;

        [SerializeField] public GameObject playerInformationPanel;

        public void Awake()
        {
            Button.OnClickAsObservable()
                .Subscribe(_ => OnPlayerInformationPanel());
        }

        private void OnPlayerInformationPanel()
        {
            playerInformationPanel.SetActive(true);
        }
    }
}
