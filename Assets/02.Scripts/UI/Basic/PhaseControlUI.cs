using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PhaseControlUI : MonoBehaviour
{
    [Header("Phase Display")]
    [SerializeField] private TextMeshProUGUI _currentPhaseTextUI;

    [Header("Phase Control Buttons")]
    [SerializeField] private Button _goToDivingButtonUI;
    [SerializeField] private Button _goToEveningButtonUI;

    [Header("Panel")]
    [SerializeField] private GameObject _panelRootUI;

    void Start()
    {
        // 버튼 이벤트 등록
        if (_goToDivingButtonUI != null)
        {
            _goToDivingButtonUI.onClick.AddListener(OnGoToDivingButtonClicked);
        }

        if (_goToEveningButtonUI != null)
        {
            _goToEveningButtonUI.onClick.AddListener(OnGoToEveningButtonClicked);
        }

        // DayManager 이벤트 구독
        if (DayManager.Instance != null)
        {
            DayManager.Instance.OnPhaseChange += OnPhaseChanged;
        }

        // 초기 상태 업데이트
        UpdatePhaseDisplay();
        UpdateButtonStates();
    }

    void OnDestroy()
    {
        if (DayManager.Instance != null)
        {
            DayManager.Instance.OnPhaseChange -= OnPhaseChanged;
        }
    }

    // ========== 페이즈 변경 이벤트 ==========

    private void OnPhaseChanged(DayPhase phase)
    {
        UpdatePhaseDisplay();
        UpdateButtonStates();
    }

    // ========== 버튼 이벤트 ==========

    private void OnGoToDivingButtonClicked()
    {
        if (DayManager.Instance != null)
        {
            DayManager.Instance.GoToDiving();
        }
    }

    private void OnGoToEveningButtonClicked()
    {
        if (DayManager.Instance != null)
        {
            DayManager.Instance.GoToEvening();
        }
    }

    // ========== UI 갱신 ==========

    private void UpdatePhaseDisplay()
    {
        if (_currentPhaseTextUI == null || DayManager.Instance == null) return;

        string phaseText = DayManager.Instance.CurrentPhase switch
        {
            DayPhase.Morning => "아침",
            DayPhase.Diving => "잠수 중",
            DayPhase.Evening => "저녁",
            DayPhase.Night => "밤",
            _ => "알 수 없음"
        };

        _currentPhaseTextUI.text = phaseText;
    }

    private void UpdateButtonStates()
    {
        if (DayManager.Instance == null) return;

        DayPhase currentPhase = DayManager.Instance.CurrentPhase;

        // "잠수 시작" 버튼은 Morning 페이즈에서만 활성화
        if (_goToDivingButtonUI != null)
        {
            _goToDivingButtonUI.interactable = (currentPhase == DayPhase.Morning);
        }

        // "배로 복귀" 버튼은 Diving 페이즈에서만 활성화
        if (_goToEveningButtonUI != null)
        {
            _goToEveningButtonUI.interactable = (currentPhase == DayPhase.Diving);
        }
    }

    // ========== 팝업 제어 ==========

    public void Show()
    {
        if (_panelRootUI != null)
        {
            _panelRootUI.SetActive(true);
        }

        UpdatePhaseDisplay();
        UpdateButtonStates();
    }

    public void Hide()
    {
        if (_panelRootUI != null)
        {
            _panelRootUI.SetActive(false);
        }
    }
}
