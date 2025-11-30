using System.Collections.Generic;
using UnityEngine;

public enum EOceanDepthZone
{
    Shallow,   
    Middle,    
    Deep       
}

[System.Serializable]
public class Entry
{
    public GameObject resourcePrefab;
    public float weight = 1f;
}

[CreateAssetMenu(menuName = "Resource/Resource Spawn Table")]
public class ResourceSpawnTable : ScriptableObject
{
    public EOceanDepthZone depthZone;
    
    public List<Entry> entries = new();

    /// <summary>
    /// 가중치 기반 랜덤으로 리소스 프리팹 하나 반환
    /// </summary>
    public GameObject GetRandomResourcePrefab()
    {
        if (entries == null || entries.Count == 0)
            return null;

        float total = 0f;
        foreach (var e in entries)
            total += Mathf.Max(e.weight, 0f);

        if (total <= 0f)
            return entries[0].resourcePrefab;

        float r = Random.value * total;
        foreach (var e in entries)
        {
            float w = Mathf.Max(e.weight, 0f);
            if (r <= w)
                return e.resourcePrefab;
            r -= w;
        }

        return entries[0].resourcePrefab;
    }
}