using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpeciesLimit
{
    public ResourceType fishType;    
    public int maxCount = 10;    
}

public class FishSpawnManager : MonoBehaviour
{
    private static FishSpawnManager instance;
    
    [Header("전체 스폰 구역")]
    [Tooltip("전체 수역의 왼쪽 아래 월드 좌표")]
    [SerializeField] private Vector2 spawnAreaMin = new Vector2(-50f, -80f);
    [Tooltip("전체 수역의 오른쪽 위 월드 좌표")]
    [SerializeField] private Vector2 spawnAreaMax = new Vector2(50f, 10f);
    
    [Header("Depth Zones (World Y)")]
    [Tooltip("얕은 구역의 최대 Y (이 위쪽이 Shallow)")]
    [SerializeField] private float shallowMaxY = 0f;
    [Tooltip("중간 구역의 최대 Y (이 아래부터 Deep)")]
    [SerializeField] private float middleMaxY = -40f;
    
    [Header("Spawn Tables")]
    [SerializeField] private ResourceSpawnTable shallowTable;
    [SerializeField] private ResourceSpawnTable middleTable;
    [SerializeField] private ResourceSpawnTable deepTable;

    [Header("Initial Spawn Counts")]
    [SerializeField] private int initialShallowCount = 12;
    [SerializeField] private int initialMiddleCount = 12;
    [SerializeField] private int initialDeepCount = 12;

    [Header("런타임 스폰 Setting")]
    [SerializeField] private float spawnInterval = 2f;      // 몇 초마다 스폰 시도할지
    [SerializeField] private int maxTotalFishCount = 40;    // 전체 최대 물고기 수
    [SerializeField] private Transform player;              // 플레이어 Transform

    [Header("디스폰 설정")]
    [SerializeField] private float despawnDistanceFromCamera = 70f; // 카메라로부터 이 이상 떨어지면 디스폰
    
    [Header("Collision & Density Check")]
    [SerializeField] private float collisionCheckRadius = 0.5f;
    [SerializeField] private LayerMask groundLayer;         // 지형 레이어
    [Tooltip("밀도 체크 반경 (반경 안에 너무 많으면 스폰 안 함)")]
    [SerializeField] private float densityRadius = 3f;
    [Tooltip("densityRadius 반경 안 허용 최대 물고기 수")]
    [SerializeField] private int maxFishPerArea = 5;
    [SerializeField] private LayerMask fishLayer;           // 물고기 레이어

    
   
    [SerializeField] private List<SpeciesLimit> speciesLimits = new();

    private readonly List<FishBase> _activeFish = new();
    private float _timer;

    public static FishSpawnManager Instance { get => instance; private set => instance = value; }

    private Camera _cam;

    private void Awake()
    {
       Init();
    }
    
    private void Start()
    {
        StartCoroutine(InitialSpawnRoutine());
    }

    private void Update()
    {
        _timer += Time.deltaTime;
        if (_timer >= spawnInterval)
        {
            _timer = 0f;
            TrySpawnRuntime();
        }
        
        DespawnFarFish();
    }
    
    /// <summary>
    /// 지형 충돌 체크 + 수심별 랜덤 위치 공용으로 쓸 수 있는 헬퍼
    /// </summary>
    public bool TryGetSpawnPositionForObstacle(EOceanDepthZone zone, out Vector2 position, int maxTry = 20)
    {
        for (int i = 0; i < maxTry; i++)
        {
        
            Vector2 p = GetRandomPositionInZone(zone);   
            
            if (Physics2D.OverlapCircle(p, collisionCheckRadius, groundLayer) != null)
                continue;

            position = p;
            return true;
        }

        position = default;
        return false;
    }
    
    private IEnumerator InitialSpawnRoutine()
    {
        yield return null;   // 카메라/플레이어 위치가 안정되고 한 프레임 지난 뒤 초기 스폰
        SpawnInitial();
    }

  

   

   // 외부 호출 함수 ---------------------------------------------------------------------------------

