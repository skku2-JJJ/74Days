using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Unity 직렬화 가능한 자원 인벤토리 클래스
/// Dictionary는 Unity가 직렬화할 수 없으므로 List로 변환하여 저장
/// </summary>
[System.Serializable]
public class Inventory
{
    // Unity 직렬화 가능한 필드 (Inspector에서 보임, 저장 시 유지됨)
    [SerializeField] private List<ResourceType> _keys = new List<ResourceType>();
    [SerializeField] private List<int> _values = new List<int>();

    // 런타임 Dictionary (빠른 접근용, 직렬화 안 됨)
    private Dictionary<ResourceType, int> _items;

    /// <summary>
    /// 모든 아이템 (읽기 전용)
    /// </summary>
    public IReadOnlyDictionary<ResourceType, int> Items
    {
        get
        {
            EnsureInitialized();
            return _items;
        }
    }

    /// <summary>
    /// Dictionary 초기화 (직렬화된 List에서 복원)
    /// Unity가 역직렬화 후 Dictionary를 복원하기 위해 필요
    /// </summary>
    private void EnsureInitialized()
    {
        if (_items == null)
        {
            _items = new Dictionary<ResourceType, int>();

            // 직렬화된 List에서 Dictionary 복원
            for (int i = 0; i < _keys.Count && i < _values.Count; i++)
            {
                _items[_keys[i]] = _values[i];
            }
        }
    }

    /// <summary>
    /// Dictionary를 List로 동기화 (직렬화 준비)
    /// Unity가 저장할 수 있도록 Dictionary 내용을 List로 변환
    /// </summary>
    private void SyncToLists()
    {
        _keys.Clear();
        _values.Clear();

        foreach (var kvp in _items)
        {
            _keys.Add(kvp.Key);
            _values.Add(kvp.Value);
        }
    }

    /// <summary>
    /// 자원 추가
    /// </summary>
    public void Add(ResourceType type, int amount = 1)
    {
        if (amount <= 0) return;

        EnsureInitialized();

        if (_items.ContainsKey(type))
            _items[type] += amount;
        else
            _items[type] = amount;

        SyncToLists();
    }

    /// <summary>
    /// 자원 소비
    /// </summary>
    public bool Consume(ResourceType type, int amount = 1)
    {
        if (amount <= 0) return false;

        EnsureInitialized();

        if (_items.TryGetValue(type, out int current) && current >= amount)
        {
            _items[type] -= amount;

            // 0이 되면 Dictionary에서 제거 (선택 사항)
            if (_items[type] == 0)
            {
                _items.Remove(type);
            }

            SyncToLists();
            return true;
        }

        return false;
    }

    /// <summary>
    /// 자원 양 확인
    /// </summary>
    public int GetAmount(ResourceType type)
    {
        EnsureInitialized();
        return _items.TryGetValue(type, out int value) ? value : 0;
    }

    /// <summary>
    /// 모든 자원 제거
    /// </summary>
    public void Clear()
    {
        EnsureInitialized();
        _items.Clear();
        SyncToLists();
    }

    /// <summary>
    /// 이 인벤토리의 모든 아이템을 target Dictionary로 옮김
    /// </summary>
    public void MoveAllTo(Dictionary<ResourceType, int> target)
    {
        EnsureInitialized();

        foreach (var kvp in _items)
        {
            if (target.ContainsKey(kvp.Key))
                target[kvp.Key] += kvp.Value;
            else
                target[kvp.Key] = kvp.Value;
        }

        _items.Clear();
        SyncToLists();
    }

    /// <summary>
    /// 다른 Inventory에서 모든 아이템을 가져옴 (복사)
    /// </summary>
    public void CopyFrom(Inventory source)
    {
        if (source == null) return;

        EnsureInitialized();

        foreach (var item in source.Items)
        {
            if (_items.ContainsKey(item.Key))
                _items[item.Key] += item.Value;
            else
                _items[item.Key] = item.Value;
        }

        SyncToLists();
    }

    /// <summary>
    /// 디버그 출력
    /// </summary>
    public void Print()
    {
        EnsureInitialized();

        if (_items.Count == 0)
        {
            Debug.Log("[Inventory] 비어있음");
            return;
        }

        Debug.Log($"[Inventory] 총 {_items.Count}종류:");
        foreach (var kvp in _items)
        {
            Debug.Log($"  {kvp.Key}: {kvp.Value}개");
        }
    }
}
