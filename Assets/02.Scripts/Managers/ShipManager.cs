using UnityEngine;
using System;

public class ShipManager : MonoBehaviour
{
    public static ShipManager Instance { get; private set; }

    [SerializeField] private Ship ship;

    // 이벤트
    public event Action<Ship> OnShipStatusChanged;
    public event Action<ResourceType, int> OnResourceChanged;  // 특정 자원 변경
    public event Action OnInventoryChanged;  // 인벤토리 전체 변경 (여러 자원 동시 변경 시)

    public Ship Ship => ship;

    void Awake()
    {
        // 싱글톤
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        // 배 객체 생성
        if (ship == null)
        {
            ship = new Ship();
        }
    }

    void Start()
    {
        // ResourceDatabase에서 초기 인벤토리 로드
        if (ResourceDatabaseManager.Instance != null && ResourceDatabaseManager.Instance.Database != null)
        {
            ship.InitializeInventory(ResourceDatabaseManager.Instance.Database);
            Debug.Log("[ShipManager] Ship 인벤토리 초기화 완료");
        }
        else
        {
            Debug.LogError("[ShipManager] ResourceDatabaseManager를 찾을 수 없습니다!");
        }

        // 초기 상태 로그
        Debug.Log(ship.GetShipStatusSummary());
    }

    // ========== 배 상태 관리 ==========

    // 배 상태 확인 (아침)
    public void CheckShipStatus()
    {
        Debug.Log($"[아침] 배 상태 확인 - 전체 상태: {ship.Hp:F1}%");

        if (ship.IsCritical)
        {
            Debug.LogWarning("⚠️ 경고: 배가 위험한 상태입니다!");
        }

        if (!ship.IsAbleToSail)
        {
            Debug.LogError("⚠️ 위험: 배가 항해 불가능 상태입니다!");
        }

        OnShipStatusChanged?.Invoke(ship);
    }

    // 배 일일 노화 처리 (밤)
    public void ProcessDailyShipDeterioration()
    {
        ship.DailyDeterioration();
        OnShipStatusChanged?.Invoke(ship);

        Debug.Log($"[밤] 배 노화 - 전체 상태: {ship.Hp:F1}%");

        if (ship.IsCritical)
        {
            Debug.LogWarning("⚠️ 경고: 배가 위험한 상태입니다!");
        }

        if (!ship.IsAbleToSail)
        {
            Debug.LogError("⚠️ 위험: 배가 항해 불가능 상태입니다!");
        }
    }

    // 배 전체 상태 출력
    public void PrintShipStatus()
    {
        Debug.Log(ship.GetShipStatusSummary());
    }

    // ========== 자원 관리==========

    // 자원 추가
    public void AddResource(ResourceType type, int amount)
    {
        ship.AddResource(type, amount);

        // 이벤트 발생
        OnResourceChanged?.Invoke(type, amount);
        OnInventoryChanged?.Invoke();

        Debug.Log($"[자원 획득] {type} +{amount} (현재: {ship.GetResourceAmount(type)})");
    }

    // 자원 사용
    public bool UseResource(ResourceType type, int amount)
    {
        bool success = ship.ConsumeResource(type, amount);

        if (success)
        {
            // 이벤트 발생
            OnResourceChanged?.Invoke(type, -amount);
            OnInventoryChanged?.Invoke();

            Debug.Log($"[자원 사용] {type} -{amount} (현재: {ship.GetResourceAmount(type)})");
        }
        else
        {
            Debug.LogWarning($"[자원 부족] {type} 부족! 필요: {amount}, 보유: {ship.GetResourceAmount(type)}");
        }

        return success;
    }

    // 자원 양 확인
    public int GetResourceAmount(ResourceType type)
    {
        return ship.GetResourceAmount(type);
    }

    // ========== 배 수리 ==========

    // 배 수리
    public void RepairShip(int materialAmount)
    {
        bool success = ship.RepairWithMaterials(materialAmount);

        if (success)
        {
            OnShipStatusChanged?.Invoke(ship);
            float repairAmount = materialAmount;
            Debug.Log($"[수리 완료] Hp +{repairAmount:F1}% (재료 소비: {materialAmount})");
        }
        else
        {
            Debug.LogWarning($"수리 재료 부족! 필요: {materialAmount}, 보유: {ship.GetTotalRepairMaterials()}");
        }
    }
}