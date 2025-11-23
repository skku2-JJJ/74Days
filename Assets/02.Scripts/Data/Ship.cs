using UnityEngine;

[System.Serializable]
public class Ship
{
    [Header("Ship Identity")]
    public string ShipName = "JJJ";

    [Header("Ship Condition")]
    [Range(0, 100)] public float Hp = 100f;    // 배 체력

    [Header("Ship Resources")]
    [SerializeField] private ShipInventory _inventory;

    [Header("Ship Status")]
    public bool IsAbleToSail = true;                          // 항해 가능 여부
    public float DailyDeteriorationRate = 1f;                // 일일 노화율

    [Header("Status Thresholds")]
    public float CriticalHpThreshold = 30f;
    public float PoorHpThreshold = 70f;

    // 배 전체 상태
    public float OverallCondition => Hp;

    // 배가 치명적 상태일시 UI 이펙트
    public bool IsCritical => Hp < CriticalHpThreshold;

    // 배 상태 프로퍼티
    public ShipStatus Status
    {
        get
        {
            if (Hp <= 0) return ShipStatus.Destroyed;
            if (Hp < CriticalHpThreshold) return ShipStatus.Critical;
            if (Hp < PoorHpThreshold) return ShipStatus.Poor;
            return ShipStatus.Healthy;
        }
    }

    // 생성자
    public Ship()
    {
        _inventory = new ShipInventory();
    }

    // 하루 경과 시 배 노화
    public void DailyDeterioration()
    {
        Hp = Mathf.Max(0, Hp - DailyDeteriorationRate);
        UpdateSeaworthyStatus();
    }

    // 배 수리
    public void RepairShip(float amount)
    {
        Hp = Mathf.Min(100, Hp + amount);
        UpdateSeaworthyStatus();
    }

    // 항해 가능 여부 업데이트
    private void UpdateSeaworthyStatus()
    {
        // Hp가 0 이하면 항해 불가
        IsAbleToSail = Hp > 0f;
    }

    // ========== 자원 관리 메서드 ==========

    // 자원 추가
    public void AddResource(ResourceType type, int amount)
    {
        _inventory.AddResource(type, amount);
    }

    // 자원 소비
    public bool ConsumeResource(ResourceType type, int amount)
    {
        return _inventory.ConsumeResource(type, amount);
    }

    // 자원 양 확인
    public int GetResourceAmount(ResourceType type)
    {
        return _inventory.GetResourceAmount(type);
    }

    // 총 식량
    public int GetTotalFood()
    {
        return _inventory.TotalFood;
    }

    // 총 의약품
    public int GetTotalMedicine()
    {
        return _inventory.TotalMedicine;
    }

    // 총 수리 재료
    public int GetTotalRepairMaterials()
    {
        return _inventory.TotalRepairMaterials;
    }

    // 배 수리 
    public bool RepairWithMaterials(int materialAmount)
    {
        // 0 이하 요청 방어
        if (materialAmount <= 0)
            return false;

        // 목재 개수 확인
        if (_inventory.Wood < materialAmount)
            return false;

        // 재료 소비
        _inventory.ConsumeResource(ResourceType.Wood, materialAmount);

        RepairShip(materialAmount);

        return true;
    }

    // 자원 상태 요약
    public string GetResourceSummary()
    {
        return _inventory.GetResourceSummary();
    }

    // ========== 배 상태 메서드 ==========

    // 배 상태 요약
    public string GetShipStatusSummary()
    {
        string status = $"=== {ShipName} 상태 ===\n";
        status += $"배 체력(Hp): {Hp:F1}%\n";
        status += $"항해 가능: {(IsAbleToSail ? "예" : "아니오")}\n";

        if (IsCritical)
        {
            status += "⚠️ 경고: 배가 위험한 상태입니다!";
        }

        return status;
    }
}