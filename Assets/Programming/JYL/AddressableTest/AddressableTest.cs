using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class AddressableTest : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI progressText;
    [SerializeField] private Image testImage;
    private Sprite testSprite;

    private AsyncOperationHandle downHandle;
    private void Awake()
    {
        downHandle = Addressables.DownloadDependenciesAsync("Image");
        progressText.text = downHandle.PercentComplete.ToString("P");
        downHandle.Completed += (AsyncOperationHandle handle) =>
        {
            Addressables.Release(handle);
        };
        
        testSprite = Addressables.LoadAssetAsync<Sprite>("1107").WaitForCompletion();
        testImage.sprite = testSprite;
    }
}
