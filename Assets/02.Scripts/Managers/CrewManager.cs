using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public class CrewManager : MonoBehaviour
{
    public static CrewManager Instance { get; private set; }

    [Header("Crew Data")]
    [SerializeField] private CrewDatabase crewDatabase;  // 선원 데이터베이스

    [SerializeField] private List<CrewMember> crewMembers = new List<CrewMember>();

    // 이벤트
    public event Action<CrewMember> OnCrewStatusChanged;          // 선원 상태 변경
    public event Action<CrewMember, ResourceType> OnResourceAssigned;  // 자원 할당
    public event Action<CrewMember> OnCrewDied;                   // 선원 사망

    // 프로퍼티
    public List<CrewMember> CrewMembers => crewMembers;
    public int TotalCrew => crewMembers.Count;
    public int AliveCrew => crewMembers.Count(c => c.IsAlive);

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

        // 선원 리스트 초기화
        if (crewMembers == null)
        {
            crewMembers = new List<CrewMember>();
        }
    }

    // ========== 테스트용 선원 관리 ==========

    // 선원 추가
    public void AddCrew(CrewMember crew)
    {
        crewMembers.Add(crew);
        Debug.Log($"[선원 추가] {crew.CrewName}");
    }

    // 선원 제거
    public void RemoveCrew(CrewMember crew)
    {
        crewMembers.Remove(crew);
        Debug.Log($"[선원 제거] {crew.CrewName}");
    }

    // ID로 선원 찾기
    public CrewMember GetCrewByID(int id)
    {
        return crewMembers.Find(c => c.CrewID == id);
    }

    // 생존 선원 수
    public int GetSurvivedCrewCount()
    {
        return crewMembers.Count(c => c.IsAlive);
    }

    // 위기 상태 선원 찾기
    public List<CrewMember> GetCriticalCrew()
    {
        return crewMembers.Where(c => c.Status == CrewStatus.Critical).ToList();
    }

    // ========== 자원 분배 ==========

    // 자원을 선원에게 할당 (amount 개수만큼)
    public bool AssignResourceToCrew(CrewMember crew, ResourceType resourceType, int amount = 1)
    {
        // 1. Evening 페이즈 체크
        if (DayManager.Instance.CurrentPhase != DayPhase.Evening)
        {
            Debug.LogWarning("[자원 분배 실패] 저녁 시간에만 자원을 분배할 수 있습니다!");
            return false;
        }

        // 2. 개수 체크
        if (amount <= 0)
        {
            Debug.LogWarning("[자원 분배 실패] 0개 이하는 분배할 수 없습니다!");
            return false;
        }

        // 3. 오늘 이미 해당 카테고리 자원을 받았는지 체크 (데이터 기반)
        var category = GetResourceCategory(resourceType);

        if (category == ResourceCategory.Food && crew.HasReceivedFoodToday)
        {
            Debug.LogWarning($"[자원 분배 실패] {crew.CrewName}은(는) 오늘 이미 식량을 받았습니다!");
            return false;
        }

        if (category == ResourceCategory.Water && crew.HasReceivedWaterToday)
        {
            Debug.LogWarning($"[자원 분배 실패] {crew.CrewName}은(는) 오늘 이미 물을 받았습니다!");
            return false;
        }

        if (category == ResourceCategory.Medicine && crew.HasReceivedMedicineToday)
        {
            Debug.LogWarning($"[자원 분배 실패] {crew.CrewName}은(는) 오늘 이미 약초를 받았습니다!");
            return false;
        }

        // 4. 배의 인벤토리에서 자원 소비
        bool resourceConsumed = ShipManager.Instance.UseResource(resourceType, amount);

        if (!resourceConsumed)
        {
            Debug.LogWarning($"[자원 분배 실패] {resourceType} 부족!");
            return false;
        }

        // 5. 선원에게 자원 전달
        crew.GiveResource(resourceType, amount);

        // 6. 이벤트 발생
        OnResourceAssigned?.Invoke(crew, resourceType);
        OnCrewStatusChanged?.Invoke(crew);

        Debug.Log($"[자원 분배] {crew.CrewName}에게 {resourceType} {amount}개 제공");

        return true;
    }

    // 자원의 카테고리 가져오기 (데이터 기반)
    private ResourceCategory GetResourceCategory(ResourceType type)
    {
        if (ResourceDatabaseManager.Instance == null || ResourceDatabaseManager.Instance.Database == null)
        {
            Debug.LogWarning("[CrewManager] ResourceDatabaseManager를 찾을 수 없습니다!");
            return ResourceCategory.Food; // 기본값
        }

        return ResourceDatabaseManager.Instance.Database.GetCategory(type);
    }

    // ========== 일일 처리 ==========

    // 아침: 선원 상태 확인
    public void UpdateCrewNeeds()
    {
        Debug.Log("=== [아침] 선원 상태 확인 ===");

        foreach (var crew in crewMembers)
        {
            if (crew.IsAlive)
            {
                // 위기 상태 선원 -> UI 이펙트 할당
                if (crew.Status == CrewStatus.Critical)
                {
                    Debug.LogWarning($"⚠️ {crew.CrewName} - 위기 상태! 배고픔: {crew.Hunger:F0}, 갈증: {crew.Thirst:F0}, 체온: {crew.Temperature:F0}");
                }
            }
        }

        PrintAllCrewStatus();
    }

    // 밤: 일일 요구사항 처리
    public void ProcessDailyNeeds()
    {
        Debug.Log("=== [밤] 선원 일일 처리 ===");

        foreach (var crew in crewMembers)
        {
            if (!crew.IsAlive) continue;

            // 일일 악화 처리
            crew.DailyDeterioration();

            // 일일 자원 분배 플래그 초기화
            crew.ResetDailyResourceFlags();

            // 상태 변경 이벤트
            OnCrewStatusChanged?.Invoke(crew);

            // 사망 체크
            if (!crew.IsAlive)
            {
                OnCrewDied?.Invoke(crew);
                Debug.LogError($"💀 {crew.CrewName}이(가) 사망했습니다!");
            }
        }

        // 전체 요약
        int alive = GetSurvivedCrewCount();
        int dead = TotalCrew - alive;
        Debug.Log($"선원 현황: 생존 {alive}명 / 사망 {dead}명");
    }

    // ========== 유틸리티 ==========

    // 전체 선원 상태 출력
    public void PrintAllCrewStatus()
    {
        Debug.Log("=== 선원 전체 상태 ===");
        foreach (var crew in crewMembers)
        {
            Debug.Log(crew.GetStatusSummary());
        }
    }
    

    // 선원 요약 정보
    public string GetCrewSummary()
    {
        int alive = GetSurvivedCrewCount();
        int critical = GetCriticalCrew().Count;

        string summary = $"=== 선원 현황 ===\n";
        summary += $"생존: {alive}/{TotalCrew}\n";
        summary += $"위기 상태: {critical}명";

        return summary;
    }

    // ========== 게임 재시작을 위한 데이터 리셋 ==========

    /// <summary>
    /// 게임 재시작 시 선원 데이터를 완전히 초기화
    /// MainMenuUI.ResetAllGameData()에서 호출
    /// </summary>
    public void ResetCrewData()
    {
        Debug.Log("[CrewManager] 선원 데이터 초기화 시작");

        // 기존 선원 리스트 클리어
        crewMembers.Clear();

        // 초기 선원 5명 생성 (ID 1~5)
        InitializeDefaultCrew();

        Debug.Log($"[CrewManager] 선원 데이터 초기화 완료 - {crewMembers.Count}명 생성");
    }

    /// <summary>
    /// CrewDatabase에서 초기 선원들을 로드하여 생성
    /// </summary>
    private void InitializeDefaultCrew()
    {
        // CrewDatabase가 할당되지 않았을 경우 경고
        if (crewDatabase == null)
        {
            Debug.LogError("[CrewManager] CrewDatabase가 할당되지 않았습니다! Unity Inspector에서 CrewDatabase를 할당하세요.");
            return;
        }

        // CrewDatabase 초기화
        crewDatabase.Initialize();

        // 기본 선원 프리셋 가져오기 (3명)
        List<CrewPreset> defaultPresets = crewDatabase.GetDefaultCrewPresets(3);

        if (defaultPresets.Count == 0)
        {
            Debug.LogError("[CrewManager] CrewDatabase에 선원 프리셋이 없습니다!");
            return;
        }

        // CrewPreset에서 CrewMember 인스턴스 생성
        foreach (var preset in defaultPresets)
        {
            CrewMember newCrew = CrewMember.FromPreset(preset);

            if (newCrew != null)
            {
                crewMembers.Add(newCrew);
                Debug.Log($"[CrewManager] 선원 생성 완료: {newCrew.CrewName} (ID: {newCrew.CrewID}) - 스프라이트: {(newCrew.AliveSprite != null ? "있음" : "없음")}");
            }
            else
            {
                Debug.LogError($"[CrewManager] CrewPreset에서 CrewMember 생성 실패: {preset.crewName}");
            }
        }
    }
}
