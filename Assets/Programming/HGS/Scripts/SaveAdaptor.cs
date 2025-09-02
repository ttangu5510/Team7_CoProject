using System;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UniRx.Triggers;
using Zenject;
using EditorAttributes;
using JYL;

namespace SHG
{
  public class SaveAdaptor : MonoBehaviour
  {
    [Inject]
    ISaveManager saveManager;
    [Inject]
    IResourceController resourceController;
    [Inject]
    ITimeFlowController timeFlowController;

    void Start()
    {
      this.timeFlowController.BeforeProgress += this.SaveProgress;
      this.OnDestroyAsObservable()
        .Subscribe(_ => this.timeFlowController.BeforeProgress -= this.SaveProgress);
      this.resourceController.Money
        .Subscribe(money => 
            this.saveManager.GetCurrentSave().currencies.gold = money)
        .AddTo(this);
      this.resourceController.Fame
        .Subscribe(fame => 
          this.saveManager.GetCurrentSave().currencies.fame = fame)
        .AddTo(this);
      this.resourceController.Coin
        .Subscribe(coin => 
          this.saveManager.GetCurrentSave().currencies.trainingCoin = coin)
        .AddTo(this);
      this.timeFlowController.WeekInYear
        .Subscribe(week =>
          this.saveManager.GetCurrentSave().time.week = week)
        .AddTo(this);
      this.timeFlowController.Year
        .Subscribe(year =>
          this.saveManager.GetCurrentSave().time.yearCycle = year)
        .AddTo(this);
    }
    
    [Button]
    void AutoSave()
    {
      this.saveManager.AutoSave();
    }

    [Button]
    void AutoLoad()
    {
      this.saveManager.AutoLoad();
    }

    [Button]
    void GetCurrentSave()
    {
      var currentSave = this.saveManager.GetCurrentSave();
      if (currentSave != null) {
        Debug.Log($"currentSave: {currentSave}");
      }
      else {
        Debug.LogError($"{nameof(currentSave)} is null");
      }
    }

    void SaveProgress()
    {
      this.saveManager.AutoSave();
    }
  }
}
