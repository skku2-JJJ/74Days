using UnityEngine;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// 선원 프리셋 데이터베이스 (ScriptableObject)
/// 모든 선원 프리셋을 중앙에서 관리하고 빠른 조회를 위한 캐싱 제공
/// </summary>
[CreateAssetMenu(fileName = "CrewDatabase", menuName = "Game/Crew Database")]
public class CrewDatabase : ScriptableObject
{
    [Header("Crew Presets")]
    [Tooltip("게임에서 사용할 모든 선원 프리셋 목록")]
    [SerializeField] private List<CrewPreset> allCrewPresets = new List<CrewPreset>();

    // 런타임 캐시 (ID 기반 빠른 조회)
    private Dictionary<int, CrewPreset> _presetByID;
    private Dictionary<string, CrewPreset> _presetByName;
    private bool _isInitialized = false;

    /// <summary>
    /// 데이터베이스 초기화 (Dictionary 캐싱)
    /// CrewManager.Awake()에서 호출
    /// </summary>
    public void Initialize()
    {
        if (_isInitialized)
        {
            Debug.LogWarning("[CrewDatabase] 이미 초기화되었습니다!");
            return;
        }

        _presetByID = new Dictionary<int, CrewPreset>();
        _presetByName = new Dictionary<string, CrewPreset>();

        foreach (var preset in allCrewPresets)
        {
            if (preset == null)
            {
                Debug.LogWarning("[CrewDatabase] null인 CrewPreset이 포함되어 있습니다!");
                continue;
            }

            // ID로 캐싱
            if (_presetByID.ContainsKey(preset.crewID))
            {
                Debug.LogWarning($"[CrewDatabase] 중복된 CrewID 발견: {preset.crewID} ({preset.crewName})");
            }
            else
            {
                _presetByID[preset.crewID] = preset;
            }

            // 이름으로 캐싱
            if (_presetByName.ContainsKey(preset.crewName))
            {
                Debug.LogWarning($"[CrewDatabase] 중복된 선원 이름 발견: {preset.crewName} (ID: {preset.crewID})");
            }
            else
            {
                _presetByName[preset.crewName] = preset;
            }
        }

        _isInitialized = true;
        Debug.Log($"[CrewDatabase] 초기화 완료 - {allCrewPresets.Count}명의 선원 프리셋 로드");
    }

    // ========== 조회 메서드 ==========

    /// <summary>
    /// ID로 선원 프리셋 조회
    /// </summary>
    public CrewPreset GetPreset(int id)
    {
        if (!_isInitialized)
        {
            Debug.LogError("[CrewDatabase] 초기화되지 않았습니다! Initialize()를 먼저 호출하세요.");
            return null;
        }

        if (_presetByID.TryGetValue(id, out CrewPreset preset))
        {
            return preset;
        }

        Debug.LogWarning($"[CrewDatabase] ID {id}에 해당하는 선원 프리셋을 찾을 수 없습니다!");
        return null;
    }

    /// <summary>
    /// 이름으로 선원 프리셋 조회
    /// </summary>
    public CrewPreset GetPreset(string name)
    {
        if (!_isInitialized)
        {
            Debug.LogError("[CrewDatabase] 초기화되지 않았습니다! Initialize()를 먼저 호출하세요.");
            return null;
        }

        if (_presetByName.TryGetValue(name, out CrewPreset preset))
        {
            return preset;
        }

        Debug.LogWarning($"[CrewDatabase] 이름 '{name}'에 해당하는 선원 프리셋을 찾을 수 없습니다!");
        return null;
    }

    /// <summary>
    /// 게임 시작 시 사용할 기본 선원 프리셋 목록 반환
    /// (리스트 앞에서부터 3명)
    /// </summary>
    public List<CrewPreset> GetDefaultCrewPresets(int count = 3)
    {
        if (!_isInitialized)
        {
            Debug.LogError("[CrewDatabase] 초기화되지 않았습니다! Initialize()를 먼저 호출하세요.");
            return new List<CrewPreset>();
        }

        if (allCrewPresets.Count < count)
        {
            Debug.LogWarning($"[CrewDatabase] 요청한 선원 수({count}명)가 등록된 프리셋 수({allCrewPresets.Count}명)보다 많습니다!");
            return allCrewPresets.Where(p => p != null).ToList();
        }

        return allCrewPresets.Take(count).Where(p => p != null).ToList();
    }

    /// <summary>
    /// 모든 선원 프리셋 반환
    /// </summary>
    public List<CrewPreset> GetAllPresets()
    {
        return allCrewPresets.Where(p => p != null).ToList();
    }

    /// <summary>
    /// 등록된 선원 프리셋 개수
    /// </summary>
    public int PresetCount => allCrewPresets.Count;

    // ========== 에디터 전용 ==========

    void OnValidate()
    {
        // Inspector에서 수정 시 경고 표시
        if (allCrewPresets.Count == 0)
        {
            Debug.LogWarning("[CrewDatabase] 선원 프리셋이 하나도 등록되지 않았습니다!");
        }

        // 중복 ID 체크
        var idGroups = allCrewPresets.Where(p => p != null).GroupBy(p => p.crewID);
        foreach (var group in idGroups)
        {
            if (group.Count() > 1)
            {
                Debug.LogWarning($"[CrewDatabase] 중복된 CrewID 발견: {group.Key} ({group.Count()}명)");
            }
        }
    }
}