using UnityEngine;

/// <summary>
/// ResourceDatabase의 싱글톤 매니저
/// DontDestroyOnLoad로 게임 전체에서 접근 가능
/// </summary>
public class ResourceDatabaseManager : MonoBehaviour
{
    public static ResourceDatabaseManager Instance { get; private set; }

    [Header("Resource Database")]
    [Tooltip("게임의 모든 자원 데이터를 담은 ScriptableObject")]
    [SerializeField] private ResourceDatabase database;

    public ResourceDatabase Database => database;

    void Awake()
    {
        // 싱글톤 패턴
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Database 초기화
            if (database != null)
            {
                database.Initialize();
                Debug.Log("[ResourceDatabaseManager] 초기화 완료");
            }
            else
            {
                Debug.LogError("[ResourceDatabaseManager] ResourceDatabase가 할당되지 않았습니다!");
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 빠른 접근을 위한 편의 메서드들
    /// </summary>
    public static ResourceMetaData GetData(ResourceType type)
    {
        return Instance?.database?.GetData(type);
    }

    public static Sprite GetIcon(ResourceType type)
    {
        return Instance?.database?.GetIcon(type);
    }

    public static ResourceCategory GetCategory(ResourceType type)
    {
        return Instance?.database?.GetCategory(type) ?? ResourceCategory.Food;
    }

    public static string GetDisplayName(ResourceType type)
    {
        return Instance?.database?.GetDisplayName(type) ?? type.ToString();
    }
}
