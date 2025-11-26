using UnityEngine;

[System.Serializable]
public class ShipInventory
{
    [Header("Food Resources")]
    public int Fish = 3;
    public int Shellfish = 0;
    public int Seaweed = 1;

    [Header("Water")]
    public int CleanWater = 4;

    [Header("Medical")]
    public int Herbs = 2;

    [Header("Repair Materials")]
    public int Wood = 7;

    // 총 식량
    public int TotalFood => Fish + Shellfish + Seaweed;

    // 총 의약품
    public int TotalMedicine => Herbs;

    // 총 수리 재료
    public int TotalRepairMaterials => Wood;

    // 자원 추가
    public void AddResource(ResourceType type, int amount)
    {
        switch (type)
        {
            case ResourceType.NormalFish:
                Fish += amount;
                break;
            case ResourceType.SpecialFish:
                Shellfish += amount;
                break;
            case ResourceType.Seaweed:
                Seaweed += amount;
                break;
            case ResourceType.CleanWater:
                CleanWater += amount;
                break;
            case ResourceType.Herbs:
                Herbs += amount;
                break;
            case ResourceType.Wood:
                Wood += amount;
                break;
        }
    }

    // 자원 소비
    public bool ConsumeResource(ResourceType type, int amount)
    {
        int currentAmount = GetResourceAmount(type);
        if (currentAmount < amount) return false;

        AddResource(type, -amount);
        return true;
    }

    // 자원 양 확인
    public int GetResourceAmount(ResourceType type)
    {
        return type switch
        {
            ResourceType.NormalFish => Fish,
            ResourceType.SpecialFish => Shellfish,
            ResourceType.Seaweed => Seaweed,
            ResourceType.CleanWater => CleanWater,
            ResourceType.Herbs => Herbs,
            ResourceType.Wood => Wood,
            _ => 0
        };
    }

    // 자원 상태 출력
    public string GetResourceSummary()
    {
        return $"식량: {TotalFood} (생선:{Fish}, 조개:{Shellfish}, 해초:{Seaweed})\n" +
               $"물: {CleanWater}\n" +
               $"의약품: {TotalMedicine} (약초:{Herbs})\n" +
               $"수리재료: {TotalRepairMaterials} (목재:{Wood})";
    }
}