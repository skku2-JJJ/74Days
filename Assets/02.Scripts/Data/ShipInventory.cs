using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class ShipInventory
{
    private Inventory _inventory = new Inventory();
    private bool _isInitialized = false;

    /// <summary>
    /// 모든 자원 (읽기 전용)
    /// </summary>
    public IReadOnlyDictionary<ResourceType, int> Items => _inventory.Items;

    /// <summary>
    /// ResourceDatabase에서 초기값을 로드하여 인벤토리 초기화
    /// </summary>
    public void Initialize(ResourceDatabase database)
    {
        if (_isInitialized)
        {
            Debug.LogWarning("[ShipInventory] 이미 초기화되었습니다!");
            return;
        }

        if (database == null)
        {
            Debug.LogError("[ShipInventory] ResourceDatabase가 null입니다!");
            return;
        }

        // ResourceDatabase에서 초기 인벤토리 로드
        var initialInventory = database.GetInitialInventory();

        foreach (var kvp in initialInventory)
        {
            _inventory.Add(kvp.Key, kvp.Value);
        }

        _isInitialized = true;
        Debug.Log($"[ShipInventory] 초기화 완료 - {_inventory.Items.Count}종류 자원");
    }

    // ========== 자원 관리 ==========

    /// <summary>
    /// 자원 추가
    /// </summary>
    public void AddResource(ResourceType type, int amount)
    {
        _inventory.Add(type, amount);
        Debug.Log($"[ShipInventory] AddResource 후 - {type}: {_inventory.GetAmount(type)}개, 전체 아이템 수: {_inventory.Items.Count}");
    }

    /// <summary>
    /// 자원 소비
    /// </summary>
    public bool ConsumeResource(ResourceType type, int amount)
    {
        return _inventory.Consume(type, amount);
    }

    /// <summary>
    /// 자원 양 확인
    /// </summary>
    public int GetResourceAmount(ResourceType type)
    {
        int amount = _inventory.GetAmount(type);
        Debug.Log($"[ShipInventory] GetResourceAmount - {type}: {amount}개, 전체 아이템 수: {_inventory.Items.Count}");
        return amount;
    }

    // ========== 자원 카테고리별 합계 ==========

    /// <summary>
    /// 총 물고기 수 (모든 물고기 합산)
    /// </summary>
    public int TotalFish
    {
        get
        {
            int total = 0;
            foreach (var item in _inventory.Items)
            {
                total += item.Value;
            }
            return total;
        }
    }

    /// <summary>
    /// 총 식량 (TotalFish와 동일, 기존 코드 호환성)
    /// </summary>
    public int TotalFood => TotalFish;

    /// <summary>
    /// 총 의약품 
    /// </summary>
    public int TotalMedicine => 0;

    /// <summary>
    /// 총 수리 재료
    /// </summary>
    public int TotalRepairMaterials => 0;
    
}
