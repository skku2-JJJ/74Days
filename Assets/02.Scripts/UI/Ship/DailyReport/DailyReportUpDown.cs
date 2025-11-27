using DG.Tweening;
using UnityEngine;

public class DailyReportUpDown : MonoBehaviour
{

    private RectTransform _reoportUI;

    [SerializeField]
    private RectTransform _crewsUI;

    private Vector2 _openPos = new Vector2(0, 0);
    private Vector2 _closePos = new Vector2(0, -1200);

    // UI 열림/닫힘 상태
    private bool _isOpen = false;
    public bool IsOpen => _isOpen;



    void Start()
    {
        _reoportUI = GetComponent<RectTransform>();
        _reoportUI.anchoredPosition = _closePos;

        // DayManager 페이즈 변경 이벤트 구독
        if (DayManager.Instance != null)
        {
            DayManager.Instance.OnPhaseChange += OnPhaseChanged;
        }
    }

    void OnDestroy()
    {
        // 이벤트 구독 해제
        if (DayManager.Instance != null)
        {
            DayManager.Instance.OnPhaseChange -= OnPhaseChanged;
        }

        CloseInternal();
    }

    /// <summary>
    /// 페이즈 변경 시 호출
    /// </summary>
    private void OnPhaseChanged(DayPhase phase)
    {
        if (phase == DayPhase.Morning)
        {
            // Morning 페이즈 시작 시 자동으로 열기
            Open();
            Debug.Log("[DailyReportUpDown] Morning 페이즈 - 아침 리포트 자동 열기");
        }
    }

    void Update()
    {

    }

    public void Open()
    {
        if (_isOpen) return; // 이미 열려있으면 무시
        UIManager.Instance.IsOpened = true;
        _isOpen = true;
        _crewsUI.DOAnchorPos(_closePos, 0.3f).SetEase(Ease.InSine).OnComplete(() =>
        {
            _reoportUI.DOAnchorPos(_openPos, 0.5f).SetEase(Ease.OutBack);
        });
    }

    public void Close()
    {
        if (!_isOpen) return; // 이미 닫혀있으면 무시
        UIManager.Instance.IsOpened = false;
        _isOpen = false;
        _reoportUI.DOAnchorPos(_closePos, 0.5f).SetEase(Ease.InBack).OnComplete(() =>
        {
            _crewsUI.DOAnchorPos(_openPos, 0.3f).SetEase(Ease.OutSine);
        });

    }

    private void CloseInternal()
    {
        if (!_isOpen) return; // 이미 닫혀있으면 무시
        if (UIManager.Instance == null)
        {
            Debug.LogWarning("UIManager가 존재하지 않습니다. Close()을 실행할 수 없습니다.");
            return;
        }

        _isOpen = false;

        _crewsUI.anchoredPosition = _closePos;
        UIManager.Instance.IsOpened = false;
    }
}
