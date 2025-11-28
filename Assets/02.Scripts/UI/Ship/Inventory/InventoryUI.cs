using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour
{
    public static InventoryUI Instance { get; private set; }

    [Header("Slot Containers")]
    [SerializeField] private Transform[] _resourceSlotsContainers;  // 자원 슬롯 컨테이너들 (여러 개 지원)

    [Header("Panel")]
    [SerializeField] private GameObject _panelRootUI;
    [SerializeField] private Button _closeButtonUI;

    // 동적으로 관리되는 자원 슬롯 Dictionary
    private Dictionary<ResourceType, TextMeshProUGUI> _resourceTextSlots = new Dictionary<ResourceType, TextMeshProUGUI>();

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
    }

    void Start()
    {
        // 자원 슬롯 초기화 (동적 생성)
        InitializeResourceSlots();

        // DayManager 이벤트 구독
        if (DayManager.Instance != null)
        {
            DayManager.Instance.OnPhaseChange += OnPhaseChanged;
        }

        // ShipManager 자원 변경 이벤트 구독 (실시간 업데이트)
        if (ShipManager.Instance != null)
        {
            ShipManager.Instance.OnInventoryChanged += UpdateAllInfo;
        }

        // 초기 정보 업데이트
        UpdateAllInfo();
    }

    void OnDestroy()
    {
        // 이벤트 구독 해제
        if (DayManager.Instance != null)
        {
            DayManager.Instance.OnPhaseChange -= OnPhaseChanged;
        }

        if (ShipManager.Instance != null)
        {
            ShipManager.Instance.OnInventoryChanged -= UpdateAllInfo;
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

    // ========== 슬롯 초기화 ==========

    /// <summary>
    /// ResourceDatabase 기반으로 자원 슬롯 동적 초기화 (여러 컨테이너 지원)
    /// </summary>
    private void InitializeResourceSlots()
    {
        _resourceTextSlots.Clear();

        if (_resourceSlotsContainers == null || _resourceSlotsContainers.Length == 0)
        {
            Debug.LogError("[InventoryUI] _resourceSlotsContainers가 할당되지 않았습니다!");
            return;
        }

        if (ResourceDatabaseManager.Instance == null || ResourceDatabaseManager.Instance.Database == null)
        {
            Debug.LogError("[InventoryUI] ResourceDatabaseManager를 찾을 수 없습니다!");
            return;
        }

        var allResources = ResourceDatabaseManager.Instance.Database.allResources;

        // 모든 컨테이너의 자식들을 모아서 순서대로 처리
        List<Transform> allSlots = new List<Transform>();

        foreach (Transform container in _resourceSlotsContainers)
        {
            if (container == null) continue;

            for (int i = 0; i < container.childCount; i++)
            {
                allSlots.Add(container.GetChild(i));
                Debug.Log($"{container.GetChild(i).name}");
            }
        }

        Debug.Log($"[InventoryUI] {_resourceSlotsContainers.Length}개 컨테이너에서 총 {allSlots.Count}개 슬롯 발견");

        // ResourceDatabase 순서대로 슬롯 매핑
        for (int i = 0; i < allResources.Count && i < allSlots.Count; i++)
        {
            Transform slotTransform = allSlots[i];
            var resourceData = allResources[i];

            if (resourceData == null)
            {
                Debug.LogWarning($"[InventoryUI] ResourceDatabase의 Element {i}가 null입니다! 슬롯: {slotTransform.name}");
                continue;
            }

            // 슬롯 내에서 TextMeshProUGUI 컴포넌트 찾기 (자식 포함)
            var textComponent = slotTransform.GetComponentInChildren<TextMeshProUGUI>();

            if (textComponent != null)
            {
                _resourceTextSlots[resourceData.resourceType] = textComponent;
                Debug.Log($"[InventoryUI] {resourceData.resourceType} 슬롯 연결 완료 (슬롯: {slotTransform.name})");
            }
            else
            {
                Debug.LogWarning($"[InventoryUI] {slotTransform.name}에서 TextMeshProUGUI를 찾을 수 없습니다!");
            }
        }

        Debug.Log($"[InventoryUI] 총 {_resourceTextSlots.Count}개 자원 슬롯 초기화 완료");
    }

    // ========== 정보 갱신 ==========

    /// <summary>
    /// 모든 자원 슬롯 업데이트 (동적 생성 방식)
    /// </summary>
    public void UpdateAllInfo()
    {
        if (ShipManager.Instance == null) return;

        // Dictionary를 순회하며 모든 자원 텍스트 업데이트
        foreach (var kvp in _resourceTextSlots)
        {
            ResourceType resourceType = kvp.Key;
            TextMeshProUGUI textUI = kvp.Value;

            if (textUI != null)
            {
                int amount = ShipManager.Instance.GetResourceAmount(resourceType);
                textUI.text = $"{amount}";
            }
        }
    }

    /// <summary>
    /// 특정 자원만 업데이트 (최적화용)
    /// </summary>
    public void UpdateResource(ResourceType type)
    {
        if (ShipManager.Instance == null) return;

        if (_resourceTextSlots.TryGetValue(type, out TextMeshProUGUI textUI))
        {
            if (textUI != null)
            {
                int amount = ShipManager.Instance.GetResourceAmount(type);
                textUI.text = $"{amount}";
            }
        }
    }
}