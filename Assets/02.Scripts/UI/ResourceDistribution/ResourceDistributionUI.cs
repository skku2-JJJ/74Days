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
    [SerializeField] private Button divisionButton;             // DivisionButton (완료 버튼)
    [SerializeField] private TextMeshProUGUI titleText;         // TitleText (제목)
    
    [Header("Resource Icons")]
    [SerializeField] private Sprite normalFishIcon;
    [SerializeField] private Sprite specialFishIcon;
    [SerializeField] private Sprite seaweedIcon;
    [SerializeField] private Sprite cleanWaterIcon;
    [SerializeField] private Sprite herbsIcon;

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
        if (divisionButton != null)
        {
            divisionButton.gameObject.SetActive(false);
        }

        // Evening 페이즈 구독
        if (DayManager.Instance != null)
        {
            DayManager.Instance.OnPhaseChange += OnPhaseChanged;
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
    }

    // ========== 페이즈 변경 이벤트 ==========

    private void OnPhaseChanged(DayPhase phase)
    {
        if (phase == DayPhase.Evening)
        {
            Debug.Log("[ResourceDistributionUI] Evening 페이즈 진입 - UI 표시");
            ShowUI();

            // DivisionButton 활성화
            if (divisionButton != null)
            {
                divisionButton.gameObject.SetActive(true);
            }
        }
        else
        {
            // Evening이 아닌 페이즈에서는 DivisionButton 숨김
            if (divisionButton != null)
            {
                divisionButton.gameObject.SetActive(false);
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

        // 생존한 선원만 표시
        var aliveCrews = CrewManager.Instance.CrewMembers
            .Where(c => c.IsAlive)
            .ToList();

        Debug.Log($"[ResourceDistributionUI] 생존 선원 {aliveCrews.Count}명, 기존 슬롯 {existingCrews.Length}개");

        // 생존 선원 수만큼 슬롯 활성화 및 초기화
        for (int i = 0; i < existingCrews.Length; i++)
        {
            if (i < aliveCrews.Count)
            {
                // 슬롯 활성화 및 데이터 초기화
                existingCrews[i].gameObject.SetActive(true);
                existingCrews[i].Initialize(aliveCrews[i]);
                crewItems.Add(existingCrews[i]);
            }
            else
            {
                // 남은 슬롯은 숨김
                existingCrews[i].gameObject.SetActive(false);
            }
        }

        // 선원이 슬롯보다 많으면 경고
        if (aliveCrews.Count > existingCrews.Length)
        {
            Debug.LogWarning($"[ResourceDistributionUI] 생존 선원({aliveCrews.Count})이 슬롯({existingCrews.Length})보다 많습니다!");
        }
    }

    // ========== 자원 슬롯 관리 ==========

    /// <summary>
    /// 인벤토리 슬롯 초기화 (InventorySlot0~9)
    /// </summary>
    private void InitializeInventorySlots()
    {
        inventorySlots.Clear();

        ResourceType[] resources = new ResourceType[]
        {
            ResourceType.BlowFish,
            ResourceType.DameselFish,
            ResourceType.EmeraldFish,
            ResourceType.BlueTang,
            ResourceType.FileFish
        };

        for (int i = 0; i < resources.Length && i < boxElementParent.childCount; i++)
        {
            Transform slotTransform = boxElementParent.GetChild(i);
            var slot = slotTransform.GetComponent<InventorySlotUI>();

            if (slot == null)
            {
                slot = slotTransform.gameObject.AddComponent<InventorySlotUI>();
            }

            // 아이콘은 Unity Editor에서 미리 할당됨
            slot.Initialize(resources[i]);
            inventorySlots.Add(slot);
        }

        Debug.Log($"[ResourceDistributionUI] 자원 슬롯 {inventorySlots.Count}개 초기화 완료");
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
    /// </summary>
    public Sprite GetResourceIcon(ResourceType type)
    {
        switch (type)
        {
            case ResourceType.BlowFish:
                return normalFishIcon;
            case ResourceType.DameselFish:
                return specialFishIcon;
            case ResourceType.EmeraldFish:
                return seaweedIcon;
            case ResourceType.BlueTang:
                return cleanWaterIcon;
            case ResourceType.FileFish:
                return herbsIcon;
            default:
                return null;
        }
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
