using UnityEngine;

/// <summary>
/// DayManager의 Phase 변경에 따라 Morning/Night 오브젝트의 애니메이터를 제어
/// Animator에 "isDay", "isEvening" 트리거가 있어야 함
/// </summary>
public class DayNightAnimationController : MonoBehaviour
{
    [Header("Animator References")]
    [SerializeField] private Animator _morningNightAnimator;

    [Header("Animator Triggers")]
    [SerializeField] private string _isDayTrigger = "Day";
    [SerializeField] private string _isEveningTrigger = "Night";

    void Awake()
    {
        // DayManager 이벤트 구독
        if (DayManager.Instance != null)
        {
            DayManager.Instance.OnPhaseChange += OnPhaseChanged;
            Debug.Log("[DayNightAnimationController] DayManager 이벤트 구독 완료");
        }
        else
        {
            Debug.LogError("[DayNightAnimationController] DayManager.Instance를 찾을 수 없습니다!");
        }
    }

    void Start()
    {
        // 초기 페이즈 설정
        if (DayManager.Instance != null)
        {
            OnPhaseChanged(DayManager.Instance.CurrentPhase);
            Debug.Log($"[DayNightAnimationController] Start() - 현재 Phase: {DayManager.Instance.CurrentPhase}");
        }
    }

    void OnDestroy()
    {
        // 이벤트 구독 해제
        if (DayManager.Instance != null)
        {
            DayManager.Instance.OnPhaseChange -= OnPhaseChanged;
        }
    }

    private void OnPhaseChanged(DayPhase phase)
    {
        if (_morningNightAnimator == null)
        {
            Debug.LogWarning("[DayNightAnimationController] Animator가 할당되지 않았습니다.");
            return;
        }

        switch (phase)
        {
            case DayPhase.Morning:
                // 낮 애니메이션
                SetDayAnimation();
                Debug.Log($"[DayNightAnimationController] {phase} - 낮 애니메이션 실행");
                break;

            case DayPhase.Evening:
                // 밤 애니메이션
                SetNightAnimation();
                Debug.Log($"[DayNightAnimationController] {phase} - 밤 애니메이션 실행");
                break;
        }
    }

    private void SetDayAnimation()
    {
        _morningNightAnimator.SetTrigger(_isDayTrigger);
    }

    private void SetNightAnimation()
    {
        _morningNightAnimator.SetTrigger(_isEveningTrigger);
    }

    // 수동으로 낮/밤 전환 (테스트용)
    [ContextMenu("Test - Switch to Day")]
    public void TestSwitchToDay()
    {
        SetDayAnimation();
    }

    [ContextMenu("Test - Switch to Night")]
    public void TestSwitchToNight()
    {
        SetNightAnimation();
    }
}