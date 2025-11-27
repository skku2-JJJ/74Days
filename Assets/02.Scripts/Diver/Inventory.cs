using System.Collections.Generic;
using UnityEngine;

public class Inventory
{
    private readonly Dictionary<ResourceType, int> _items = new();

    public IReadOnlyDictionary<ResourceType, int> Items => _items;

    public void Add(ResourceType type, int amount = 1)
    {
        if (amount <= 0) return;

        if (_items.ContainsKey(type))
            _items[type] += amount;
        else
            _items[type] = amount;
    }

    public bool Consume(ResourceType type, int amount = 1)
    {
        if (amount <= 0) return false;

        if (_items.TryGetValue(type, out int current) && current >= amount)
        {
            _items[type] -= amount;
            return true;
        }

        return false;
    }

    public int GetAmount(ResourceType type)
    {
        return _items.TryGetValue(type, out int value) ? value : 0;
    }

    public void Clear()
    {
        _items.Clear();
    }

    /// <summary>
    /// 이 인벤토리 객체의 모든 아이템을 target으로 옮김.
    /// </summary>
    public void MoveAllTo(Inventory target)
    {
        foreach (var kvp in _items)
        {
            target.Add(kvp.Key, kvp.Value);
        }
        _items.Clear();
    }
}