    public void RegisterFish(FishBase fish)
    {
        if (fish == null) return;
        
        if (!_activeFish.Contains(fish))
            _activeFish.Add(fish);

        // 종별 제한 넘으면 바로 제거
        if (IsSpeciesOverLimit(fish.FishType))
        {
            _activeFish.Remove(fish);
            Destroy(fish.gameObject);
        }
    }

    public void UnregisterFish(FishBase fish)
    {
        if (fish == null) return;
        _activeFish.Remove(fish);
    }

    // ---------------------------------------------------------------------------------------------

    // Depth 매핑 / Table 매핑 -------------------------------------------------------------------------------

    private EOceanDepthZone GetDepthZone(float y)
    {
        if (y >= shallowMaxY) return EOceanDepthZone.Shallow;
        if (y >= middleMaxY) return EOceanDepthZone.Middle;
        return EOceanDepthZone.Deep;
    }

    private ResourceSpawnTable GetTable(EOceanDepthZone zone)
    {
        return zone switch
        {
            EOceanDepthZone.Shallow => shallowTable,
            EOceanDepthZone.Middle => middleTable,
            EOceanDepthZone.Deep => deepTable,
            _ => null
        };
    }

    // --------------------------------------------------------------------------------------------

   // 초기 스폰 ------------------------------------------------------------------------------------

    private void SpawnInitial()
    {
        SpawnInitialForZone(EOceanDepthZone.Shallow, initialShallowCount, allowOnScreen: true);
        SpawnInitialForZone(EOceanDepthZone.Middle, initialMiddleCount, allowOnScreen: true);
        SpawnInitialForZone(EOceanDepthZone.Deep, initialDeepCount, allowOnScreen: false);
    }

    private void SpawnInitialForZone(EOceanDepthZone zone, int count, bool allowOnScreen)
    {
        ResourceSpawnTable table = GetTable(zone);
        if (table == null || count <= 0) return;

        const int maxTryPerFish = 30;

        for (int i = 0; i < count; i++)
        {
            for (int t = 0; t < maxTryPerFish; t++)
            {
                Vector2 pos = GetRandomPositionInZone(zone);

                // 지형과 충돌 체크
                if (Physics2D.OverlapCircle(pos, collisionCheckRadius, groundLayer) != null)
                    continue;

                // 지역 밀도 체크
                if (IsAreaTooCrowded(pos))
                    continue;

                // 초기 스폰에서만 화면 안을 허용할지 여부
                if (!allowOnScreen && !IsOffScreen(pos, 2f))
                    continue;

                SpawnFishAt(table, pos);
                break;
            }
        }
    }

    // ----------------------------------------------------------------------------------------------

   // 런타임 스폰 -------------------------------------------------------------------------------------

    private void TrySpawnRuntime()
    {
        CleanupNullFish();

        if (_activeFish.Count >= maxTotalFishCount)
            return;

        // 플레이어 깊이 기준으로 어떤 구역에 스폰할지 선택
        float targetY = player != null ? player.position.y : 0f;
        EOceanDepthZone zone = GetDepthZone(targetY);
        ResourceSpawnTable table = GetTable(zone);
        if (table == null) return;

        const int maxTry = 20;

        for (int i = 0; i < maxTry; i++)
        {
            Vector2 pos = GetRandomPositionInZone(zone);

            // 반드시 카메라 화면 밖
            if (!IsOffScreen(pos, 2f))
                continue;

            // 지형과 충돌 X
            if (Physics2D.OverlapCircle(pos, collisionCheckRadius, groundLayer) != null)
                continue;

            // 주변에 물고기 너무 많으면 패스
            if (IsAreaTooCrowded(pos))
                continue;

            SpawnFishAt(table, pos);
            break;
        }
    }

    // -------------------------------------------------------------------------------------------------

    // Spawn Helpers -----------------------------------------------------------------------------------

