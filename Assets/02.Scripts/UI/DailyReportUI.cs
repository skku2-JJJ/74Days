using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

/// <summary>
/// Daily Report UI 관리
/// Ship 씬의 기존 구조(Report/Crews/Crew(Report))에 맞게 수정됨
/// </summary>
public class DailyReportUI : MonoBehaviour
{
    public static DailyReportUI Instance { get; private set; }

    [Header("Day Info")]
    [SerializeField] private TextMeshProUGUI _dayReportText; // DayReportText

    [Header("Crew Report Items (기존 구조 활용)")]
    [SerializeField] private CrewReportItem[] _crewReportItems = new CrewReportItem[3]; // 최대 3명

    [Header("Panel")]
    [SerializeField] private GameObject _panelRoot; // Report GameObject

    [System.Serializable]
    public class CrewReportItem
    {
        public GameObject crewObject;           // Crew(Report) GameObject
        public TextMeshProUGUI nameText;        // Name
        public TextMeshProUGUI mentText;        // Ment (대사)
        public CrewsStateUpdate stateSliders;   // StateSliders (슬라이더 컴포넌트)
    }

    void Awake()
    {
        // 싱글톤
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // DayManager 이벤트 구독
        if (DayManager.Instance != null)
        {
            DayManager.Instance.OnDayStart += OnDayStart;
            DayManager.Instance.OnPhaseChange += OnPhaseChanged;
        }
    }

    void Start()
    {
        // 패널은 항상 활성화 상태 (위치로 제어)
    }

    void OnDestroy()
    {
        // 이벤트 구독 해제
        if (DayManager.Instance != null)
        {
            DayManager.Instance.OnDayStart -= OnDayStart;
            DayManager.Instance.OnPhaseChange -= OnPhaseChanged;
        }
    }

    // ========== 이벤트 핸들러 ==========

    // 새 날 시작 시 호출 - 데이터만 업데이트
    private void OnDayStart(int day)
    {
        UpdateAllInfo();
        Debug.Log("[Daily Report] Day 시작 - 데이터 업데이트 완료");
    }

    // 페이즈 변경 시 호출 - 필요 시 사용
    private void OnPhaseChanged(DayPhase phase)
    {
        // 애니메이션은 DailyReportUpDown에서 처리
    }

    // ========== 데이터 제어 ==========
    // 실제 Show/Hide는 DailyReportUpDown에서 애니메이션으로 처리
    // 여기서는 데이터 업데이트만 담당

    // ========== 정보 갱신 ==========

    public void UpdateAllInfo()
    {
        UpdateDayInfo();
        UpdateCrewReports();
    }

    // 날짜 정보 갱신
    private void UpdateDayInfo()
    {
        if (_dayReportText == null) return;

        if (DayManager.Instance != null)
        {
            int day = DayManager.Instance.CurrentDay;
            _dayReportText.text = $"Day {day} Report";
        }
        else
        {
            _dayReportText.text = "Day ? Report";
        }
    }

    // 선원 리포트 갱신
    private void UpdateCrewReports()
    {
        if (CrewManager.Instance == null)
        {
            Debug.LogWarning("[MorningReportUI] CrewManager.Instance가 null입니다.");
            return;
        }

        var crewMembers = CrewManager.Instance.CrewMembers;

        // 각 선원 항목 업데이트
        for (int i = 0; i < _crewReportItems.Length; i++)
        {
            if (_crewReportItems[i].crewObject == null) continue;

            // 선원이 있으면 활성화하고 데이터 설정
            if (i < crewMembers.Count)
            {
                _crewReportItems[i].crewObject.SetActive(true);
                UpdateCrewReportItem(_crewReportItems[i], crewMembers[i], i);
            }
            else
            {
                // 선원이 없으면 비활성화
                _crewReportItems[i].crewObject.SetActive(false);
            }
        }
    }

    // 개별 선원 리포트 항목 업데이트
    private void UpdateCrewReportItem(CrewReportItem item, CrewMember crew, int crewIndex)
    {
        if (crew == null) return;

        // 이름 업데이트
        if (item.nameText != null)
        {
            item.nameText.text = crew.IsAlive ? $"{crew.CrewName} : " : $"{crew.CrewName} (사망)";
        }

        // 대사 업데이트
        if (item.mentText != null)
        {
            item.mentText.text = GetStatusComment(crew);
        }

        // 슬라이더 업데이트 (CrewsStateUpdate의 기존 메서드 활용)
        if (item.stateSliders != null)
        {
            item.stateSliders.HpSliderUpdate(crew.Hunger);
            item.stateSliders.WaterSliderUpdate(crew.Thirst);
            item.stateSliders.WarmSliderUpdate(crew.Temperature);
        }
    }

    // 상태에 따른 멘트 반환
    private string GetStatusComment(CrewMember crew)
    {
        if (crew == null) return "";

        // CrewDialogues가 있으면 사용
        if (System.Type.GetType("CrewDialogues") != null)
        {
            return CrewDialogues.GetRandomDialogue(crew);
        }

        // 없으면 기본 멘트
        switch (crew.Status)
        {
            case CrewStatus.Healthy:
                return "배고프고, 목마르고, 춥지만...\n이제 잘만 견디어 보이야...";
            case CrewStatus.Poor:
                return "배고프고, 목마르고, 춥지만...\n버틸 수 있어...!";
            case CrewStatus.Critical:
                return "너무... 힘들어...\n이대로는 안 돼...";
            case CrewStatus.Dead:
                return "...";
            default:
                return "";
        }
    }

    // ========== 공개 메서드 ==========

    [ContextMenu("Test - Update Report")]
    public void TestUpdate()
    {
        UpdateAllInfo();
    }
}