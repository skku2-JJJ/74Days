using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI Instance { get; private set; }

    [Header("Food Resources")]
    [SerializeField] private TextMeshProUGUI _fishTextUI;
    [SerializeField] private TextMeshProUGUI _shellfishTextUI;
    [SerializeField] private TextMeshProUGUI _seaweedTextUI;

    [Header("Water")]
    [SerializeField] private TextMeshProUGUI _waterTextUI;

    [Header("Medical")]
    [SerializeField] private TextMeshProUGUI _herbsTextUI;

    [Header("Repair Materials")]
    [SerializeField] private TextMeshProUGUI _woodTextUI;

    [Header("Panel")]
    [SerializeField] private GameObject _panelRootUI;
    [SerializeField] private Button _closeButtonUI;

    void Awake()
    {
        // 싱글톤
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // DayManager 이벤트 구독
        if (DayManager.Instance != null)
        {
            DayManager.Instance.OnPhaseChange += OnPhaseChanged;
        }
    }

    void Start()
    {
        // 닫기 버튼 이벤트 등록
        if (_closeButtonUI != null)
        {
            _closeButtonUI.onClick.AddListener(Hide);
        }

        // 시작 시 숨기기
        //Hide();
    }

    void OnDestroy()
    {
        // 이벤트 구독 해제
        if (DayManager.Instance != null)
        {
            DayManager.Instance.OnPhaseChange -= OnPhaseChanged;
        }
    }

    // 페이즈 변경 시 호출
    private void OnPhaseChanged(DayPhase phase)
    {
        // 페이즈가 전환되면 인벤토리 패널 닫기
        Hide();
    }

    // ========== 팝업 제어 ==========

    public void Show()
    {
        if (_panelRootUI != null)
        {
            _panelRootUI.SetActive(true);
        }

        UpdateAllInfo();
        Debug.Log("[인벤토리] 팝업 표시");
    }

    public void Hide()
    {
        if (_panelRootUI != null)
        {
            _panelRootUI.SetActive(false);
        }

        Debug.Log("[인벤토리] 팝업 닫힘");
    }

    public void Toggle()
    {
        if (_panelRootUI != null && _panelRootUI.activeSelf)
        {
            Hide();
        }
        else
        {
            Show();
        }
    }

    // ========== 정보 갱신 ==========

    public void UpdateAllInfo()
    {
        if (ShipManager.Instance == null) return;

        // 식량
        if (_fishTextUI != null)
            _fishTextUI.text = $"생선 x {ShipManager.Instance.GetResourceAmount(ResourceType.Fish)}";

        if (_shellfishTextUI != null)
            _shellfishTextUI.text = $"조개 x {ShipManager.Instance.GetResourceAmount(ResourceType.Shellfish)}";

        if (_seaweedTextUI != null)
            _seaweedTextUI.text = $"해초 x {ShipManager.Instance.GetResourceAmount(ResourceType.Seaweed)}";

        // 물
        if (_waterTextUI != null)
            _waterTextUI.text = $"깨끗한 물 x {ShipManager.Instance.GetResourceAmount(ResourceType.CleanWater)}";

        // 의료
        if (_herbsTextUI != null)
            _herbsTextUI.text = $"약초 x {ShipManager.Instance.GetResourceAmount(ResourceType.Herbs)}";

        // 수리 재료
        if (_woodTextUI != null)
            _woodTextUI.text = $"목재 x {ShipManager.Instance.GetResourceAmount(ResourceType.Wood)}";
    }
}