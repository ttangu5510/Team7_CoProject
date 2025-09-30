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

public class AddressableManager : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    [SerializeField] private TextMeshProUGUI progressText;
    [SerializeField] private Slider progressSlider;

    private AsyncOperationHandle downHandle;

    private async void Start()
    {
        // 1. Addressables 초기화
        await Addressables.InitializeAsync().Task;

        // 2. 카탈로그 업데이트 확인 및 갱신
        var checkHandle = Addressables.CheckForCatalogUpdates();
        var catalogs = await checkHandle.Task;
        if (catalogs != null && catalogs.Count > 0)
        {
            await Addressables.UpdateCatalogs(catalogs).Task;
        }

        // 3. 의존성 다운로드 (ALL 라벨 기준)
        downHandle = Addressables.DownloadDependenciesAsync("ALL", true);
        while (!downHandle.IsDone)
        {
            var status = downHandle.GetDownloadStatus();
            progressText.text = $"{status.Percent:P0}";
            progressSlider.value = (float)status.DownloadedBytes / status.TotalBytes;
            await UniTask.WaitForSeconds(0.1f); // 0.1초 마다 업데이트
        }

        panel.gameObject.SetActive(false);
    }

}
