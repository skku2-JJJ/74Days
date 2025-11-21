using UnityEngine;
using System;

public class ShipManager : MonoBehaviour
{
    public static ShipManager Instance { get; private set; }

    [SerializeField] private Ship ship;

    // 이벤트
    public event Action<Ship> OnShipStatusChanged;
    public event Action<ResourceType, int> OnResourceChanged;

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

        // 배 초기화
        if (ship == null)
        {
            ship = new Ship();
        }
    }

    void Start()
    {
        // 초기 상태 로그
        Debug.Log(ship.GetShipStatusSummary());
        Debug.Log(ship.GetResourceSummary());
    }

    // ========== 배 상태 관리 ==========

    // 배 상태 업데이트
    public void UpdateShipStatus()
    {
        ship.DailyDeterioration();
        OnShipStatusChanged?.Invoke(ship);

        Debug.Log($"[Day] 배 노화 - 전체 상태: {ship.OverallCondition:F1}%");

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
        OnResourceChanged?.Invoke(type, amount);

        Debug.Log($"[자원 획득] {type} +{amount} (현재: {ship.GetResourceAmount(type)})");
    }

    // 자원 사용
public bool UseResource(ResourceType type, int amount)
{
        bool success = ship.ConsumeResource(type, amount);

        if (success)
        {
            OnResourceChanged?.Invoke(type, -amount);
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

    // 자원 전체 상태 출력
    public void PrintResourceStatus()
    {
        Debug.Log(ship.GetResourceSummary());
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