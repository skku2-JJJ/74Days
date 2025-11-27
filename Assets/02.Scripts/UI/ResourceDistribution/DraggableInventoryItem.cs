using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// 드래그 가능한 자원 아이템
/// 인벤토리 슬롯에서 선원에게 드래그 앤 드롭
/// </summary>
[RequireComponent(typeof(CanvasGroup))]
public class DraggableInventoryItem : MonoBehaviour,
    IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public ResourceType ResourceType { get; private set; }

    private InventorySlotUI parentSlot;
    private Canvas canvas;
    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector2 originalPosition;

    [Header("Drag Settings")]
    [SerializeField] private float draggingAlpha = 0.6f;  // 드래그 중 투명도

    // ========== 초기화 ==========

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasGroup = GetComponent<CanvasGroup>();

        // Canvas 찾기
        canvas = GetComponentInParent<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("[DraggableInventoryItem] Canvas를 찾을 수 없습니다!");
        }

        // 초기 위치 저장
        originalPosition = rectTransform.anchoredPosition;
    }

    /// <summary>
    /// 드래그 아이템 초기화
    /// </summary>
    public void Initialize(ResourceType type, InventorySlotUI parent)
    {
        ResourceType = type;
        parentSlot = parent;
    }

    // ========== 드래그 이벤트 ==========

    /// <summary>
    /// 드래그 시작
    /// </summary>
    public void OnBeginDrag(PointerEventData eventData)
    {
        // 가용 수량 확인 (temporaryReserved 고려)
        if (parentSlot != null && parentSlot.AvailableAmount <= 0)
        {
            Debug.LogWarning($"[DraggableInventoryItem] {ResourceType} 가용 수량이 0입니다!");
            return;
        }

        Debug.Log($"[DraggableInventoryItem] {ResourceType} 드래그 시작");

        // 투명도 낮춤
        if (canvasGroup != null)
        {
            canvasGroup.alpha = draggingAlpha;
            canvasGroup.blocksRaycasts = false;  // 드롭 감지를 위해 레이캐스트 차단 해제
        }
    }

    /// <summary>
    /// 드래그 중
    /// </summary>
    public void OnDrag(PointerEventData eventData)
    {
        if (canvas == null) return;

        // 마우스 위치 따라다니기
        rectTransform.anchoredPosition += eventData.delta / canvas.scaleFactor;
    }

    /// <summary>
    /// 드래그 종료
    /// </summary>
    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log($"[DraggableInventoryItem] {ResourceType} 드래그 종료");

        // 원위치로 복귀
        rectTransform.anchoredPosition = originalPosition;

        // 투명도 복구
        if (canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
        }
    }

}
