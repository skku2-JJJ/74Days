using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class ResourceDistributeUI : MonoBehaviour
{
    public static ResourceDistributeUI Instance { get; private set; }

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
        Debug.Log("[ResourceDistributionUI.Awake] 시작");

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // DayManager 이벤트 구독 (Start보다 먼저 실행되는 Awake에서)
        Debug.Log($"[ResourceDistributionUI.Awake] DayManager.Instance is null? {DayManager.Instance == null}");

        if (DayManager.Instance != null)
        {
            DayManager.Instance.OnPhaseChange += OnPhaseChanged;
            Debug.Log("[ResourceDistributionUI.Awake] 이벤트 구독 성공!");
        }
        else
        {
            Debug.LogError("[ResourceDistributionUI.Awake] DayManager.Instance가 null입니다!");
        }
    }

    void Start()
    {
        Debug.Log("[ResourceDistributionUI.Start] 시작");

        // 버튼 이벤트 등록
        if (_completeButtonUI != null)
        {
            _completeButtonUI.onClick.AddListener(OnCompleteButtonClicked);
        }

        if (_closeButtonUI != null)
        {
            _closeButtonUI.onClick.AddListener(Hide);
        }
    }

    void OnDestroy()
    {
        if (DayManager.Instance != null)
        {
            DayManager.Instance.OnPhaseChange -= OnPhaseChanged;
        }
    }

    // 페이즈 변경 시 호출
    private void OnPhaseChanged(DayPhase phase)
    {
        Debug.Log($"[ResourceDistributionUI.OnPhaseChanged] Phase: {phase}");

        if (phase == DayPhase.Evening)
        {
            Debug.Log("[ResourceDistributionUI] Evening 페이즈 진입 → 슬롯 생성 시작");
            UpdateDayInfo();
            CreateCrewSlots();
            CreateResourceSlots();
            Show();
        }
        else
        {
            // Evening이 아닌 페이즈로 전환되면 패널 닫기
            Hide();

            // Night 페이즈일 때는 슬롯도 정리
            if (phase == DayPhase.Night)
            {
                Debug.Log("[ResourceDistributionUI] Night 페이즈 진입 → 슬롯 정리");
                ClearSlots();
            }
        }
    }

    // ========== 팝업 제어 ==========

    public void Show()
    {
        if (_panelRootUI != null)
        {
            _panelRootUI.SetActive(true);
        }

        Debug.Log("[자원 분배] 팝업 표시");
    }

    public void Hide()
    {
        if (_panelRootUI != null)
        {
            _panelRootUI.SetActive(false);
        }

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
        Debug.Log("[CreateCrewSlots] 시작");
        ClearCrewSlots();

        if (_crewSectionParentUI == null || _crewSlotPrefabUI == null)
        {
            Debug.LogError("[CreateCrewSlots] Parent 또는 Prefab이 null입니다!");
            return;
        }

        if (CrewManager.Instance == null)
        {
            Debug.LogError("[CreateCrewSlots] CrewManager.Instance가 null입니다!");
            return;
        }

        Debug.Log($"[CreateCrewSlots] 선원 수: {CrewManager.Instance.CrewMembers.Count}");

        foreach (var crew in CrewManager.Instance.CrewMembers)
        {
            if (!crew.IsAlive) continue;

            GameObject slotObj = Instantiate(_crewSlotPrefabUI, _crewSectionParentUI);
            CrewDropSlot slot = slotObj.GetComponent<CrewDropSlot>();

            if (slot != null)
            {
                slot.Initialize(crew);
                _crewSlots.Add(slot);
                Debug.Log($"[CreateCrewSlots] {crew.CrewName} 슬롯 생성 완료");
            }
        }
    }

    // 자원 슬롯 생성
    private void CreateResourceSlots()
    {
        Debug.Log("[CreateResourceSlots] 시작");
        ClearResourceSlots();

        if (_resourceSectionParentUI == null || _resourceSlotPrefabUI == null)
        {
            Debug.LogError("[CreateResourceSlots] Parent 또는 Prefab이 null입니다!");
            return;
        }

        if (ShipManager.Instance == null)
        {
            Debug.LogError("[CreateResourceSlots] ShipManager.Instance가 null입니다!");
            return;
        }

        // 각 자원 타입별로 슬롯 생성
        CreateResourceSlot(ResourceType.NormalFish, "생선");
        CreateResourceSlot(ResourceType.SpecialFish, "조개");
        CreateResourceSlot(ResourceType.Seaweed, "해초");
        CreateResourceSlot(ResourceType.CleanWater, "물");
        CreateResourceSlot(ResourceType.Herbs, "약초");
        CreateResourceSlot(ResourceType.Wood, "목재");

        Debug.Log($"[CreateResourceSlots] 총 {_resourceSlots.Count}개 슬롯 생성 완료");
    }

    private void CreateResourceSlot(ResourceType type, string displayName)
    {
        int amount = ShipManager.Instance.GetResourceAmount(type);
        Debug.Log($"[CreateResourceSlot] {displayName}({type}): {amount}개");

        GameObject slotObj = Instantiate(_resourceSlotPrefabUI, _resourceSectionParentUI);
        ResourceSlotItem slot = slotObj.GetComponent<ResourceSlotItem>();

        if (slot != null)
        {
            slot.Initialize(type, displayName, amount);
            _resourceSlots.Add(slot);
        }
        else
        {
            Debug.LogError($"[CreateResourceSlot] {displayName} 슬롯의 ResourceSlotItem 컴포넌트가 없습니다!");
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
            DayManager.Instance.GoToNight();
        }

        Debug.Log("[자원 분배] 분배 완료 버튼 클릭");
    }
}