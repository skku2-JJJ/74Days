using System.Collections.Generic;
using UnityEngine;

public class ItemSpawnManager : MonoBehaviour
{
    [Header("Spawn Points")]
    private Transform[] _spawnPoints; 

    [Header("Spawn Table")]
    [SerializeField] private ResourceSpawnTable _dropTable;

    [Header("옵션")]
    [Range(0f, 1f)]
    [SerializeField] private float _spawnChancePerPoint = 1f; // 각 포인트별 생성 확률 (1이면 100%)

    private readonly List<Transform> _points = new();

    private void Awake()
    {
        _spawnPoints = GetComponentsInChildren<Transform>();
        
        for(int i = 0; i<_spawnPoints.Length; i++)
        {
            if(i==0) continue;
            
            _points.Add(_spawnPoints[i]);
        }
    }

    private void Start()
    {
        SpawnOnce();
    }

    private void SpawnOnce()
    {
        foreach (Transform p in _points)
        {
            if (p == null) continue;
            
            if (Random.value > _spawnChancePerPoint)
                continue;

            var prefab = _dropTable.GetRandomResourcePrefab();
            if (prefab == null) continue;

            Instantiate(prefab, p.position, Quaternion.identity);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (_spawnPoints == null) return;
        
        Gizmos.color = Color.yellow;
        foreach (Transform child in _spawnPoints)
        {
            if (child == null) continue;
            Gizmos.DrawSphere(child.position, 0.1f);
        }
    }
}
