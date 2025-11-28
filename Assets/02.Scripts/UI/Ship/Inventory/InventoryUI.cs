using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI Instance { get; private set; }

    [Header("Food Resources")]
    [SerializeField] private TextMeshProUGUI _blowFishTextUI;
    [SerializeField] private TextMeshProUGUI _blueTangTextUI;
    [SerializeField] private TextMeshProUGUI _emeraldFishTextUI;
    [SerializeField] private TextMeshProUGUI _nemoTextUI;
    [SerializeField] private TextMeshProUGUI _sawSharkTextUI;
    [SerializeField] private TextMeshProUGUI _stripedMarlinTextUI;
    [SerializeField] private TextMeshProUGUI _turtleTextUI;
    [SerializeField] private TextMeshProUGUI _grouperTextUI;
    [SerializeField] private TextMeshProUGUI _attack1TextUI;
    [SerializeField] private TextMeshProUGUI _attack2TextUI;

    [Header("Water")]
    [SerializeField] private TextMeshProUGUI _waterTextUI;

    [Header("Medical")]
    [SerializeField] private TextMeshProUGUI _herbsTextUI;

    [Header("Repair Materials")]
    //[SerializeField] private TextMeshProUGUI _woodTextUI;

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
        if (_blowFishTextUI != null)
            _blowFishTextUI.text = $"{ShipManager.Instance.GetResourceAmount(ResourceType.BlowFish)}";

        if (_blueTangTextUI != null)
            _blueTangTextUI.text = $"{ShipManager.Instance.GetResourceAmount(ResourceType.BlueTang)}";

        if (_emeraldFishTextUI != null)
            _emeraldFishTextUI.text = $"{ShipManager.Instance.GetResourceAmount(ResourceType.EmeraldFish)}";
        
        if (_nemoTextUI != null)
            _nemoTextUI.text = $"{ShipManager.Instance.GetResourceAmount(ResourceType.Nemo)}";
        
        if (_sawSharkTextUI != null)
            _sawSharkTextUI.text = $"{ShipManager.Instance.GetResourceAmount(ResourceType.SawShark)}";
        
        if (_stripedMarlinTextUI != null)
            _stripedMarlinTextUI.text = $"{ShipManager.Instance.GetResourceAmount(ResourceType.StripedMarlin)}";
        
        if (_turtleTextUI != null)
            _turtleTextUI.text = $"{ShipManager.Instance.GetResourceAmount(ResourceType.Turtle)}";
        
        if (_grouperTextUI != null)
            _grouperTextUI.text = $"{ShipManager.Instance.GetResourceAmount(ResourceType.Grouper)}";
        
        if (_attack1TextUI != null)
            _attack1TextUI.text = $"{ShipManager.Instance.GetResourceAmount(ResourceType.Attack1)}";
        
        if (_attack2TextUI != null)
            _attack2TextUI.text = $"{ShipManager.Instance.GetResourceAmount(ResourceType.Attack2)}";

        // 물
        if (_waterTextUI != null)
            _waterTextUI.text = $"{ShipManager.Instance.GetResourceAmount(ResourceType.Water)}";

        // 의료
        if (_herbsTextUI != null)
            _herbsTextUI.text = $"{ShipManager.Instance.GetResourceAmount(ResourceType.Herb)}";

        // 수리 재료
       // if (_woodTextUI != null)
       //     _woodTextUI.text = $"{ShipManager.Instance.GetResourceAmount(ResourceType.Wood)}";
    }
}