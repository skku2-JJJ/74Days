using UnityEngine;

[System.Serializable]
public class CrewMember
{
    [Header("Identity")]
    public string CrewName;
    public int CrewID;

    [Header("Vital Status")]
    [Range(0, 100)] public float Hunger = 100f;          // 배고픔 (0=굶어죽음, 100=배부름)
    [Range(0, 100)] public float Thirst = 100f;          // 갈증 (0=탈수, 100=충분)
    [Range(0, 100)] public float Temperature = 100f;     // 체온 (0=동사, 100=정상)
    public bool IsAlive = true;

    [Header("Daily Settings")]
    public float MinHungerDecrease = 10f;                // 배고픔 최소 감소량
    public float MaxHungerDecrease = 20f;                // 배고픔 최대 감소량
    public float MinThirstDecrease = 15f;                // 갈증 최소 감소량
    public float MaxThirstDecrease = 25f;                // 갈증 최대 감소량
    public float MinTemperatureDecrease = 0f;            // 체온 최소 감소량
    public float MaxTemperatureDecrease = 10f;           // 체온 최대 감소량

    [Header("Resource Recovery Settings")]
    public float FoodHungerRecovery = 25f;               // 식량의 배고픔 회복량
    public float WaterThirstRecovery = 30f;              // 물의 갈증 회복량
    public float HerbsTemperatureRecovery = 20f;         // 약초의 체온 회복량
    
    [Header("Status Thresholds")]
    public float CriticalThreshold = 20f;
    public float PoorThreshold = 50f;

    [Header("Daily Resource Distribution")]
    public bool HasReceivedFoodToday = false;          // 오늘 식량 받았는지
    public bool HasReceivedWaterToday = false;         // 오늘 물 받았는지
    public bool HasReceivedMedicineToday = false;      // 오늘 약초 받았는지

    // 상태 프로퍼티
    public CrewStatus Status
    {
        get
        {
            if (!IsAlive) return CrewStatus.Dead;
            if (Hunger < CriticalThreshold || Thirst < CriticalThreshold || Temperature < CriticalThreshold) return CrewStatus.Critical;
            if (Hunger < PoorThreshold || Thirst < PoorThreshold || Temperature < PoorThreshold) return CrewStatus.Poor;
            return CrewStatus.Healthy;
        }
    }

    // Poor 상태인 수치들 반환
    public VitalType GetPoorVitals()
    {
        VitalType result = VitalType.None;

        if (Hunger < PoorThreshold) result |= VitalType.Hunger;
        if (Thirst < PoorThreshold) result |= VitalType.Thirst;
        if (Temperature < PoorThreshold) result |= VitalType.Temperature;

        return result;
    }

    // Critical 상태인 수치들 반환
    public VitalType GetCriticalVitals()
    {
        VitalType result = VitalType.None;

        if (Hunger < CriticalThreshold) result |= VitalType.Hunger;
        if (Thirst < CriticalThreshold) result |= VitalType.Thirst;
        if (Temperature < CriticalThreshold) result |= VitalType.Temperature;

        return result;
    }

    // 생성자
    public CrewMember(string name, int id)
    {
        CrewName = name;
        CrewID = id;
    }

    // ========== 하루 경과 ==========

    // 하루 경과 시 생존 수치 감소
    public void DailyDeterioration()
    {
        if (!IsAlive) return;

        // 배고픔 랜덤 감소
        float hungerDecrease = Random.Range(MinHungerDecrease, MaxHungerDecrease);
        Hunger -= hungerDecrease;

        // 갈증 랜덤 감소
        float thirstDecrease = Random.Range(MinThirstDecrease, MaxThirstDecrease);
        Thirst -= thirstDecrease;

        // 체온 랜덤 감소
        float temperatureDecrease = Random.Range(MinTemperatureDecrease, MaxTemperatureDecrease);
        Temperature -= temperatureDecrease;

        // 값 제한
        Hunger = Mathf.Clamp(Hunger, 0, 100);
        Thirst = Mathf.Clamp(Thirst, 0, 100);
        Temperature = Mathf.Clamp(Temperature, 0, 100);

        // 사망 체크 (하나라도 0 이하면 사망)
        if (Hunger <= 0f || Thirst <= 0f || Temperature <= 0f)
        {
            Die();
        }
    }

    // ========== 자원 할당 ==========

    // 자원을 받음 (amount 개수만큼)
    public void GiveResource(ResourceType type, int amount = 1)
    {
        if (!IsAlive) return;
        if (amount <= 0) return;

        switch (type)
        {
            case ResourceType.NormalFish:
            case ResourceType.SpecialFish:
            case ResourceType.Seaweed:
                // 식량: 배고픔 회복
                Hunger = Mathf.Min(100, Hunger + FoodHungerRecovery * amount);
                HasReceivedFoodToday = true;
                break;

            case ResourceType.CleanWater:
                // 물: 갈증 회복
                Thirst = Mathf.Min(100, Thirst + WaterThirstRecovery * amount);
                HasReceivedWaterToday = true;
                break;

            case ResourceType.Herbs:
                // 약초: 체온 회복
                Temperature = Mathf.Min(100, Temperature + HerbsTemperatureRecovery * amount);
                HasReceivedMedicineToday = true;
                break;

            case ResourceType.Wood:
                // 목재는 선원에게 줄 수 없음
                Debug.LogWarning($"{CrewName}에게 목재를 줄 수 없습니다!");
                break;
        }
    }

    // 일일 자원 분배 플래그 초기화 (Night 페이즈에 호출)
    public void ResetDailyResourceFlags()
    {
        HasReceivedFoodToday = false;
        HasReceivedWaterToday = false;
        HasReceivedMedicineToday = false;
    }

    // ========== 상태 관리 ==========

    // 사망 처리
    private void Die()
    {
        IsAlive = false;
        Hunger = 0f;
        Thirst = 0f;
        Temperature = 0f;
    }

    // 상태 요약
    public string GetStatusSummary()
    {
        if (!IsAlive)
        {
            return $"[{CrewName}] 사망";
        }

        string summary = $"[{CrewName}] (ID: {CrewID})\n";
        summary += $"  상태: {Status}\n";
        summary += $"  배고픔: {Hunger:F0} | 갈증: {Thirst:F0} | 체온: {Temperature:F0}";

        return summary;
    }
}
