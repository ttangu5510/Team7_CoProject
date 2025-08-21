using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseUI : MonoBehaviour
{
    private Dictionary<string, GameObject> gameObjectDic;
    private Dictionary<string, Component> componentDic;
    
    private void Awake()
    {
        // BaseUI를 기준으로 모든 자식 게임오브젝트의 컴포넌트를 가져오기 (비활성화 오브젝트 포함 true 시)
        RectTransform[] transforms = GetComponentsInChildren<RectTransform>(true);
        gameObjectDic = new Dictionary<string, GameObject>(transforms.Length * 4);
        foreach (RectTransform child in transforms)
        {
            gameObjectDic.TryAdd(child.gameObject.name, child.gameObject);
        }
        Component[] components = GetComponentsInChildren<Component>(true);
        componentDic = new Dictionary<string, Component>(components.Length * 4);
        foreach (Component child in components)
        {
            componentDic.TryAdd($"{child.gameObject.name}_{child.GetType().Name}", child);
        }
    }

    public GameObject GetUI(in string name)
    {
        gameObjectDic.TryGetValue(name, out GameObject gameObject);
        return gameObject;
    }

    public T GetUI<T>(in string name) where T : Component
    {
        componentDic.TryGetValue($"{name}_{typeof(T).Name}", out Component component);
        return component as T;
    }
}
