using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UniRx;
using UniRx.Triggers;

namespace SJL
{
    public class PlayerListInformation : MonoBehaviour
    {
        [SerializeField] Button Button;

        //[SerializeField] public GameObject Panel;

        public void Awake()
        {
            Button.OnClickAsObservable()
                .Subscribe(_ => gameObject.SetActive(false));
        }

    }
}
