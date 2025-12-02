using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawnManager : MonoBehaviour
{
    [Header("참조")]
    [SerializeField] private FishSpawnManager _fishSpawnManager;

    [Header("장애물 프리팹")]
    [SerializeField] private GameObject[] _obstaclePrefabs;

    [Header("초기 스폰 개수")]
    [SerializeField] private int _initialShallowCount = 3;
    [SerializeField] private int _initialMiddleCount  = 4;
    [SerializeField] private int _initialDeepCount    = 3;

    [Header("전체 개수 제한")]
    [SerializeField] private int _maxTotalObstacles = 20;

    [Header("지역 밀집도 제한")]
    [SerializeField] private int   _maxObstaclesPerArea = 3;  
    [SerializeField] private float _crowdCheckRadius    = 3f; 
    [SerializeField] private LayerMask _obstacleLayer;        // 해파리 전용 레이어

    private readonly List<Obstacle> _spawned = new();

    private void Start()
    {
        SpawnInitialForZone(EOceanDepthZone.Shallow, _initialShallowCount);
        SpawnInitialForZone(EOceanDepthZone.Middle,  _initialMiddleCount);
        SpawnInitialForZone(EOceanDepthZone.Deep,    _initialDeepCount);
    }

    private void SpawnInitialForZone(EOceanDepthZone zone, int count)
    {
        if (count <= 0 || _obstaclePrefabs == null || _obstaclePrefabs.Length == 0)
            return;

        for (int i = 0; i < count; i++)
        {
            TrySpawnOne(zone);
        }
    }

    public bool TrySpawnOne(EOceanDepthZone zone)
    {
        CleanupNulls();
        if (_spawned.Count >= _maxTotalObstacles) return false;
        if (!_fishSpawnManager.TryGetSpawnPositionForObstacle(zone, out var pos))  return false;
        if (IsAreaTooCrowded(pos)) return false;
            

        // 실제 스폰
        GameObject obstaclePrefab = _obstaclePrefabs[Random.Range(0, _obstaclePrefabs.Length)];
        GameObject obstacleObj = Instantiate(obstaclePrefab, pos, Quaternion.identity);

        Obstacle obstacle = obstacleObj.GetComponent<Obstacle>();
        if (obstacle != null)
        {
            obstacle.SetOwner(this); 
            _spawned.Add(obstacle);
        }

        return true;
    }

    private bool IsAreaTooCrowded(Vector2 pos)
    {
        var hits = Physics2D.OverlapCircleAll(pos, _crowdCheckRadius, _obstacleLayer);
        int count = 0;

        foreach (var h in hits)
        {
            if (h.GetComponent<Obstacle>() != null)
                count++;
        }

        return count >= _maxObstaclesPerArea;
    }

    private void CleanupNulls()
    {
        // 파괴된 장애물 정리
        _spawned.RemoveAll(j => j == null);
    }

    // 장애물 사라질 때 호출
    public void NotifyJellyfishDestroyed(Obstacle jelly)
    {
        _spawned.Remove(jelly);
    }
}
