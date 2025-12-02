using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// 배 씬의 자원 분배 UI 관리
/// Evening 페이즈 시작 시 UI 표시
/// 좌측: 선원 상태 표시, 우측: 자원 인벤토리 (드래그 앤 드롭)
/// 애니메이션은 UI Basic Open Close에서 처리
/// </summary>
public class ResourceDistributionUI : MonoBehaviour
{
    public static ResourceDistributionUI Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private RectTransform divisionUI;          // DivisionUI (메인 패널)
    [SerializeField] private Transform crewsParent;             // Crews (선원 리스트 부모)
    [SerializeField] private Transform boxElementParent;        // BoxElement (인벤토리 슬롯 부모)
    [SerializeField] private GameObject _divisionTable;          // division 영역
    [SerializeField] private GameObject _divisionGuide;          // division 가이드
    [SerializeField] private TextMeshProUGUI titleText;         // TitleText (제목)
    
    // Resource Icons는 ResourceDatabase에서 가져오므로 제거됨

    private List<CrewResourceItem> crewItems = new List<CrewResourceItem>();
    private List<InventorySlotUI> inventorySlots = new List<InventorySlotUI>();
    
    [SerializeField] private UIBasicOpenClose _distributeUI;

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
            return;
        }
    }

    void Start()
    {
        // 초기 상태: DivisionButton 숨김
        if (_divisionTable != null)
        {
            _divisionTable.SetActive(false);
            _divisionGuide.SetActive(false);
        }

        // Evening 페이즈 구독
        if (DayManager.Instance != null)
        {
            DayManager.Instance.OnPhaseChange += OnPhaseChanged;
        }

        // ShipManager 자원 변경 이벤트 구독 (실시간 업데이트)
        if (ShipManager.Instance != null)
        {
            ShipManager.Instance.OnResourceChanged += OnResourceChanged;
        }

        Debug.Log("[ResourceDistributionUI] 초기화 완료 - Evening 페이즈 대기 중");
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
            ShipManager.Instance.OnResourceChanged -= OnResourceChanged;
        }
    }

    // ========== 이벤트 핸들러 ==========

    /// <summary>
    /// 자원 변경 시 호출 (실시간 업데이트)
    /// </summary>
    private void OnResourceChanged(ResourceType type, int amount)
    {
        // 해당 자원 슬롯만 업데이트 (최적화)
        RefreshResourceSlot(type);
    }

    // ========== 페이즈 변경 이벤트 ==========

    private void OnPhaseChanged(DayPhase phase)
    {
        if (phase == DayPhase.Evening)
        {
            Debug.Log("[ResourceDistributionUI] Evening 페이즈 진입 - UI 표시");
            ShowUI();

            // DivisionButton 활성화
            if (_divisionTable != null)
            {
                _divisionTable.SetActive(true);
                _divisionGuide.SetActive(true);
            }
        }
        else
        {
            // Evening이 아닌 페이즈에서는 DivisionButton 숨김
            if (_divisionTable != null)
            {
                _divisionTable.SetActive(false);
                _divisionGuide.SetActive(false);
            }
        }
    }

    // ========== UI 표시/숨기기 ==========

    /// <summary>
    /// UI 표시 및 데이터 초기화
    /// </summary>
    public void ShowUI()
    {
        // 선원 슬롯 초기화
        CreateCrewSlots();

        // 자원 슬롯 초기화
        InitializeInventorySlots();

        // 이전 분배 내역 초기화
        ClearAllDivisionBoxes();

        Debug.Log("[ResourceDistributionUI] UI 표시 완료 - 데이터 최신화");
    }

    /// <summary>
    /// 모든 DivisionBox 초기화 (이전 할당 제거)
    /// </summary>
    private void ClearAllDivisionBoxes()
    {
        foreach (var crewItem in crewItems)
        {
            if (crewItem != null)
            {
                crewItem.ClearAssignedResources();
            }
        }

        // 모든 InventorySlot의 임시 예약도 초기화
        foreach (var slot in inventorySlots)
        {
            if (slot != null)
            {
                slot.ResetTemporaryReservations();
            }
        }

        Debug.Log("[ResourceDistributionUI] 모든 DivisionBox 및 임시 예약 초기화 완료");
    }

    // ========== 선원 슬롯 관리 ==========

    /// <summary>
    /// 생존한 선원들의 슬롯 초기화 (Editor에 배치된 슬롯 재사용)
    /// ID 기반 매칭: Slot i는 CrewID i인 선원을 표시
    /// </summary>
    private void CreateCrewSlots()
    {
        crewItems.Clear();

        if (CrewManager.Instance == null)
        {
            Debug.LogError("[ResourceDistributionUI] CrewManager.Instance가 null입니다!");
            return;
        }

        // Editor에 미리 배치된 CrewResourceItem 슬롯들 찾기
        var existingCrews = crewsParent.GetComponentsInChildren<CrewResourceItem>(true);

        // 전체 선원 목록 가져오기
        var allCrews = CrewManager.Instance.CrewMembers;

        Debug.Log($"[ResourceDistributionUI] 전체 선원 {allCrews.Count}명, 기존 슬롯 {existingCrews.Length}개");

        // ID 기반 매칭: Slot i → CrewID가 i인 선원
        for (int i = 0; i < existingCrews.Length; i++)
        {
            // CrewID가 i인 선원 찾기
            var crew = allCrews.FirstOrDefault(c => c.CrewID == i);

            // 해당 선원이 존재하고 생존 중이면 슬롯 활성화
            if (crew != null && crew.IsAlive)
            {
                existingCrews[i].gameObject.SetActive(true);
                existingCrews[i].Initialize(crew);
                crewItems.Add(existingCrews[i]);
                Debug.Log($"[ResourceDistributionUI] Slot {i} → {crew.CrewName} (ID: {crew.CrewID})");
            }
            else
            {
                // 선원이 없거나 사망한 경우 슬롯 숨김
                existingCrews[i].gameObject.SetActive(false);

                if (crew != null && !crew.IsAlive)
                {
                    Debug.Log($"[ResourceDistributionUI] Slot {i} → 사망한 선원 {crew.CrewName} (숨김)");
                }
            }
        }

        Debug.Log($"[ResourceDistributionUI] 활성화된 선원 슬롯: {crewItems.Count}개");
    }

    // ========== 자원 슬롯 관리 ==========

    /// <summary>
    /// 인벤토리 슬롯 초기화 (동적 생성 - ResourceDatabase 기반)
    /// </summary>
    private void InitializeInventorySlots()
    {
        inventorySlots.Clear();

        // ResourceDatabase에서 모든 자원 목록 가져오기
        if (ResourceDatabaseManager.Instance == null || ResourceDatabaseManager.Instance.Database == null)
        {
            Debug.LogError("[ResourceDistributionUI] ResourceDatabaseManager를 찾을 수 없습니다!");
            return;
        }

        var allResources = ResourceDatabaseManager.Instance.Database.allResources;

        for (int i = 0; i < allResources.Count && i < boxElementParent.childCount; i++)
        {
            Transform slotTransform = boxElementParent.GetChild(i);
            var slot = slotTransform.GetComponent<InventorySlotUI>();

            if (slot == null)
            {
                slot = slotTransform.gameObject.AddComponent<InventorySlotUI>();
            }

            // ResourceMetaData에서 ResourceType 가져와서 초기화
            var resourceData = allResources[i];
            if (resourceData != null)
            {
                slot.Initialize(resourceData.resourceType);
                inventorySlots.Add(slot);
            }
        }

        Debug.Log($"[ResourceDistributionUI] 자원 슬롯 {inventorySlots.Count}개 초기화 완료 (데이터 기반)");
    }

    /// <summary>
    /// 특정 자원 슬롯의 수량 갱신
    /// </summary>
    public void RefreshResourceSlot(ResourceType type)
    {
        var slot = inventorySlots.Find(s => s.ResourceType == type);
        if (slot != null)
        {
            slot.UpdateAmount();
        }
    }

    /// <summary>
    /// 모든 자원 슬롯 갱신
    /// </summary>
    public void RefreshAllResourceSlots()
    {
        foreach (var slot in inventorySlots)
        {
            slot.UpdateAmount();
        }
    }

    /// <summary>
    /// 특정 자원 타입의 InventorySlot 가져오기
    /// </summary>
    public InventorySlotUI GetInventorySlot(ResourceType type)
    {
        return inventorySlots.Find(s => s.ResourceType == type);
    }

    // ========== 자원 아이콘 ==========

    /// <summary>
    /// 자원 타입에 맞는 아이콘 스프라이트 반환 (DivisionBoxSlot에서도 사용)
    /// ResourceDatabase에서 가져옴 (데이터 기반)
    /// </summary>
    public Sprite GetResourceIcon(ResourceType type)
    {
        if (ResourceDatabaseManager.Instance == null || ResourceDatabaseManager.Instance.Database == null)
        {
            Debug.LogWarning("[ResourceDistributionUI] ResourceDatabaseManager를 찾을 수 없습니다!");
            return null;
        }

        return ResourceDatabaseManager.Instance.Database.GetIcon(type);
    }

    // ========== 완료 버튼 ==========

    public void OnCompleteButtonClicked()
    {
        Debug.Log("[ResourceDistributionUI] 완료 버튼 클릭 - Evening 종료");

        // UI 닫기
        _distributeUI.Close();

        // Fade Out → DayManager에 위임 → Fade In
        if (FadeManager.Instance != null)
        {
            FadeManager.Instance.FadeOutToBlack(2.5f, () =>
            {
                // Fade Out 완료 후 DayManager에 Evening 완료 처리 위임
                if (DayManager.Instance != null)
                {
                    DayManager.Instance.CompleteEvening();
                }

                // Fade In
                FadeManager.Instance.FadeIn(3f);
            });
        }
    }

}
