using DG.Tweening;
using UnityEngine;

public class FishHitFeedback : MonoBehaviour
{
    [Header("Hit Effect Settings")]
    [SerializeField] protected float flashDuration = 0.07f;

    [SerializeField] protected float scalePunch = 0.15f;
    [SerializeField] protected float scalePunchDuration = 0.12f;

    [SerializeField] protected float knockbackDistance = 0.35f;
    [SerializeField] protected float knockbackDuration = 0.1f;

    [SerializeField] protected float hitShakeStrength = 0.1f;
    [SerializeField] protected float hitShakeDuration = 0.12f;
    
    // 참조
    protected SpriteRenderer spriteRenderer;
    protected Vector3 originalScale;
    protected Tweener flashTween, shakeTween;
    
    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        originalScale = transform.localScale;
    }

    /// <summary>
    /// 피격 피드백
    /// </summary>
    /// <param name="harpoonDir"></param>
    public void Play(Vector2 harpoonDir)
    {
        PlayFlash();
        PlayPunchScale();
        PlayKnockback(harpoonDir);
        PlayShake();
    }

    public void PlayKnockback(Vector2 harpoonDir)
    {
        // 넉백
        Vector3 knockDir = -(Vector3)harpoonDir.normalized * knockbackDistance;

        transform.DOMove(
            transform.position + knockDir,
            knockbackDuration
        ).SetEase(Ease.OutQuad);
    }

    public void PlayShake()
    {
        // 작살 위치 기준 흔들림
        shakeTween?.Kill();
        shakeTween = transform
            .DOShakePosition(
                hitShakeDuration,
                strength: hitShakeStrength,
                vibrato: 18,
                randomness: 90,
                snapping: false,
                fadeOut: true
            );
    }

    public void PlayPunchScale()
    {
        // 스케일 펀치
        transform.DOKill(true);
        transform.localScale = originalScale;
        transform.DOPunchScale(
            Vector3.one * scalePunch,
            scalePunchDuration,
            vibrato: 8,
            elasticity: 0.4f
        );
    }

    public void PlayFlash()
    {
        // 색상 효과
        flashTween?.Kill();
        Color original = spriteRenderer.color;
        spriteRenderer.color = Color.white;

        flashTween = spriteRenderer
            .DOColor(original, flashDuration)
            .SetEase(Ease.OutQuad);
    }
    
    
}