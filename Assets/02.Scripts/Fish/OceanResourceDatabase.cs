using System.Collections.Generic;
using UnityEngine;

public class OceanResourceDatabase : MonoBehaviour
{
    [SerializeField] private ResourceMetaData[] _resources;

    private Dictionary<ResourceType, ResourceMetaData> _lookup;

    private void Awake()
    {
        _lookup = new Dictionary<ResourceType, ResourceMetaData>();
        foreach (var data in _resources)
        {
            if (data == null) continue;
            _lookup[data.resourceType] = data;
        }
    }

    public ResourceMetaData Get(ResourceType type)
    {
        return _lookup.TryGetValue(type, out var data) ? data : null;
    }
}
