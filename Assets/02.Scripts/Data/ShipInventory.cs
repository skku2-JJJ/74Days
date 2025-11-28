using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class ShipInventory
{
    private Inventory _inventory = new Inventory();

    /// <summary>
    /// 모든 자원 (읽기 전용)
    /// </summary>
    public IReadOnlyDictionary<ResourceType, int> Items => _inventory.Items;

    // 생성자에서 초기값 설정
    public ShipInventory()
    {
        // 초기화
        _inventory.Add(ResourceType.BlowFish, 5);
        _inventory.Add(ResourceType.BlueTang, 3);
        _inventory.Add(ResourceType.DameselFish, 2);
        _inventory.Add(ResourceType.EmeraldFish, 5);
        _inventory.Add(ResourceType.FileFish, 3);
        _inventory.Add(ResourceType.TinyFish, 2);
        _inventory.Add(ResourceType.Nemo, 5);
        _inventory.Add(ResourceType.PinkFish, 3);
        _inventory.Add(ResourceType.SawShark, 2);
        _inventory.Add(ResourceType.StripedMarlin, 5);
        _inventory.Add(ResourceType.Turtle, 3);
        _inventory.Add(ResourceType.Grouper, 2);
        _inventory.Add(ResourceType.Water, 5);
        _inventory.Add(ResourceType.Wood, 3);
    }

    // ========== 자원 관리 ==========

    /// <summary>
    /// 자원 추가
    /// </summary>
    public void AddResource(ResourceType type, int amount)
    {
        _inventory.Add(type, amount);
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
        return _inventory.GetAmount(type);
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
