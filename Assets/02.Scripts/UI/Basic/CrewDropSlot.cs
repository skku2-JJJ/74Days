using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections.Generic;

public class CrewDropSlot : MonoBehaviour, IDropHandler
{
    [Header("Crew Info")]
    [SerializeField] private Image _crewImageUI;
    [SerializeField] private TextMeshProUGUI _crewNameTextUI;

    [Header("Distributed Items")]
    [SerializeField] private Transform _distributedItemsParentUI;
    [SerializeField] private GameObject _distributedItemPrefabUI;

    [Header("Resource Icons")]
    [SerializeField] private Sprite _fishIconUI;
    [SerializeField] private Sprite _shellfishIconUI;
    [SerializeField] private Sprite _seaweedIconUI;
    [SerializeField] private Sprite _waterIconUI;
    [SerializeField] private Sprite _herbsIconUI;
    [SerializeField] private Sprite _woodIconUI;

    private CrewMember _crew;
    private List<GameObject> _distributedItems = new List<GameObject>();

    public CrewMember Crew => _crew;

    public void Initialize(CrewMember crew)
    {
        _crew = crew;

        if (_crewNameTextUI != null)
        {
            _crewNameTextUI.text = crew.CrewName;
        }

        // 분배된 아이템 초기화
        ClearDistributedItems();
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (_crew == null || !_crew.IsAlive) return;

        // 드래그된 아이템 확인
        DraggableResourceItem draggedItem = eventData.pointerDrag?.GetComponent<DraggableResourceItem>();
        if (draggedItem == null) return;

        ResourceType resourceType = draggedItem.ResourceType;

        // 자원 분배 시도
        bool success = CrewManager.Instance.AssignResourceToCrew(_crew, resourceType, 1);

        if (success)
        {
            // 드래그 아이템에 성공 알림
            draggedItem.OnDropSuccess();

            // 분배된 아이템 표시
            AddDistributedItem(resourceType);

            // 자원 슬롯 갱신
            ResourceDistributionUI.Instance?.RefreshResourceSlot(resourceType);

            Debug.Log($"[드롭 성공] {_crew.CrewName}에게 {resourceType} 분배");
        }
        else
        {
            Debug.Log($"[드롭 실패] {_crew.CrewName}에게 {resourceType} 분배 실패");
        }
    }

    // 분배된 아이템 아이콘 추가
    private void AddDistributedItem(ResourceType type)
    {
        if (_distributedItemsParentUI == null || _distributedItemPrefabUI == null) return;

        GameObject itemObj = Instantiate(_distributedItemPrefabUI, _distributedItemsParentUI);
        Image itemImage = itemObj.GetComponent<Image>();

        if (itemImage != null)
        {
            itemImage.sprite = GetResourceIcon(type);
        }

        _distributedItems.Add(itemObj);
    }

    // 자원 타입에 따른 아이콘 반환
    private Sprite GetResourceIcon(ResourceType type)
    {
        return type switch
        {
            ResourceType.NormalFish => _fishIconUI,
            ResourceType.SpecialFish => _shellfishIconUI,
            ResourceType.Seaweed => _seaweedIconUI,
            ResourceType.CleanWater => _waterIconUI,
            ResourceType.Herbs => _herbsIconUI,
            ResourceType.Wood => _woodIconUI,
            _ => null
        };
    }

    // 분배된 아이템 초기화
    private void ClearDistributedItems()
    {
        foreach (var item in _distributedItems)
        {
            if (item != null)
            {
                Destroy(item);
            }
        }
        _distributedItems.Clear();
    }

    void OnDestroy()
    {
        ClearDistributedItems();
    }
}