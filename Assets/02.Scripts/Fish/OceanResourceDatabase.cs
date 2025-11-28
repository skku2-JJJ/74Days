using System.Collections.Generic;
using UnityEngine;

public class OceanResourceDatabase : MonoBehaviour
{
    [SerializeField] private ResourceData[] _resources;

    private Dictionary<ResourceType, ResourceData> _lookup;

    private void Awake()
    {
        _lookup = new Dictionary<ResourceType, ResourceData>();
        foreach (var data in _resources)
        {
            if (data == null) continue;
            _lookup[data.Type] = data;
        }
    }

    public ResourceData Get(ResourceType type)
    {
        return _lookup.TryGetValue(type, out var data) ? data : null;
    }
}
