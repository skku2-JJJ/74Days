using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// 모든 자원 데이터를 관리하는 데이터베이스 ScriptableObject
/// Unity Inspector에서 모든 ResourceMetaData를 등록하여 중앙 관리
/// </summary>
[CreateAssetMenu(fileName = "ResourceDatabase", menuName = "Game/Resource Database", order = 0)]
public class ResourceDatabase : ScriptableObject
{
    [Header("All Resources")]
    [Tooltip("게임에 존재하는 모든 자원 데이터 목록")]
    public List<ResourceMetaData> allResources = new List<ResourceMetaData>();

    // 런타임 캐시 (빠른 검색용)
    private Dictionary<ResourceType, ResourceMetaData> _resourceMap;

    /// <summary>
    /// Dictionary 초기화 (게임 시작 시 호출)
    /// </summary>
    public void Initialize()
    {
        _resourceMap = new Dictionary<ResourceType, ResourceMetaData>();

        foreach (var resourceData in allResources)
        {
            if (resourceData == null)
            {
                Debug.LogWarning("[ResourceDatabase] null ResourceMetaData 발견!");
                continue;
            }

            if (_resourceMap.ContainsKey(resourceData.resourceType))
            {
                Debug.LogWarning($"[ResourceDatabase] 중복된 ResourceType: {resourceData.resourceType}");
                continue;
            }

            _resourceMap[resourceData.resourceType] = resourceData;
        }

        Debug.Log($"[ResourceDatabase] 초기화 완료 - {_resourceMap.Count}개 자원 등록");
    }

    /// <summary>
    /// ResourceType으로 데이터 조회
    /// </summary>
    public ResourceMetaData GetData(ResourceType type)
    {
        if (_resourceMap == null)
        {
            Debug.LogWarning("[ResourceDatabase] 초기화되지 않음! Initialize() 호출 필요");
            Initialize();
        }

        if (_resourceMap.TryGetValue(type, out var data))
        {
            return data;
        }

        Debug.LogWarning($"[ResourceDatabase] {type}에 대한 데이터 없음!");
        return null;
    }

    /// <summary>
    /// 자원 아이콘 가져오기
    /// </summary>
    public Sprite GetIcon(ResourceType type)
    {
        var data = GetData(type);
        return data?.icon;
    }

    /// <summary>
    /// 자원 카테고리 가져오기
    /// </summary>
    public ResourceCategory GetCategory(ResourceType type)
    {
        var data = GetData(type);
        return data?.category ?? ResourceCategory.Food;
    }

    /// <summary>
    /// 자원 표시 이름 가져오기
    /// </summary>
    public string GetDisplayName(ResourceType type)
    {
        var data = GetData(type);
        return data?.displayName ?? type.ToString();
    }

    /// <summary>
    /// 특정 카테고리의 모든 자원 가져오기
    /// </summary>
    public List<ResourceMetaData> GetResourcesByCategory(ResourceCategory category)
    {
        return allResources.Where(r => r != null && r.category == category).ToList();
    }

    /// <summary>
    /// 초기 인벤토리 데이터 가져오기 (initialAmount > 0인 자원만)
    /// </summary>
    public Dictionary<ResourceType, int> GetInitialInventory()
    {
        var initialInventory = new Dictionary<ResourceType, int>();

        foreach (var resourceData in allResources)
        {
            if (resourceData != null && resourceData.initialAmount > 0)
            {
                initialInventory[resourceData.resourceType] = resourceData.initialAmount;
            }
        }

        return initialInventory;
    }
}
