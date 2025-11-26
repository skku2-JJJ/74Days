using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class MorningReportUI : MonoBehaviour
{
    public static MorningReportUI Instance { get; private set; }

    [Header("Day Info")]
    [SerializeField] private TextMeshProUGUI _dayTextUI;

    [Header("Crew List")]
    [SerializeField] private Transform _crewListParentUI;
    [SerializeField] private GameObject _crewStatusItemPrefabUI;

    [Header("Panel")]
    [SerializeField] private GameObject _panelRootUI;
    [SerializeField] private Button _closeButtonUI;

    private List<CrewStatusItem> _crewStatusItems = new List<CrewStatusItem>();

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

        // DayManager 이벤트 구독 (Start보다 먼저 실행되는 Awake에서)
        if (DayManager.Instance != null)
        {
            DayManager.Instance.OnDayStart += OnDayStart;
            DayManager.Instance.OnPhaseChange += OnPhaseChanged;
        }
    }

    void Start()
    {
        // 닫기 버튼 이벤트 등록
        if (_closeButtonUI != null)
        {
            _closeButtonUI.onClick.AddListener(Hide);
        }

        // 시작 시 숨기기
        // Hide();
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

    // 새 날 시작 시 호출
    private void OnDayStart(int day)
    {
        UpdateAllInfo();
        Show();
    }

    // 페이즈 변경 시 호출
    private void OnPhaseChanged(DayPhase phase)
    {
        // Morning 페이즈가 아니면 패널 닫기
        if (phase != DayPhase.Morning)
        {
            Hide();
        }
    }

    // ========== 팝업 제어 ==========

    // 팝업 표시
    public void Show()
    {
        if (_panelRootUI != null)
        {
            _panelRootUI.SetActive(true);
        }

        Debug.Log("[아침 리포트] 팝업 표시");
    }

    // 팝업 숨기기
    public void Hide()
    {
        if (_panelRootUI != null)
        {
            _panelRootUI.SetActive(false);
        }

        Debug.Log("[아침 리포트] 팝업 닫힘");
    }

    // ========== 정보 갱신 ==========

    // 모든 정보 갱신
    public void UpdateAllInfo()
    {
        UpdateDayInfo();
        UpdateCrewList();
    }

    // 날짜 정보 갱신
    private void UpdateDayInfo()
    {
        if (_dayTextUI == null) return;

        if (DayManager.Instance != null)
        {
            int day = DayManager.Instance.CurrentDay;
            _dayTextUI.text = $"Day {day} - 아침";
        }
        else
        {
            _dayTextUI.text = "Day ? - 아침";
        }
    }

    // 선원 리스트 갱신
    private void UpdateCrewList()
    {
        if (_crewListParentUI == null || _crewStatusItemPrefabUI == null) return;
        if (CrewManager.Instance == null) return;

        // 기존 아이템 제거
        ClearCrewList();

        // 선원 데이터로 아이템 생성
        foreach (var crew in CrewManager.Instance.CrewMembers)
        {
            GameObject itemObj = Instantiate(_crewStatusItemPrefabUI, _crewListParentUI);
            CrewStatusItem item = itemObj.GetComponent<CrewStatusItem>();

            if (item != null)
            {
                item.SetCrewData(crew);
                _crewStatusItems.Add(item);
            }
        }
    }

    // 선원 리스트 초기화
    private void ClearCrewList()
    {
        foreach (var item in _crewStatusItems)
        {
            if (item != null)
            {
                Destroy(item.gameObject);
            }
        }
        _crewStatusItems.Clear();
    }
}
