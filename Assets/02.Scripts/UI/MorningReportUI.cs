using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

public class MorningReportUI : MonoBehaviour
{
    public static MorningReportUI Instance { get; private set; }

    [Header("Day Info")]
    [SerializeField] private TextMeshProUGUI _dayTextUI;

    [Header("Ship Status")]
    [SerializeField] private Slider _shipHpSliderUI;
    [SerializeField] private TextMeshProUGUI _shipHpTextUI;

    [Header("Resources")]
    [SerializeField] private TextMeshProUGUI _fishTextUI;
    [SerializeField] private TextMeshProUGUI _shellfishTextUI;
    [SerializeField] private TextMeshProUGUI _seaweedTextUI;
    [SerializeField] private TextMeshProUGUI _waterTextUI;
    [SerializeField] private TextMeshProUGUI _herbsTextUI;
    [SerializeField] private TextMeshProUGUI _woodTextUI;

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
    }

    void Start()
    {
        // 닫기 버튼 이벤트 등록
        if (_closeButtonUI != null)
        {
            _closeButtonUI.onClick.AddListener(Hide);
        }

        // DayManager 이벤트 구독
        if (DayManager.Instance != null)
        {
            DayManager.Instance.OnPhaseChange += OnPhaseChanged;
        }

        // 시작 시 숨기기
        Hide();
    }

    void OnDestroy()
    {
        // 이벤트 구독 해제
        if (DayManager.Instance != null)
        {
            DayManager.Instance.OnPhaseChange -= OnPhaseChanged;
        }
    }

    // 페이즈 변경 시 호출
    private void OnPhaseChanged(DayPhase phase)
    {
        if (phase == DayPhase.Morning)
        {
            Show();
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

        UpdateAllInfo();
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
        UpdateShipStatus();
        UpdateResources();
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

    // 배 상태 갱신
    private void UpdateShipStatus()
    {
        if (ShipManager.Instance == null) return;

        float hp = ShipManager.Instance.Ship.Hp;
        float maxHp = 100f;

        // 슬라이더 갱신
        if (_shipHpSliderUI != null)
        {
            _shipHpSliderUI.maxValue = maxHp;
            _shipHpSliderUI.value = hp;
        }

        // 텍스트 갱신
        if (_shipHpTextUI != null)
        {
            _shipHpTextUI.text = $"{hp:F0}/{maxHp:F0}";
        }
    }

    // 자원 현황 갱신
    private void UpdateResources()
    {
        if (ShipManager.Instance == null) return;

        // 각 자원 수량 가져오기
        int fish = ShipManager.Instance.GetResourceAmount(ResourceType.Fish);
        int shellfish = ShipManager.Instance.GetResourceAmount(ResourceType.Shellfish);
        int seaweed = ShipManager.Instance.GetResourceAmount(ResourceType.Seaweed);
        int water = ShipManager.Instance.GetResourceAmount(ResourceType.CleanWater);
        int herbs = ShipManager.Instance.GetResourceAmount(ResourceType.Herbs);
        int wood = ShipManager.Instance.GetResourceAmount(ResourceType.Wood);

        // 텍스트 갱신
        if (_fishTextUI != null) _fishTextUI.text = fish.ToString();
        if (_shellfishTextUI != null) _shellfishTextUI.text = shellfish.ToString();
        if (_seaweedTextUI != null) _seaweedTextUI.text = seaweed.ToString();
        if (_waterTextUI != null) _waterTextUI.text = water.ToString();
        if (_herbsTextUI != null) _herbsTextUI.text = herbs.ToString();
        if (_woodTextUI != null) _woodTextUI.text = wood.ToString();
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
