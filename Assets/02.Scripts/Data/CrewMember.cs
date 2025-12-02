using UnityEngine;

[System.Serializable]
public class CrewMember
{
    [Header("Identity")]
    public string CrewName;
    public int CrewID;

    [Header("sprite")]
    public Sprite AliveSprite;
    public Sprite DeadSprite;

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

    // ========== CrewPreset 기반 생성 ==========

    /// <summary>
    /// CrewPreset에서 CrewMember 인스턴스 생성 (Factory Method)
    /// </summary>
    public static CrewMember FromPreset(CrewPreset preset)
    {
        if (preset == null)
        {
            Debug.LogError("[CrewMember] CrewPreset이 null입니다!");
            return null;
        }

        // 생성자로 기본 생성
        CrewMember crew = new CrewMember(preset.crewName, preset.crewID);

        // 스프라이트 설정
        crew.AliveSprite = preset.aliveSprite;
        crew.DeadSprite = preset.deadSprite;

        // 초기 생존 수치 설정
        crew.Hunger = preset.initialHunger;
        crew.Thirst = preset.initialThirst;
        crew.Temperature = preset.initialTemperature;
        crew.IsAlive = true;

        // 일일 감소 설정
        crew.MinHungerDecrease = preset.minHungerDecrease;
        crew.MaxHungerDecrease = preset.maxHungerDecrease;
        crew.MinThirstDecrease = preset.minThirstDecrease;
        crew.MaxThirstDecrease = preset.maxThirstDecrease;
        crew.MinTemperatureDecrease = preset.minTemperatureDecrease;
        crew.MaxTemperatureDecrease = preset.maxTemperatureDecrease;

        // 자원 회복 설정
        crew.FoodHungerRecovery = preset.foodHungerRecovery;
        crew.WaterThirstRecovery = preset.waterThirstRecovery;
        crew.HerbsTemperatureRecovery = preset.herbsTemperatureRecovery;

        // 상태 임계값 설정
        crew.CriticalThreshold = preset.criticalThreshold;
        crew.PoorThreshold = preset.poorThreshold;

        // 일일 플래그 초기화
        crew.ResetDailyResourceFlags();

        Debug.Log($"[CrewMember] CrewPreset에서 생성: {crew.CrewName} (ID: {crew.CrewID})");

        return crew;
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

        // ResourceDatabase에서 자원 정보 가져오기
        if (ResourceDatabaseManager.Instance == null || ResourceDatabaseManager.Instance.Database == null)
        {
            Debug.LogWarning($"[CrewMember] ResourceDatabaseManager를 찾을 수 없습니다!");
            return;
        }

        var resourceData = ResourceDatabaseManager.Instance.Database.GetData(type);
        if (resourceData == null)
        {
            Debug.LogWarning($"[CrewMember] {type}의 ResourceMetaData를 찾을 수 없습니다!");
            return;
        }

        // 데이터 기반 회복량 적용
        if (resourceData.hungerRecovery > 0)
        {
            Hunger = Mathf.Min(100, Hunger + resourceData.hungerRecovery * amount);
        }

        if (resourceData.thirstRecovery > 0)
        {
            Thirst = Mathf.Min(100, Thirst + resourceData.thirstRecovery * amount);
        }

        if (resourceData.temperatureRecovery > 0)
        {
            Temperature = Mathf.Min(100, Temperature + resourceData.temperatureRecovery * amount);
        }

        // 카테고리별 일일 플래그 업데이트
        switch (resourceData.category)
        {
            case ResourceCategory.Food:
                HasReceivedFoodToday = true;
                break;
            case ResourceCategory.Water:
                HasReceivedWaterToday = true;
                break;
            case ResourceCategory.Medicine:
                HasReceivedMedicineToday = true;
                break;
            case ResourceCategory.Material:
                // 수리 재료는 선원에게 줄 수 없음
                Debug.LogWarning($"{CrewName}에게 {resourceData.displayName}을(를) 줄 수 없습니다!");
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
