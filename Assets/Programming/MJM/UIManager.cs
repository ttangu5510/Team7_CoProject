using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class UIPopupOptions : MonoBehaviour { public bool IsModal = true; }

public class UIManager : MonoBehaviour
{
    public static bool IsUIOpen { get; set; }   // 유아이 켜짐 꺼짐 상태변수 

    private static UIManager instance;
    public static UIManager Instance => instance;

    [Header("Auto-Bind Roots")]
    [SerializeField] private Transform panelsRoot;      // 판넬 루트 - 판넬은 여기에서 생성됨
    [SerializeField] private Transform popupsRoot;      // 팝업 루트 - 팝업은 여기에서 생성됨
    [SerializeField] private Canvas[] canvasScopes;     // 비워두면 씬의 모든 Canvas에서 이름이 Btn.~~ 인 모든 버튼을 자동 탐색

    [Header("Popup")]
    [SerializeField] private GameObject popupBlocker;   // 팝업시 다른 터치를 막는 용도의 오브젝트

    // ===== 문자열 키 기반 패널 관리 =====
    private readonly Dictionary<string, GameObject> panels = new(); // key: normalized name
    private string currentPanelKey; // null = 아무 패널도 안 열림

    // ===== 팝업 스택 =====
    private readonly Stack<GameObject> popupStack = new();

    // ===== 팝업 캐시 딕셔너리 =====
    private readonly Dictionary<string, GameObject> popupPrefabCache = new();

    [Header("Toast")]
    [SerializeField] private Transform toastRoot;
    [SerializeField] private GameObject toastPrefab;
    [SerializeField] private int maxToasts = 3;
    [SerializeField] private float toastLife = 1.8f;
    private readonly Queue<GameObject> activeToasts = new();

    [Header("Popup Sorting (optional)")]
    [SerializeField] private int popupBaseOrder = 500;
    [SerializeField] private int popupOrderStep = 10;

    // ===== 키 정규화 유틸 =====
    private const string PANEL_PREFIX = "Panel.";
    private const string BUTTON_PREFIX = "Btn.";

    // ===== 팝업프리펩 리소스 폴더에서 찾아주는 역할 =====
    const string POPUP_PREFIX = "Popup.";
    const string POPUP_FOLDER = "Popups/"; // Resources/Popups/Popup.<Key>.prefab




    // 공백제거, 소문자로 변경해주는 함수
    private static string NormalizeKey(string raw)
    {
        if (string.IsNullOrWhiteSpace(raw)) return string.Empty;
        return raw.Trim().ToLowerInvariant().Replace(" ", ""); // 내부 공백까지 제거 원치 않으면 Replace 제거
    }




    private void Awake()
    {
        // ===== 싱글톤 보장 & 파괴 금지 =====
        if (instance == null) { instance = this; DontDestroyOnLoad(gameObject); }
        else { Destroy(gameObject); return; }

        // ===== 초기 바인딩 =====
        AutoBindPanels();   // Panels 하위의 Panel.* 오브젝트를 사전에 등록
        AutoBindButtons();  // BottomBar 하위의 Btn.* 버튼 클릭을 OpenPanel에 연결

        // ===== 시작 상태 초기화 =====
        foreach (var go in panels.Values) go?.SetActive(false);
        popupBlocker?.SetActive(false);
        currentPanelKey = null;

        // popupsRoot 자동 확보
        if (!popupsRoot)
        {
            var found = GameObject.Find("Popups");
            if (found) popupsRoot = found.transform;
            else
            {
                var go = new GameObject("Popups");
                var canvas = go.AddComponent<Canvas>();
                canvas.overrideSorting = true; canvas.sortingOrder = popupBaseOrder;
                popupsRoot = go.transform;
            }
        }
        // Popups 루트에 CanvasGroup 달아서 레이캐스트 제어 추천
        if (!popupsRoot.GetComponent<CanvasGroup>()) popupsRoot.gameObject.AddComponent<CanvasGroup>();
        if (!popupsRoot.GetComponent<GraphicRaycaster>()) popupsRoot.gameObject.AddComponent<GraphicRaycaster>();



        UpdateBlockerByStack();

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
            HandleBack();
    }

