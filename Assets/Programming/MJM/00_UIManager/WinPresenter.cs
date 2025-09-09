using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinPresenter : BaseUI
{
    [SerializeField] Button nextButton => GetUI<Button>("NextButton"); //이렇게 람다식으로 작성해도 됨
    [SerializeField] private GameObject coinText;

    private void Start()
    {
        coinText = GetUI("CoinText");
    }
}
