using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class ResourceDistributionUI : MonoBehaviour
{
    public static ResourceDistributionUI Instance { get; private set; }

    [Header("Header")]
    [SerializeField] private TextMeshProUGUI _dayTextUI;
    [SerializeField] private TextMeshProUGUI _descriptionTextUI;

    [Header("Crew Section")]
    [SerializeField] private Transform _crewSectionParentUI;
    [SerializeField] private GameObject _crewSlotPrefabUI;

    [Header("Resource Section")]
    [SerializeField] private Transform _resourceSectionParentUI;
    [SerializeField] private GameObject _resourceSlotPrefabUI;

    [Header("Panel")]
    [SerializeField] private GameObject _panelRootUI;
    [SerializeField] private Button _completeButtonUI;
    [SerializeField] private Button _closeButtonUI;

    private List<CrewDropSlot> _crewSlots = new List<CrewDropSlot>();
    private List<ResourceSlotItem> _resourceSlots = new List<ResourceSlotItem>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // 버튼 이벤트 등록
        if (_completeButtonUI != null)
        {
            _completeButtonUI.onClick.AddListener(OnCompleteButtonClicked);
        }

        if (_closeButtonUI != null)
        {
            _closeButtonUI.onClick.AddListener(Hide);
        }

        // DayManager 이벤트 구독
        if (DayManager.Instance != null)
        {
            DayManager.Instance.OnPhaseChange += OnPhaseChanged;
        }

        Hide();
    }

    void OnDestroy()
    {
        if (DayManager.Instance != null)
        {
            DayManager.Instance.OnPhaseChange -= OnPhaseChanged;
        }
    }

    // Evening 페이즈에 자동 표시
    private void OnPhaseChanged(DayPhase phase)
    {
        if (phase == DayPhase.Evening)
        {
            Show();
        }
    }

    // ========== 팝업 제어 ==========

    public void Show()
    {
        if (_panelRootUI != null)
        {
            _panelRootUI.SetActive(true);
        }

        UpdateDayInfo();
        CreateCrewSlots();
        CreateResourceSlots();

        Debug.Log("[자원 분배] 팝업 표시");
    }

    public void Hide()
    {
        if (_panelRootUI != null)
        {
            _panelRootUI.SetActive(false);
        }

        ClearSlots();
        Debug.Log("[자원 분배] 팝업 닫힘");
    }

    // ========== UI 생성 ==========

    private void UpdateDayInfo()
    {
        if (_dayTextUI != null && DayManager.Instance != null)
        {
            _dayTextUI.text = $"{DayManager.Instance.CurrentDay}일 차";
        }

        if (_descriptionTextUI != null)
        {
            _descriptionTextUI.text = "배급 시간이다...";
        }
    }

    // 선원 슬롯 생성
    private void CreateCrewSlots()
    {
        ClearCrewSlots();

        if (_crewSectionParentUI == null || _crewSlotPrefabUI == null) return;
        if (CrewManager.Instance == null) return;

        foreach (var crew in CrewManager.Instance.CrewMembers)
        {
            if (!crew.IsAlive) continue;

            GameObject slotObj = Instantiate(_crewSlotPrefabUI, _crewSectionParentUI);
            CrewDropSlot slot = slotObj.GetComponent<CrewDropSlot>();

            if (slot != null)
            {
                slot.Initialize(crew);
                _crewSlots.Add(slot);
            }
        }
    }

    // 자원 슬롯 생성
    private void CreateResourceSlots()
    {
        ClearResourceSlots();

        if (_resourceSectionParentUI == null || _resourceSlotPrefabUI == null) return;
        if (ShipManager.Instance == null) return;

        // 각 자원 타입별로 슬롯 생성
        CreateResourceSlot(ResourceType.Fish, "생선");
        CreateResourceSlot(ResourceType.Shellfish, "조개");
        CreateResourceSlot(ResourceType.Seaweed, "해초");
        CreateResourceSlot(ResourceType.CleanWater, "물");
        CreateResourceSlot(ResourceType.Herbs, "약초");
        CreateResourceSlot(ResourceType.Wood, "목재");
    }

    private void CreateResourceSlot(ResourceType type, string displayName)
    {
        int amount = ShipManager.Instance.GetResourceAmount(type);

        GameObject slotObj = Instantiate(_resourceSlotPrefabUI, _resourceSectionParentUI);
        ResourceSlotItem slot = slotObj.GetComponent<ResourceSlotItem>();

        if (slot != null)
        {
            slot.Initialize(type, displayName, amount);
            _resourceSlots.Add(slot);
        }
    }

    // ========== 슬롯 정리 ==========

    private void ClearSlots()
    {
        ClearCrewSlots();
        ClearResourceSlots();
    }

    private void ClearCrewSlots()
    {
        foreach (var slot in _crewSlots)
        {
            if (slot != null)
            {
                Destroy(slot.gameObject);
            }
        }
        _crewSlots.Clear();
    }

    private void ClearResourceSlots()
    {
        foreach (var slot in _resourceSlots)
        {
            if (slot != null)
            {
                Destroy(slot.gameObject);
            }
        }
        _resourceSlots.Clear();
    }

    // ========== 자원 분배 ==========

    // 자원 슬롯 수량 갱신
    public void RefreshResourceSlot(ResourceType type)
    {
        foreach (var slot in _resourceSlots)
        {
            if (slot.ResourceType == type)
            {
                int newAmount = ShipManager.Instance.GetResourceAmount(type);
                slot.UpdateAmount(newAmount);
                break;
            }
        }
    }

    // 분배 완료 버튼
    private void OnCompleteButtonClicked()
    {
        Hide();

        // Night 페이즈로 진행
        if (DayManager.Instance != null)
        {
            DayManager.Instance.ChangePhase(DayPhase.Night);
        }

        Debug.Log("[자원 분배] 분배 완료 → Night 페이즈");
    }
}