    // ===================== 자동 바인딩 =====================
    private void AutoBindPanels()
    {
        panels.Clear();

        // Panels 루트 자동 탐색(인스펙터 미할당 시)
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

        // Panels 하위의 직계 자식 중 이름이 "Panel.<키>" 인 것만 등록
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

            panels[key] = t.gameObject; // 같은 키가 있으면 덮어씀
        }
    }

    private void AutoBindButtons()
    {
        // 1) 스코프 비었으면 씬 내 모든 Canvas 탐색(비활성 포함)
        if (canvasScopes == null || canvasScopes.Length == 0)
            canvasScopes = FindObjectsOfType<Canvas>(true);

        // 2) 각 Canvas 하위의 모든 Button 검색 후 이름 규칙에 맞는 것만 연결
        foreach (var canvas in canvasScopes)
        {
            if (!canvas) continue;
            BindButtonsUnder(canvas.transform);
        }
    }

    // Canvas/루트 하위 버튼 묶음 바인딩
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

            //btn.onClick.RemoveAllListeners(); // <- 인스펙터의 연결을 끊지 않게 변경했음
            btn.onClick.AddListener(() => OpenPanel(key)); // Btn.X → Panel.X
        }
    }


    // ===================== 패널 제어 =====================
    // OpenPanel("Info") → "Panel.Info"를 찾음
    public void OpenPanel(string rawKey)
    {
        string key = NormalizeKey(rawKey);
        if (string.IsNullOrEmpty(key))
        {
            Debug.LogWarning("[UIManager] OpenPanel: key가 비었습니다.");
            return;
        }

        // 등록된 패널 조회
        if (!panels.TryGetValue(key, out var target) || target == null)
        {
            Debug.LogWarning($"[UIManager] OpenPanel: '{rawKey}' 패널을 찾을 수 없습니다. (Panel.{rawKey})");
            return;
        }

        // 같은 패널이 이미 열려 있으면 토글처럼 전체 닫기
        if (!string.IsNullOrEmpty(currentPanelKey) && currentPanelKey == key)
        {
            CloseAllPanels();
            return;
        }

        // 해당 패널만 활성화, 나머지는 비활성화
        foreach (var kv in panels)
            kv.Value?.SetActive(kv.Key == key);

        currentPanelKey = key;

        // 베이스 패널은 블로커 불필요 (모달 아님)
        if (popupStack.Count == 0) popupBlocker?.SetActive(false);

        // isuiopen 상태변수 제어용
        currentPanelKey = key;
        UpdateUIOpenFlag();
    }

    public void CloseAllPanels()
    {
        // 모두 끄기
        foreach (var go in panels.Values) go?.SetActive(false);
        currentPanelKey = null;

        // 팝업 없으면 블로커도 끔
        if (popupStack.Count == 0) popupBlocker?.SetActive(false);

        // isuiopen 상태변수 제어용
        currentPanelKey = null;
        UpdateUIOpenFlag();
    }

    // ===================== 팝업 =====================
    public void ShowPopup(GameObject popup)
    {
        if (!popup) return;
        if (popupStack.Contains(popup)) return;

        popup.SetActive(true);
        popupStack.Push(popup);
        UpdatePopupSorting();
        UpdateBlockerByStack();

        // isuiopen 상태변수 제어용
        popupStack.Push(popup);
        UpdateUIOpenFlag();
    }

    public void CloseTopPopup()
    {
        PruneDeadPopups();
        if (popupStack.Count == 0) return;

        var top = popupStack.Pop();
        if (top)
        {
            // var p = top.GetComponent<IPopup>(); p?.OnClose(); // 선택
            top.SetActive(false);
            SafeDestroy(top); // 인스턴스는 파괴(토스트처럼 누수 방지)
        }
        UpdatePopupSorting();
        UpdateBlockerByStack();

        // isuiopen 상태변수 제어용
        UpdateUIOpenFlag();
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
                    // var ip = p.GetComponent<IPopup>(); ip?.OnClose(); // 선택
                    p.SetActive(false);
                    SafeDestroy(p);
                }
                closed = true;
                continue;
            }
            temp.Push(p);
        }
        while (temp.Count > 0) popupStack.Push(temp.Pop());

        UpdatePopupSorting();
        UpdateBlockerByStack();
    }

    // 스택 안에 파괴된(GameObject == null) 항목들을 제거
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

    private void UpdatePopupSorting()
    {
        int i = 0;
        foreach (var p in popupStack)
        {
            if (!p) continue;

            var c = p.GetComponent<Canvas>();
            if (!c)
            {
                c = p.AddComponent<Canvas>();
                c.overrideSorting = true;

                if (!p.GetComponent<UnityEngine.UI.GraphicRaycaster>())
                    p.AddComponent<UnityEngine.UI.GraphicRaycaster>();
            }
            else
            {
                c.overrideSorting = true;
                if (!p.GetComponent<UnityEngine.UI.GraphicRaycaster>())
                    p.AddComponent<UnityEngine.UI.GraphicRaycaster>();
            }

            c.sortingOrder = popupBaseOrder + i * popupOrderStep;
            i++;
        }
    }

    public class UIPopupOptions : MonoBehaviour { public bool IsModal = true; }

    private bool HasModalPopup()
    {
        foreach (var p in popupStack)
        {
            if (!p) continue;
            var opt = p.GetComponent<UIPopupOptions>();
            if (opt == null || opt.IsModal) return true;
        }
        return false;
    }


    private void UpdateBlockerByStack()
    {
        bool anyModal = HasModalPopup();
        var cg = popupsRoot.GetComponent<CanvasGroup>();
        if (cg)
        {
            cg.blocksRaycasts = anyModal; // 팝업 있을 때만 뒤 입력 차단
            cg.interactable = anyModal;
        }
        if (popupBlocker) popupBlocker.SetActive(anyModal);
    }


    // ============ 프리팹 로딩 ============
    // 1) 캐시 → 2) Resources.Load("Popups/Popup.<Key>")
    private GameObject LoadPopupPrefab(string rawKey)
    {
        var key = NormalizeKey(rawKey);
        if (string.IsNullOrEmpty(key)) return null;

        if (popupPrefabCache.TryGetValue(key, out var cached) && cached) return cached;

        var path = POPUP_FOLDER + POPUP_PREFIX + rawKey; // 대소문자 구분 없음 처리 원하면 NormalizeKey로 일치화
        var prefab = Resources.Load<GameObject>(path);

        // Addressables 사용 시(선택):
        // var handle = Addressables.LoadAssetAsync<GameObject>($"Popup.{rawKey}");
        // var prefab = handle.WaitForCompletion();

        if (!prefab)
        {
            Debug.LogWarning($"[UIManager] Popup prefab not found at Resources/{path}");
            return null;
        }
        popupPrefabCache[key] = prefab;
        return prefab;
    }

    // ============ 팝업 열기(키 기반) ============
    public GameObject ShowPopup(string rawKey, bool modal = true, object initData = null)
    {
        var prefab = LoadPopupPrefab(rawKey);
        if (!prefab) return null;

        var go = Instantiate(prefab, popupsRoot);
        // NonModal 처리(태그나 플래그로 구분하고 싶으면 여기서 적용)
        if (!modal) go.tag = "NonModal";

        // 팝업 스크립트가 있으면 초기화 데이터 넘기기(선택)
        // var p = go.GetComponent<IPopup>(); p?.OnOpen(initData);

        ShowPopup(go); // 아래 GO 기반 오버로드 재사용
        return go;
    }


    // ===================== 토스트 =====================
    public void ShowToast(string msg)
    {
        // 프리팹/루트가 없으면 경고
        if (!toastPrefab || !toastRoot)
        {
            Debug.LogWarning("[UIManager] Toast 설정 누락 (toastPrefab/toastRoot)");
            return;
        }

        // 최대 개수 초과 시 가장 오래된 토스트 제거
        while (activeToasts.Count >= maxToasts)
        {
            var old = activeToasts.Dequeue();
            if (old) Destroy(old);
        }

        // 토스트 생성 및 큐에 등록
        var go = Instantiate(toastPrefab, toastRoot);
        activeToasts.Enqueue(go);

        // 표시 텍스트 설정(Toast 스크립트 가정: SetText/PlayIn/PlayOut 제공)
        var toast = go.GetComponent<Toast>();
        if (toast) toast.SetText(msg);

        // 수명 코루틴
        StartCoroutine(_ToastLifetime(go, toast));

        // isuiopen 상태변수 제어 현재 주석처리로 토스트는 처리 안함
        // UpdateUIOpenFlag();
    }

    private System.Collections.IEnumerator _ToastLifetime(GameObject go, Toast toast)
    {
        if (toast != null) yield return toast.PlayIn();

        // 표시 유지 시간
        yield return new WaitForSecondsRealtime(toastLife);
        if (toast != null) yield return toast.PlayOut();

        // 큐에서 자기 자신 제거(맨 앞이면 바로 제거, 아니면 스캔해서 제거)
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

        // isuiopen 상태변수 제어 현재 주석처리로 토스트는 처리 안함
        // UpdateUIOpenFlag();
    }

    // ===================== Back 처리 =====================
    private void HandleBack()
    {
        // 우선 팝업이 있으면 최상단 팝업 닫기
        PruneDeadPopups();
        if (popupStack.Count > 0) { CloseTopPopup(); return; }

        // 그 다음 현재 패널이 있으면 모두 닫기
        if (!string.IsNullOrEmpty(currentPanelKey)) { CloseAllPanels(); return; }


        // TODO 아무 것도 없으면 기본 동작: 타이틀/종료 팝업 등
    }

    // ===================== 외부에서 등록/해제 (동적 확장용, 선택) =====================
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

    public void TestCode()
    {
        ShowPopup("Test");
    }

   

    // ===== 버그 해결을 위한 임시 안전제거 코드 =====
    private System.Collections.IEnumerator _DestroyNextFrame(GameObject go)
    {
        yield return null;              // 인스펙터가 selection 변경할 틈 주기
        if (go) Destroy(go);
    }

    private void SafeDestroy(GameObject go)
    {
        if (!go) return;

        // 에디터에서 현재 선택이 이 팝업(혹은 자식)이라면 선택 해제
#if UNITY_EDITOR
        var sel = UnityEditor.Selection.activeGameObject;
        if (sel && (sel == go || sel.transform.IsChildOf(go.transform)))
            UnityEditor.Selection.activeGameObject = null;
#endif

        StartCoroutine(_DestroyNextFrame(go));   // 즉시 Destroy 대신 다음 프레임에 파괴
    }

    // isuiopen 상태변수 제어
    private void UpdateUIOpenFlag()
    {
        IsUIOpen = !string.IsNullOrEmpty(currentPanelKey)
                   || popupStack.Count > 0
                   || activeToasts.Count > 0;
    }

}
