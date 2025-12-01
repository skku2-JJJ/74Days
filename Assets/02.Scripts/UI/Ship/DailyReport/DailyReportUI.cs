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
        public GameObject crewObject;           // Crew GameObject
        public TextMeshProUGUI nameText;        // Name
        public TextMeshProUGUI mentText;        // 대사
        public SliderUpdate stateSliders;   // StateSliders (슬라이더 컴포넌트)
        public Image crewImage;   // 이미지
    }

    void Awake()
    {
        // 싱글톤
        if (Instance == null)
        {
            Instance = this;
            Debug.Log("[DailyReportUI] Instance 생성됨");
        }
        else
        {
            Destroy(gameObject);
            Debug.LogWarning("[DailyReportUI] 중복 인스턴스 제거됨");
            return;
        }

        // DayManager 이벤트 구독
        if (DayManager.Instance != null)
        {
            DayManager.Instance.OnDayStart += OnDayStart;
            DayManager.Instance.OnPhaseChange += OnPhaseChanged;
            Debug.Log("[DailyReportUI] DayManager 이벤트 구독 완료");
        }
        else
        {
            Debug.LogError("[DailyReportUI] DayManager.Instance가 null입니다!");
        }
    }

    void Start()
    {
        Debug.Log($"[DailyReportUI] Start() 호출 - 현재 Phase: {DayManager.Instance?.CurrentPhase}");
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

    // 페이즈 변경 시 호출
    private void OnPhaseChanged(DayPhase phase)
    {
        Debug.Log($"[DailyReportUI] OnPhaseChanged 호출됨 - Phase: {phase}");

        // Morning과 Evening에 UI 갱신 (씬 전환 후 데이터 로드)
        if (phase == DayPhase.Morning || phase == DayPhase.Evening)
        {
            UpdateAllInfo();
            Debug.Log($"[Daily Report] {phase} 페이즈 - 데이터 업데이트");
        }
        else
        {
            Debug.Log($"[DailyReportUI] {phase} 페이즈는 업데이트 안함");
        }
    }

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

        //sprite 업데이트
        if (item.crewImage != null)
        {
            item.crewImage.sprite = crew.IsAlive ? crew.AliveSprite : crew.DeadSprite;
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

        return "";
    }

    // ========== 공개 메서드 ==========

    [ContextMenu("Test - Update Report")]
    public void TestUpdate()
    {
        UpdateAllInfo();
    }
}