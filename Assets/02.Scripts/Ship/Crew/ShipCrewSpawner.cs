using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Ship 씬에서 모든 선원 NPC를 생성 및 관리
/// CrewManager의 선원 데이터를 기반으로 자동 생성
/// </summary>
public class ShipCrewSpawner : MonoBehaviour
{
    [System.Serializable]
    public class CrewSpawnData
    {
        public int crewID;                     // 연동할 CrewMember ID
        public GameObject crewPrefab;          // 각 선원의 프리팹 (BigMan, BoldMan, Captain 등)
        public Vector3 spawnPosition;          // 초기 생성 위치
        public Transform[] patrolPoints;       // 순찰 포인트 배열
        public float moveSpeed = 2f;           // 이동 속도
        public float waitTime = 2f;            // 대기 시간
    }

    [Header("Spawn Settings")]
    [SerializeField] private List<CrewSpawnData> _crewSpawnDataList = new List<CrewSpawnData>();

    [Header("Container")]
    [SerializeField] private Transform _npcContainer;  // NPC들을 담을 부모 오브젝트 (옵션)

    // 생성된 NPC 관리
    private Dictionary<int, ShipCrewNPC> _spawnedNPCs = new Dictionary<int, ShipCrewNPC>();

    void Start()
    {
        // NPC 컨테이너 설정
        if (_npcContainer == null)
        {
            GameObject container = new GameObject("CrewNPCs");
            _npcContainer = container.transform;
            _npcContainer.SetParent(transform);
        }

        // 선원 NPC 생성
        SpawnAllCrewNPCs();
    }

    /// <summary>
    /// 모든 선원 NPC 생성
    /// </summary>
    private void SpawnAllCrewNPCs()
    {
        if (CrewManager.Instance == null)
        {
            Debug.LogError("[ShipCrewSpawner] CrewManager가 없습니다!");
            return;
        }

        foreach (var spawnData in _crewSpawnDataList)
        {
            // 프리팹 체크
            if (spawnData.crewPrefab == null)
            {
                Debug.LogError($"[ShipCrewSpawner] CrewID {spawnData.crewID}의 프리팹이 할당되지 않았습니다!");
                continue;
            }

            // CrewManager에서 해당 ID의 선원 존재 확인
            CrewMember crew = CrewManager.Instance.GetCrewByID(spawnData.crewID);

            if (crew == null)
            {
                Debug.LogWarning($"[ShipCrewSpawner] CrewID {spawnData.crewID}에 해당하는 선원이 없습니다!");
                continue;
            }

            // 이미 사망한 선원은 생성하지 않음
            if (!crew.IsAlive)
            {
                Debug.Log($"[ShipCrewSpawner] CrewID {spawnData.crewID} ({crew.CrewName})은(는) 이미 사망했습니다. NPC를 생성하지 않습니다.");
                continue;
            }

            // NPC 생성
            SpawnCrewNPC(spawnData);
        }

        Debug.Log($"[ShipCrewSpawner] 총 {_spawnedNPCs.Count}명의 선원 NPC 생성 완료");
    }

    /// <summary>
    /// 개별 선원 NPC 생성
    /// </summary>
    private void SpawnCrewNPC(CrewSpawnData spawnData)
    {
        // 각 선원의 프리팹 인스턴스화
        GameObject npcObject = Instantiate(spawnData.crewPrefab, spawnData.spawnPosition, Quaternion.identity, _npcContainer);
        npcObject.name = $"{spawnData.crewPrefab.name}_CrewID{spawnData.crewID}";

        // ShipCrewNPC 컴포넌트 가져오기 또는 추가
        ShipCrewNPC npc = npcObject.GetComponent<ShipCrewNPC>();

        if (npc == null)
        {
            npc = npcObject.AddComponent<ShipCrewNPC>();
            Debug.Log($"[ShipCrewSpawner] {spawnData.crewPrefab.name}에 ShipCrewNPC 컴포넌트 자동 추가");
        }

        // ShipCrewNPC 설정
        npc.SetCrewID(spawnData.crewID);
        npc.SetPatrolPoints(spawnData.patrolPoints);
        npc.SetMoveSpeed(spawnData.moveSpeed);

        // 딕셔너리에 추가
        _spawnedNPCs[spawnData.crewID] = npc;

        Debug.Log($"[ShipCrewSpawner] {spawnData.crewPrefab.name} (CrewID {spawnData.crewID}) NPC 생성 완료");
    }

    /// <summary>
    /// 특정 선원 NPC 가져오기
    /// </summary>
    public ShipCrewNPC GetCrewNPC(int crewID)
    {
        if (_spawnedNPCs.TryGetValue(crewID, out ShipCrewNPC npc))
        {
            return npc;
        }

        return null;
    }

    /// <summary>
    /// 모든 선원 NPC 순찰 일시 정지
    /// </summary>
    public void PauseAllPatrols()
    {
        foreach (var npc in _spawnedNPCs.Values)
        {
            if (npc != null)
            {
                npc.StopPatrol();
            }
        }
    }

    /// <summary>
    /// 모든 선원 NPC 순찰 재개
    /// </summary>
    public void ResumeAllPatrols()
    {
        foreach (var npc in _spawnedNPCs.Values)
        {
            if (npc != null)
            {
                npc.StartPatrol();
            }
        }
    }

    // ========== 디버그용 ==========

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        if (_crewSpawnDataList == null || _crewSpawnDataList.Count == 0) return;

        // 생성 위치 표시
        Gizmos.color = Color.green;

        foreach (var spawnData in _crewSpawnDataList)
        {
            Gizmos.DrawWireSphere(spawnData.spawnPosition, 0.5f);
            UnityEditor.Handles.Label(spawnData.spawnPosition + Vector3.up * 0.7f, $"Crew {spawnData.crewID}");
        }
    }
#endif
}
