using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// DivisionBox 슬롯 (Hunger/Thirst/Temperature 카테고리별)
/// 각 선원마다 3개의 박스가 있으며, 카테고리에 맞는 자원만 드롭 가능
/// 한 박스당 1개의 자원만 할당 가능
/// 우클릭으로 자원 제거 가능
/// </summary>
public class DivisionBoxSlot : MonoBehaviour, IDropHandler, IPointerClickHandler
{
    public enum BoxType { Hunger, Thirst, Temperature }

    [Header("Box Settings")]
    [SerializeField] private BoxType boxType;

    [Header("UI References")]
    [SerializeField] private Image iconImage;           // 할당된 자원 아이콘
    [SerializeField] private Image backgroundImage;     // 박스 배경 (옵션)

    private CrewMember assignedCrew;
    private ResourceType? assignedResource = null;      // 현재 할당된 자원 (null = 비어있음)

    // ========== 초기화 ==========

    /// <summary>
    /// DivisionBox 초기화
    /// </summary>
    public void Initialize(CrewMember crew)
    {
        assignedCrew = crew;
        ClearResource();

        Debug.Log($"[DivisionBoxSlot] {boxType} 박스 초기화 - {crew.CrewName}");
    }

    // ========== 드롭 처리 ==========

    /// <summary>
    /// 자원이 드롭되었을 때 처리 (할당만 기록, 실제 적용은 완료 버튼에서)
    /// </summary>
    public void OnDrop(PointerEventData eventData)
    {
        if (assignedCrew == null || !assignedCrew.IsAlive)
        {
            Debug.LogWarning("[DivisionBoxSlot] 사망한 선원에게는 자원을 줄 수 없습니다!");
            return;
        }

        // 드래그된 아이템 확인
        var draggable = eventData.pointerDrag?.GetComponent<DraggableInventoryItem>();
        if (draggable == null)
        {
            Debug.LogWarning("[DivisionBoxSlot] 유효하지 않은 아이템입니다!");
            return;
        }

        ResourceType resourceType = draggable.ResourceType;

        // 카테고리 검증
        if (!IsValidResourceForBox(resourceType))
        {
            Debug.LogWarning($"[DivisionBoxSlot] {boxType} 박스에는 {resourceType}를 넣을 수 없습니다!");
            return;
        }

        // 이미 자원이 할당되어 있는지 확인
        if (assignedResource.HasValue)
        {
            Debug.LogWarning($"[DivisionBoxSlot] {assignedCrew.CrewName}의 {boxType} 박스에 이미 {assignedResource.Value}가 할당되어 있습니다!");
            return;
        }

        // 인벤토리에 자원이 있는지 확인
        if (ShipManager.Instance.GetResourceAmount(resourceType) <= 0)
        {
            Debug.LogWarning($"[DivisionBoxSlot] {resourceType} 자원이 부족합니다!");
            return;
        }

        // 자원 할당 기록 (아직 실제 소비는 안 함)
        AssignResource(resourceType);

        // InventorySlot에 예약 알림
        NotifyInventoryReservation(resourceType, true);

        Debug.Log($"[DivisionBoxSlot] {assignedCrew.CrewName}의 {boxType} 박스에 {resourceType} 할당 예약");
    }

    /// <summary>
    /// 우클릭으로 자원 제거
    /// </summary>
    public void OnPointerClick(PointerEventData eventData)
    {
        Debug.Log($"[DivisionBoxSlot] OnPointerClick 호출 - 버튼: {eventData.button}");

        // 우클릭 확인
        if (eventData.button != PointerEventData.InputButton.Right)
        {
            Debug.Log("[DivisionBoxSlot] 우클릭이 아님");
            return;
        }

        // 할당된 자원이 있는지 확인
        if (!assignedResource.HasValue)
        {
            Debug.Log("[DivisionBoxSlot] 제거할 자원이 없습니다.");
            return;
        }

        ResourceType removedResource = assignedResource.Value;
        Debug.Log($"[DivisionBoxSlot] 자원 제거 시작 - {removedResource}");

        // InventorySlot에 예약 취소 알림
        NotifyInventoryReservation(removedResource, false);

        // 자원 제거
        ClearResource();

        Debug.Log($"[DivisionBoxSlot] {assignedCrew.CrewName}의 {boxType} 박스에서 {removedResource} 제거 완료");
    }

    // ========== 자원 검증 ==========

    /// <summary>
    /// 해당 박스에 유효한 자원인지 확인
    /// </summary>
    private bool IsValidResourceForBox(ResourceType type)
    {
        switch (boxType)
        {
            case BoxType.Hunger:
                return type == ResourceType.BlowFish ||
                       type == ResourceType.BlueTang ||
                       type == ResourceType.EmeraldFish ||
                       type == ResourceType.Nemo ||
                       type == ResourceType.SawShark ||
                       type == ResourceType.StripedMarlin ||
                       type == ResourceType.Turtle ||
                       type == ResourceType.Grouper ||
                       type == ResourceType.Attack1 ||
                       type == ResourceType.Attack2;

            case BoxType.Thirst:
                return type == ResourceType.Water;

            case BoxType.Temperature:
                return type == ResourceType.Herb;

            default:
                return false;
        }
    }

    // ========== 자원 관리 ==========

    /// <summary>
    /// 자원 할당 및 아이콘 표시
    /// </summary>
    private void AssignResource(ResourceType type)
    {
        assignedResource = type;

        // 아이콘 표시
        if (iconImage != null && ResourceDistributionUI.Instance != null)
        {
            iconImage.sprite = ResourceDistributionUI.Instance.GetResourceIcon(type);
            iconImage.enabled = true;
            iconImage.color = Color.white;
        }

        Debug.Log($"[DivisionBoxSlot] {boxType} 박스에 {type} 아이콘 표시");
    }

    /// <summary>
    /// 자원 초기화 (새로운 Evening 시작 시 호출)
    /// </summary>
    public void ClearResource()
    {
        Debug.Log($"[DivisionBoxSlot] ClearResource 호출 - 현재 자원: {assignedResource}");

        assignedResource = null;

        // 아이콘 숨김
        if (iconImage != null)
        {
            iconImage.sprite = null;
            iconImage.enabled = false;
            Debug.Log($"[DivisionBoxSlot] 아이콘 숨김 완료 - enabled: {iconImage.enabled}");
        }
        else
        {
            Debug.LogWarning("[DivisionBoxSlot] iconImage가 null입니다!");
        }
    }

    // ========== InventorySlot 연동 ==========

    /// <summary>
    /// InventorySlot에 예약/취소 알림
    /// </summary>
    private void NotifyInventoryReservation(ResourceType type, bool isReserve)
    {
        if (ResourceDistributionUI.Instance == null)
            return;

        var inventorySlot = ResourceDistributionUI.Instance.GetInventorySlot(type);
        if (inventorySlot != null)
        {
            if (isReserve)
                inventorySlot.ReserveResource();
            else
                inventorySlot.ReleaseResource();
        }
    }

    // ========== 게터 ==========

    public bool HasResource => assignedResource.HasValue;
    public ResourceType? AssignedResource => assignedResource;
    public CrewMember GetAssignedCrew() => assignedCrew;
}
