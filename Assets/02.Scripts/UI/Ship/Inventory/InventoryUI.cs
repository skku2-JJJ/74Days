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

    // 페이즈 변경 시 호출
    private void OnPhaseChanged(DayPhase phase)
    {
        if (phase == DayPhase.Morning || phase == DayPhase.Evening)
        {
            UpdateAllInfo();
        }
    }

    // ========== 정보 갱신 ==========

    public void UpdateAllInfo()
    {
        if (ShipManager.Instance == null) return;

        // 식량
        if (_fishTextUI != null)
            _fishTextUI.text = $"{ShipManager.Instance.GetResourceAmount(ResourceType.BlowFish)}";

        if (_shellfishTextUI != null)
            _shellfishTextUI.text = $"{ShipManager.Instance.GetResourceAmount(ResourceType.DameselFish)}";

        if (_seaweedTextUI != null)
            _seaweedTextUI.text = $"{ShipManager.Instance.GetResourceAmount(ResourceType.EmeraldFish)}";

        // 물
        if (_waterTextUI != null)
            _waterTextUI.text = $"{ShipManager.Instance.GetResourceAmount(ResourceType.BlueTang)}";

        // 의료
        if (_herbsTextUI != null)
            _herbsTextUI.text = $"{ShipManager.Instance.GetResourceAmount(ResourceType.FileFish)}";

        // 수리 재료
        if (_woodTextUI != null)
            _woodTextUI.text = $"{ShipManager.Instance.GetResourceAmount(ResourceType.TinyFish)}";
    }
}