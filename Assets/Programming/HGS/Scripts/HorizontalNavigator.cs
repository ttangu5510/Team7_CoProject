using System;
using UnityEngine;
using UnityEngine.UI;
using LightScrollSnap;
using UniRx;
using UniRx.Triggers;
using Cysharp.Threading.Tasks;

namespace SHG
{
  [RequireComponent(typeof(ScrollSnap), typeof(ScrollRect))]
  public class HorizontalNavigator : MonoBehaviour
  {
    public ReactiveProperty<int> CurrentNavigationIndex { get; private set; } = new (0);
    const float TRANSITION_DURATION = 0.5f;
    [SerializeField]
    Transform container;

    ScrollRect scrollRect;
    ScrollSnap scrollSnap;
    Transform[] navigationScreens;
    IDisposable disposable;

    public void Pop()
    {
      if (this.CurrentNavigationIndex.Value <= 0) {
      #if UNITY_EDITOR
        throw (new ApplicationException($"{nameof(Pop)}: {nameof(this.CurrentNavigationIndex)} <= 0"));
      #else
        return ;
      #endif
      }
      this.CurrentNavigationIndex.Value -= 1;
    }

    public void Push()
    {
      if (this.CurrentNavigationIndex.Value >= this.navigationScreens.Length - 1) {
      #if UNITY_EDITOR
        throw (new ApplicationException($"{nameof(Push)}: {nameof(this.CurrentNavigationIndex)} >= {nameof(this.navigationScreens)}.Length - 1"));
      #else
        return ;
      #endif
      }
      this.CurrentNavigationIndex.Value += 1;
    }

    void Awake()
    {
      this.scrollRect = this.GetComponent<ScrollRect>();
      if (this.container == null) {
        this.container = this.GetComponentInChildren(
          typeof(HorizontalLayoutGroup)).transform;
      }
      this.scrollSnap = this.GetComponent<ScrollSnap>();
    }

    void Start()
    {
      int screenCount = this.container.childCount;
      this.navigationScreens = new Transform[screenCount];
      for (int i = 0; i < screenCount; ++i) {
        this.navigationScreens[i] = this.container.GetChild(i);
      }
      this.disposable = this.CurrentNavigationIndex.Subscribe(
        async index =>  {
        //FIXME: Animation not work
          this.scrollRect.enabled = true;
          //this.scrollSnap.SmoothScrollToItem(index);
          this.scrollSnap.ScrollToItem(index);
          await UniTask.WaitForSeconds(0.5f);
          this.scrollRect.enabled = false;
        });
      this.OnDestroyAsObservable()
        .Subscribe(_ => this.disposable?.Dispose());
    }
  }
}
