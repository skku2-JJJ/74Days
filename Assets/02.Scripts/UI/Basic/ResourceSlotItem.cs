using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ResourceSlotItem : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Image _iconImageUI;
    [SerializeField] private TextMeshProUGUI _countTextUI;
    [SerializeField] private TextMeshProUGUI _nameTextUI;

    [Header("Draggable Item")]
    [SerializeField] private DraggableResourceItem _draggableItemUI;

    [Header("Resource Icons")]
    [SerializeField] private Sprite _fishIconUI;
    [SerializeField] private Sprite _shellfishIconUI;
    [SerializeField] private Sprite _seaweedIconUI;
    [SerializeField] private Sprite _waterIconUI;
    [SerializeField] private Sprite _herbsIconUI;
    [SerializeField] private Sprite _woodIconUI;

    private ResourceType _resourceType;
    private int _currentAmount;

    public ResourceType ResourceType => _resourceType;
    public int CurrentAmount => _currentAmount;

    public void Initialize(ResourceType type, string displayName, int amount)
    {
        _resourceType = type;
        _currentAmount = amount;

        // 아이콘 설정
        Sprite icon = GetResourceIcon(type);
        if (_iconImageUI != null && icon != null)
        {
            _iconImageUI.sprite = icon;
        }

        // 이름 설정
        if (_nameTextUI != null)
        {
            _nameTextUI.text = displayName;
        }

        // 수량 설정
        UpdateAmountDisplay();

        // 드래그 아이템 초기화
        if (_draggableItemUI != null)
        {
            _draggableItemUI.Initialize(type, icon, this);
        }
    }

    // 수량 갱신
    public void UpdateAmount(int newAmount)
    {
        _currentAmount = newAmount;
        UpdateAmountDisplay();
    }

    // 수량 감소
    public void DecreaseAmount()
    {
        _currentAmount--;
        UpdateAmountDisplay();
    }

    // 수량 표시 갱신
    private void UpdateAmountDisplay()
    {
        if (_countTextUI != null)
        {
            _countTextUI.text = $"x{_currentAmount}";
        }

        // 수량 0이면 비활성화
        if (_draggableItemUI != null)
        {
            _draggableItemUI.gameObject.SetActive(_currentAmount > 0);
        }

        // 전체 슬롯 투명도 조절
        CanvasGroup canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup != null)
        {
            canvasGroup.alpha = _currentAmount > 0 ? 1f : 0.5f;
        }
    }

    // 자원 타입에 따른 아이콘 반환
    private Sprite GetResourceIcon(ResourceType type)
    {
        return type switch
        {
            ResourceType.BlowFish => _fishIconUI,
            ResourceType.DameselFish => _shellfishIconUI,
            ResourceType.EmeraldFish => _seaweedIconUI,
            ResourceType.BlueTang => _waterIconUI,
            ResourceType.FileFish => _herbsIconUI,
            ResourceType.TinyFish => _woodIconUI,
            _ => null
        };
    }
}