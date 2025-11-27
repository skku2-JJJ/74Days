using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

/// <summary>
/// 화면 페이드 인/아웃 관리
/// DontDestroyOnLoad로 모든 씬에서 사용
/// 테마 색상: 수중(파란색), 배(검은색)
/// </summary>
public class FadeManager : MonoBehaviour
{
    public static FadeManager Instance { get; private set; }

    [Header("Fade Settings")]
    [SerializeField] private Image _fadeOverlay;  // 페이드용 검은/파란 이미지
    [SerializeField] private Canvas _fadeCanvas;

    [Header("Fade Colors")]
    [SerializeField] private Color _blackColor = new Color(0f, 0f, 0f, 1f);  // 검은색

    [Header("Default Duration")]
    [SerializeField] private float _defaultFadeDuration = 0.7f;

    private bool _isFading = false;

    void Awake()
    {
        // 싱글톤
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // 초기 상태: 완전히 투명
            if (_fadeOverlay != null)
            {
                _fadeOverlay.color = new Color(0, 0, 0, 0);
            }

            Debug.Log("[FadeManager] 초기화 완료 - DontDestroyOnLoad");
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // ========== 공개 메서드 ==========

    /// <summary>
    /// 화면을 특정 색상으로 페이드 아웃
    /// </summary>
    public void FadeOut(Color targetColor, float duration, System.Action onComplete = null)
    {
        if (_isFading)
        {
            Debug.LogWarning("[FadeManager] 이미 페이드 진행 중");
            return;
        }

        _isFading = true;

        // 색상 설정 (알파값 0으로 시작)
        targetColor.a = 0f;
        _fadeOverlay.color = targetColor;

        // 알파값만 1로 (불투명하게)
        targetColor.a = 1f;

        _fadeOverlay.DOColor(targetColor, duration)
            .SetEase(Ease.InOutQuad)
            .OnComplete(() =>
            {
                _isFading = false;
                Debug.Log($"[FadeManager] 페이드 아웃 완료 - 색상: {targetColor}");
                onComplete?.Invoke();
            });

        Debug.Log($"[FadeManager] 페이드 아웃 시작 - 색상: {targetColor}, 시간: {duration}초");
    }

    /// <summary>
    /// 화면을 투명하게 페이드 인
    /// </summary>
    public void FadeIn(float duration, System.Action onComplete = null)
    {
        if (_isFading)
        {
            Debug.LogWarning("[FadeManager] 이미 페이드 진행 중");
            return;
        }

        _isFading = true;

        // 현재 색상에서 알파값만 0으로
        Color transparentColor = _fadeOverlay.color;
        transparentColor.a = 0f;

        _fadeOverlay.DOColor(transparentColor, duration)
            .SetEase(Ease.InOutQuad)
            .OnComplete(() =>
            {
                _isFading = false;
                Debug.Log("[FadeManager] 페이드 인 완료");
                onComplete?.Invoke();
            });

        Debug.Log($"[FadeManager] 페이드 인 시작 - 시간: {duration}초");
    }

    // ========== 편의 메서드 ==========

    /// <summary>
    /// 검은색으로 페이드 아웃 (배로 돌아갈 때)
    /// </summary>
    public void FadeOutToBlack(float duration = -1f, System.Action onComplete = null)
    {
        if (duration < 0) duration = _defaultFadeDuration;
        FadeOut(_blackColor, duration, onComplete);
    }
}
