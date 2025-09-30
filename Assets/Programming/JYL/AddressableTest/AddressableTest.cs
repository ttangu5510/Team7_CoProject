using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class AddressableTest : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI progressText;
    // [SerializeField] private Image testImage;
    // private Sprite testSprite;

    private AsyncOperationHandle downHandle;

    private async void Start()
    {
        Addressables.InitializeAsync().WaitForCompletion();
        Addressables.CheckForCatalogUpdates().WaitForCompletion(); // 카탈로그 비교해서 수정사항들 비교해서 다운로드함.
        downHandle = Addressables.DownloadDependenciesAsync("ALL");
        while (!downHandle.IsDone)
        {
            progressText.text = $"{downHandle.GetDownloadStatus().Percent:P}";
            await UniTask.Yield();
        }
        
        gameObject.SetActive(false);
        
        // testSprite = Addressables.LoadAssetAsync<Sprite>("ImageAssets/character profile/ID순/1101.png").WaitForCompletion();
        // testImage.sprite = testSprite;
    }

}
