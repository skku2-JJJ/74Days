using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// 자원 슬롯 UI 컴포넌트
/// 자원 아이콘, 수량 표시 및 드래그 가능한 아이템 관리
/// </summary>
public class InventorySlotUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image iconImage;                // 자원 아이콘 이미지
    [SerializeField] private TextMeshProUGUI amountText;     // 자원 수량 텍스트
    [SerializeField] private DraggableInventoryItem draggableItem;  // 드래그 가능한 아이템

    [Header("Display Settings")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color emptyColor = new Color(1f, 1f, 1f, 0.3f);  // 투명도 30%

    public ResourceType ResourceType { get; private set; }
    private int currentAmount;
    private int temporaryReserved = 0;  // 할당 예약된 개수 (UI 표시용)

    // ========== 초기화 ==========

    /// <summary>
    /// 슬롯 초기화
    /// </summary>
    public void Initialize(ResourceType type)
    {
        ResourceType = type;

        // 아이콘은 Unity Editor에서 미리 할당됨

        // 드래그 아이템 초기화
        if (draggableItem == null)
        {
            draggableItem = GetComponentInChildren<DraggableInventoryItem>();
        }

        if (draggableItem != null)
        {
            draggableItem.Initialize(type, this);
        }

        // 수량 갱신
        UpdateAmount();

        Debug.Log($"[InventorySlotUI] {type} 슬롯 초기화 완료");
    }

    // ========== 수량 관리 ==========

    /// <summary>
    /// 배 인벤토리에서 현재 자원 수량 가져와서 UI 갱신
    /// </summary>
    public void UpdateAmount()
    {
        if (ShipManager.Instance == null)
        {
            Debug.LogWarning("[InventorySlotUI] ShipManager.Instance가 null입니다!");
            return;
        }

        currentAmount = ShipManager.Instance.GetResourceAmount(ResourceType);

        // 수량 텍스트 갱신 (실제 수량 - 예약된 수량)
        int displayAmount = currentAmount - temporaryReserved;
        if (amountText != null)
        {
            amountText.text = displayAmount.ToString();
        }

        // 수량에 따라 비활성화 처리
        UpdateVisualState();
    }

    /// <summary>
    /// 수량에 따라 비주얼 상태 변경
    /// </summary>
    private void UpdateVisualState()
    {
        int availableAmount = currentAmount - temporaryReserved;
        bool hasItems = availableAmount > 0;

        // 드래그 가능 여부 설정
        if (draggableItem != null)
        {
            draggableItem.enabled = hasItems;
        }

        // 아이콘 투명도 조정
        if (iconImage != null)
        {
            iconImage.color = hasItems ? normalColor : emptyColor;
        }

        // 수량 텍스트 색상 조정
        if (amountText != null)
        {
            amountText.color = hasItems ? Color.white : new Color(1f, 1f, 1f, 0.5f);
        }
    }

    // ========== 아이템 사용 ==========

    /// <summary>
    /// 드롭 성공 시 호출 (DraggableInventoryItem에서 호출)
    /// </summary>
    public void OnItemUsed()
    {
        UpdateAmount();
        Debug.Log($"[InventorySlotUI] {ResourceType} 사용 - 남은 수량: {currentAmount}");
    }

    // ========== 임시 예약 관리 ==========

    /// <summary>
    /// 자원 할당 예약 (DivisionBox에 드롭 시)
    /// </summary>
    public void ReserveResource()
    {
        temporaryReserved++;
        UpdateAmount();
        Debug.Log($"[InventorySlotUI] {ResourceType} 예약 +1 (총 예약: {temporaryReserved})");
    }

    /// <summary>
    /// 자원 할당 예약 취소 (DivisionBox에서 제거 시)
    /// </summary>
    public void ReleaseResource()
    {
        if (temporaryReserved > 0)
        {
            temporaryReserved--;
            UpdateAmount();
            Debug.Log($"[InventorySlotUI] {ResourceType} 예약 -1 (총 예약: {temporaryReserved})");
        }
    }

    /// <summary>
    /// 모든 임시 예약 초기화 (새로운 Evening 시작 시)
    /// </summary>
    public void ResetTemporaryReservations()
    {
        temporaryReserved = 0;
        UpdateAmount();
    }

    // ========== 게터 ==========

    public int CurrentAmount => currentAmount;
    public int AvailableAmount => currentAmount - temporaryReserved;
}
