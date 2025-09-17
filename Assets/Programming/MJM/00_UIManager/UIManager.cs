using System;
using System.Collections.Generic;
using JYL;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System.Collections;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class UIManager : MonoBehaviour, IUiManager
{
    public static bool IsUIOpen { get; set; }   // UI on/off 상태

    // Rx 공개
    public static readonly BoolReactiveProperty IsUIOpenRx = new BoolReactiveProperty(false);

    public static HashSet<string> isUIOpen { get; private set; }

   // private static UIManager instance;
   // public static UIManager Instance => instance;

    [Header("Auto-Bind Roots")]
    [SerializeField] private Transform panelsRoot;                       // 패널 루트
    [SerializeField] private Transform[] popupRoots = new Transform[8];  // 팝업 하위 루트(0~7)
    [SerializeField] private Canvas[] canvasScopes;                      // 자동 버튼 바인딩 스코프

    // ===== 문자열 키 기반 패널 관리 =====
    private readonly Dictionary<string, GameObject> panels = new(); // key: normalized name
    private string currentPanelKey; // null = 열려있지 않음

    // ===== 팝업 스택 =====
    private readonly Stack<GameObject> popupStack = new();

    // ===== 팝업 프리팹 캐시 =====
    private readonly Dictionary<string, GameObject> popupPrefabCache = new();

    [Header("Toast")]
    [SerializeField] private Transform toastRoot;
    [SerializeField] private GameObject toastPrefab;
    [SerializeField] private int maxToasts = 3;
    [SerializeField] private float toastLife = 1.8f;
    private readonly Queue<GameObject> activeToasts = new();

    // 키 정규화 유틸
    private const string PANEL_PREFIX = "Panel.";
    private const string BUTTON_PREFIX = "Btn.";

    // 팝업 프리팹 로딩 규칙
    const string POPUP_PREFIX = "Popup.";
    const string POPUP_FOLDER = "Popups/"; // Resources/Popups/Popup.<Key>.prefab

    [Header("Panel Init Exceptions")]
    [Tooltip("기능이 신기해서 써봄 ㅋㅋ")]
    [SerializeField]
    private List<string> ignoreInitPanelKeys = new();


    private void Awake()
    {
        // 싱글톤 보장 & 파괴 금지
       // if (instance == null) { instance = this; DontDestroyOnLoad(gameObject); }
       // else { Destroy(gameObject); return; }

        isUIOpen = new();
        // 초기 바인딩
        AutoBindPanels();
        AutoBindButtons();

        // 시작 상태 초기화
        // foreach (var go in panels.Values) go?.SetActive(false);
        // currentPanelKey = null;

        foreach (var kv in panels)
        {
            string key = kv.Key;
            GameObject go = kv.Value;

            if (ignoreInitPanelKeys.Exists(x => NormalizeKey(x) == key))
            {
                // Debug.Log($"[UIManager] 패널 예외 적용됨: {key}");
                continue;
            }
            // Debug.Log($"[UIManager] 패널 끔: {key}");
            go?.SetActive(false);
        }

        UpdateUIState();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            HandleBack();
    }

    #region 자동바인딩
    private void AutoBindPanels()
    {
        panels.Clear();

        if (!panelsRoot)
        {
            var found = GameObject.Find("Panels");
            if (found) panelsRoot = found.transform;
        }
        if (!panelsRoot)
        {
            Debug.LogWarning("[UIManager] 'Panels' 루트를 찾을 수 없습니다.");
            return;
        }

        foreach (Transform t in panelsRoot)
        {
            string n = t.name;
            if (!n.StartsWith(PANEL_PREFIX, StringComparison.OrdinalIgnoreCase)) continue;

            string keyRaw = n.Substring(PANEL_PREFIX.Length);
            string key = NormalizeKey(keyRaw);
            if (string.IsNullOrEmpty(key))
            {
                Debug.LogWarning($"[UIManager] 잘못된 패널 이름: '{n}'");
                continue;
            }

            if (panels.ContainsKey(key))
                Debug.LogWarning($"[UIManager] 중복 패널 키 '{keyRaw}' 감지. 마지막 값을 사용합니다.");

            panels[key] = t.gameObject; // 덮어씀
        }
    }

    private void AutoBindButtons()
    {
        if (canvasScopes == null || canvasScopes.Length == 0)
            canvasScopes = FindObjectsOfType<Canvas>(true);

        foreach (var canvas in canvasScopes)
        {
            if (!canvas) continue;
            BindButtonsUnder(canvas.transform);
        }
    }

    private void BindButtonsUnder(Transform root)
    {
        foreach (var btn in root.GetComponentsInChildren<Button>(true))
        {
            var n = btn.gameObject.name;
            if (!n.StartsWith(BUTTON_PREFIX, StringComparison.OrdinalIgnoreCase)) continue;

            var key = NormalizeKey(n.Substring(BUTTON_PREFIX.Length));
            if (string.IsNullOrEmpty(key))
            {
                Debug.LogWarning($"[UIManager] 잘못된 버튼 이름: '{n}'");
                continue;
            }

            btn.onClick.AddListener(() => OpenPanel(key)); // Btn.X → Panel.X
        }
    }
    #endregion

    #region 패널 제어
    public void OpenPanel(string rawKey, bool toggleIfSame = true)
    {
        string key = NormalizeKey(rawKey);
        if (string.IsNullOrEmpty(key))
        {
            Debug.LogWarning("[UIManager] OpenPanel: key가 비었습니다.");
            return;
        }

        if (!panels.TryGetValue(key, out var target) || target == null)
        {
            Debug.LogWarning($"[UIManager] OpenPanel: '{rawKey}' 패널을 찾을 수 없습니다. (Panel.{rawKey})");
            return;
        }

        if (!string.IsNullOrEmpty(currentPanelKey) && currentPanelKey == key)
        {
            if (toggleIfSame) CloseAllPanels();
            else UpdateUIState();
            return;
        }

        foreach (var kv in panels)
            kv.Value?.SetActive(kv.Key == key);

        currentPanelKey = key;
        //isUIOpen.Add(key);
        UpdateUIState();
    }

    public void CloseAllPanels()
    {
        foreach (var go in panels.Values) go?.SetActive(false);
        currentPanelKey = null;
        UpdateUIState();
    }
    #endregion

    #region 팝업 제어


    // 키 + 루트 index (0~7)
    // string 버전
    public GameObject ShowPopup(string rawKey, int rootIndex, object initData = null)
    {
        var prefab = LoadPopupPrefab(rawKey);
        if (!prefab) return null;

        var go = Instantiate(prefab, popupRoots[rootIndex], false);

        ShowPopupInternal(go, rootIndex); 
        return go;
    }


    // GameObject 버전
    private void ShowPopupInternal(GameObject popup, int rootIndex)
    {
        if (!popup) return;
        if (popupStack.Contains(popup)) return;

        popup.transform.SetParent(popupRoots[rootIndex], false);
        popup.SetActive(true);
        popupStack.Push(popup);

        UpdateUIState();
    }

    public void CloseTopPopup()
    {
        PruneDeadPopups();
        if (popupStack.Count == 0) return;

        var top = popupStack.Pop();
        if (top)
        {
            // var p = top.GetComponent<IPopup>(); p?.OnClose();
            top.SetActive(false);
            SafeDestroy(top);
        }
        UpdateUIState();
    }

    public void CloseSpecificPopup(GameObject popup)
    {
        PruneDeadPopups();
        if (!popup || popupStack.Count == 0) return;

        var temp = new Stack<GameObject>();
        bool closed = false;

        while (popupStack.Count > 0)
        {
            var p = popupStack.Pop();
            if (!closed && p == popup)
            {
                if (p)
                {
                    // var ip = p.GetComponent<IPopup>(); ip?.OnClose();
                    p.SetActive(false);
                    SafeDestroy(p);
                }
                closed = true;
                continue;
            }
            temp.Push(p);
        }
        while (temp.Count > 0) popupStack.Push(temp.Pop());

        UpdateUIState();
    }

    // 유틸
    private void PruneDeadPopups()
    {
        if (popupStack.Count == 0) return;
        var temp = new Stack<GameObject>();
        while (popupStack.Count > 0)
        {
            var p = popupStack.Pop();
            if (p) temp.Push(p);
        }
        while (temp.Count > 0) popupStack.Push(temp.Pop());
    }

   

    // 프리팹 로딩 (캐시 → Resources)
    private GameObject LoadPopupPrefab(string rawKey)
    {
        var key = NormalizeKey(rawKey);
        if (string.IsNullOrEmpty(key)) return null;

        if (popupPrefabCache.TryGetValue(key, out var cached) && cached) return cached;

        var path = POPUP_FOLDER + POPUP_PREFIX + rawKey;
        var prefab = Resources.Load<GameObject>(path);
        if (!prefab)
        {
            Debug.LogWarning($"[UIManager] Popup prefab not found at Resources/{path}");
            return null;
        }
        popupPrefabCache[key] = prefab;
        return prefab;
    }
    #endregion

    #region 토스트
    public void ShowToast(string msg)
    {
        if (!toastPrefab || !toastRoot)
        {
            Debug.LogWarning("[UIManager] Toast 설정 누락 (toastPrefab/toastRoot)");
            return;
        }

        while (activeToasts.Count >= maxToasts)
        {
            var old = activeToasts.Dequeue();
            if (old) Destroy(old);
        }

        var go = Instantiate(toastPrefab, toastRoot);
        activeToasts.Enqueue(go);

        var toast = go.GetComponent<Toast>();
        if (toast) toast.SetText(msg);

        UpdateUIState();
        StartCoroutine(_ToastLifetime(go, toast));
    }

    private System.Collections.IEnumerator _ToastLifetime(GameObject go, Toast toast)
    {
        if (toast != null) yield return toast.PlayIn();
        yield return new WaitForSecondsRealtime(toastLife);
        if (toast != null) yield return toast.PlayOut();

        if (activeToasts.Count > 0 && activeToasts.Peek() == go)
            activeToasts.Dequeue();
        else
        {
            var temp = new Queue<GameObject>();
            while (activeToasts.Count > 0)
            {
                var x = activeToasts.Dequeue();
                if (x != go) temp.Enqueue(x);
            }
            while (temp.Count > 0) activeToasts.Enqueue(temp.Dequeue());
        }

        if (go) Destroy(go);
        UpdateUIState();
    }
    #endregion

    #region 유틸리티/공통
    private static string NormalizeKey(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw)) return string.Empty;
        return raw.Trim().ToLowerInvariant().Replace(" ", "");
    }

    private void HandleBack()
    {
        // 1) 팝업 우선 처리
        PruneDeadPopups();
        if (popupStack.Count > 0)
        {
            var top = popupStack.Peek();
            if (top)
            {
                var policy = top.GetComponent<UIClosePolicy>();
                if (policy == null || policy.closeOnBack)   // 정책이 없거나, 허용일 때만 닫기
                {
                    CloseTopPopup();
                }
            }
            return;
        }

        // 2) 패널 처리
        if (!string.IsNullOrEmpty(currentPanelKey) && panels.TryGetValue(currentPanelKey, out var panel) && panel)
        {
            var policy = panel.GetComponent<UIClosePolicy>();
            if (policy == null || policy.closeOnBack)
            {
                CloseAllPanels();
            }
            return;
        }
        CloseAllPanels();
            return;

        // TODO: 기본 동작 (예: 종료 팝업 등)
    }

    public bool RegisterPanel(string rawKey, GameObject panel)
    {
        if (!panel) return false;
        string key = NormalizeKey(rawKey);
        if (string.IsNullOrEmpty(key)) return false;

        panels[key] = panel;
        panel.SetActive(false);
        return true;
    }

    public void UnregisterPanel(string rawKey)
    {
        string key = NormalizeKey(rawKey);
        if (string.IsNullOrEmpty(key)) return;
        panels.Remove(key);
    }

    private System.Collections.IEnumerator _DestroyNextFrame(GameObject go)
    {
        yield return null;
        if (go) Destroy(go);
    }

    private void SafeDestroy(GameObject go)
    {
        if (!go) return;
#if UNITY_EDITOR
        var sel = UnityEditor.Selection.activeGameObject;
        if (sel && (sel == go || sel.transform.IsChildOf(go.transform)))
            UnityEditor.Selection.activeGameObject = null;
#endif
        StartCoroutine(_DestroyNextFrame(go));
    }

    private void UpdateUIState()
    {
        bool hasPanel = !string.IsNullOrEmpty(currentPanelKey);
        bool hasPopup = popupStack.Count > 0;
        bool hasToast = activeToasts.Count > 0;


        IsUIOpenRx.Value = hasPanel || hasPopup || hasToast || isUIOpen.Count > 0 ;
        UIManager.IsUIOpen = IsUIOpenRx.Value;
    }
    #endregion



    
    public void AddHashSet<T>(T manjun)
    {
        string a = manjun.GetType().Name;
        isUIOpen.Add(a);
        IsUIOpen = true;
        IsUIOpenRx.Value = true;
    }

    public void RemoveHashSet<T>(T manjun)
    {
        string a = manjun.GetType().Name;
        isUIOpen.Remove(a);

        UpdateUIState();
    }


    public void TestPopup(int num)
    {
        ShowPopup("Test", num);
    }

}
