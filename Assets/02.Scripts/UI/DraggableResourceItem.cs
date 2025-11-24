using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DraggableResourceItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private Image _iconImageUI;
    [SerializeField] private CanvasGroup _canvasGroupUI;

    private ResourceType _resourceType;
    private ResourceSlotItem _parentSlot;
    private Transform _originalParent;
    private Vector3 _originalPosition;
    private Canvas _canvas;

    public ResourceType ResourceType => _resourceType;

    public void Initialize(ResourceType type, Sprite icon, ResourceSlotItem parentSlot)
    {
        _resourceType = type;
        _parentSlot = parentSlot;

        if (_iconImageUI != null && icon != null)
        {
            _iconImageUI.sprite = icon;
        }

        // Canvas 찾기 (드래그 시 위치 계산용)
        _canvas = GetComponentInParent<Canvas>();

        // CanvasGroup 없으면 추가
        if (_canvasGroupUI == null)
        {
            _canvasGroupUI = gameObject.AddComponent<CanvasGroup>();
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // 수량 체크
        if (_parentSlot != null && _parentSlot.CurrentAmount <= 0)
        {
            eventData.pointerDrag = null;
            return;
        }

        _originalParent = transform.parent;
        _originalPosition = transform.position;

        // 최상위로 이동 (다른 UI 위에 표시)
        transform.SetParent(_canvas.transform);
        transform.SetAsLastSibling();

        // Raycast 차단 해제 (드롭 영역 감지 위해)
        _canvasGroupUI.blocksRaycasts = false;
        _canvasGroupUI.alpha = 0.7f;

        Debug.Log($"[드래그 시작] {_resourceType}");
    }

    public void OnDrag(PointerEventData eventData)
    {
        // 마우스 따라다니기
        transform.position = eventData.position;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Raycast 복구
        _canvasGroupUI.blocksRaycasts = true;
        _canvasGroupUI.alpha = 1f;

        // 원위치로 복귀 (드롭 성공 시 CrewDropSlot에서 처리)
        transform.SetParent(_originalParent);
        transform.position = _originalPosition;

        Debug.Log($"[드래그 종료] {_resourceType}");
    }

    // 드롭 성공 시 호출 (CrewDropSlot에서 호출)
    public void OnDropSuccess()
    {
        // 부모 슬롯의 수량 감소
        if (_parentSlot != null)
        {
            _parentSlot.DecreaseAmount();
        }
    }
}