    private void SpawnFishAt(ResourceSpawnTable table, Vector2 pos)
    {
        GameObject prefab = table.GetRandomResourcePrefab();
        if (prefab == null) return;

        Instantiate(prefab, pos, Quaternion.identity, transform);
    }

    private void CleanupNullFish()
    {
        _activeFish.RemoveAll(f => f == null);
    }

    private Vector2 GetRandomPositionInZone(EOceanDepthZone zone)
    {
        float minX = spawnAreaMin.x;
        float maxX = spawnAreaMax.x;

        float minY;
        float maxY;

        switch (zone)
        {
            case EOceanDepthZone.Shallow:
                minY = shallowMaxY;
                maxY = spawnAreaMax.y;
                break;
            case EOceanDepthZone.Middle:
                minY = middleMaxY;
                maxY = shallowMaxY;
                break;
            default: 
                minY = spawnAreaMin.y;
                maxY = middleMaxY;
                break;
        }

        float x = Random.Range(minX, maxX);
        float y = Random.Range(minY, maxY);
        return new Vector2(x, y);
    }

    private bool IsOffScreen(Vector2 pos, float margin = 1f)
    {
        float height = _cam.orthographicSize * 2f;
        float width = height * _cam.aspect;
        Vector3 camPos = _cam.transform.position;

        float left = camPos.x - width / 2f;
        float right = camPos.x + width / 2f;
        float bottom = camPos.y - height / 2f;
        float top = camPos.y + height / 2f;

        left -= margin;
        right += margin;
        bottom -= margin;
        top += margin;

        return pos.x < left || pos.x > right || pos.y < bottom || pos.y > top;
    }

    private bool IsAreaTooCrowded(Vector2 pos)
    {
        if (maxFishPerArea <= 0) return false;

        Collider2D[] hits = Physics2D.OverlapCircleAll(pos, densityRadius, fishLayer);
        return hits != null && hits.Length >= maxFishPerArea;
    }

    private bool IsSpeciesOverLimit(ResourceType fishType)
    {
        int max = GetSpeciesMaxCount(fishType);
        if (max <= 0) return false;

        int current = 0;
        foreach (var f in _activeFish)
        {
            if (f == null) continue;
            if (f.FishType == fishType)
                current++;
        }

        return current > max;
    }

    private int GetSpeciesMaxCount(ResourceType fishType)
    {
        foreach (var limit in speciesLimits)
        {
            if (limit != null && limit.fishType == fishType)
                return limit.maxCount;
        }
        return -1; // 제한 없음
    }

   // ----------------------------------------------------------------------------------------



    private void Init()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        if (player == null)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            player = p.transform;
        }

        _cam = Camera.main;
    }
    
    private void DespawnFarFish()
    {
        Vector3 camPos = _cam.transform.position;
        
        for (int i = _activeFish.Count - 1; i >= 0; i--)
        {
            FishBase fish = _activeFish[i];
            if (fish == null)
            {
                _activeFish.RemoveAt(i);
                continue;
            }

            float dist = Vector2.Distance(fish.transform.position, camPos);
            if (dist > despawnDistanceFromCamera)
            {
                Destroy(fish.gameObject);
            }
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        // 스폰 영역
        Gizmos.color = Color.cyan;
        Vector3 size = new Vector3(spawnAreaMax.x - spawnAreaMin.x,
                                   spawnAreaMax.y - spawnAreaMin.y, 0f);
        Vector3 center = new Vector3((spawnAreaMin.x + spawnAreaMax.x) / 2f,
                                     (spawnAreaMin.y + spawnAreaMax.y) / 2f, 0f);
        Gizmos.DrawWireCube(center, size);

        // 수심 경계선
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(new Vector3(spawnAreaMin.x, shallowMaxY, 0f),
                        new Vector3(spawnAreaMax.x, shallowMaxY, 0f));

        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(new Vector3(spawnAreaMin.x, middleMaxY, 0f),
                        new Vector3(spawnAreaMax.x, middleMaxY, 0f));
    }

   
}