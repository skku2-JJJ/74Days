using UnityEngine;
using DG.Tweening;

/// <summary>
/// 물고기 추상 클래스
/// </summary>
public abstract class FishBase : MonoBehaviour
{
    [Header("체력 / 포획 조건")]
    [SerializeField] protected float health;
    [SerializeField] protected float maxHealth;
    [SerializeField, Range(0f, 1f)] protected float captureHpRatioThreshold = 0.5f;
    
    [Header("Hit Effect Settings")]
    [SerializeField] protected float flashDuration = 0.07f;

    [SerializeField] protected float scalePunch = 0.15f;
    [SerializeField] protected float scalePunchDuration = 0.12f;

    [SerializeField] protected float knockbackDistance = 0.35f;
    [SerializeField] protected float knockbackDuration = 0.1f;

    [SerializeField] protected float hitShakeStrength = 0.1f;
    [SerializeField] protected float hitShakeDuration = 0.12f;
    
    // 참조
    // protected Transform visualTransform;
    
    protected SpriteRenderer spriteRenderer;
    protected Vector3 originalScale;
    protected Tweener flashTween, shakeTween;

    
    public bool CanBeCaptured => health <= maxHealth * captureHpRatioThreshold;

    public abstract void TakeHarpoonHit(float damage, Vector2 harpoonDir);
    public abstract void Capture();
    public abstract void OnCaptureFailed();
    
    protected void ApplyHitVisualEffect()
    {
        // 색상 효과
        flashTween?.Kill();
        Color original = spriteRenderer.color;
        spriteRenderer.color = Color.white;

        flashTween = spriteRenderer
            .DOColor(original, flashDuration)
            .SetEase(Ease.OutQuad);

    
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

    protected void ApplyHitPhysics(Vector2 harpoonDir)
    {
        // 넉백
        Vector3 knockDir = -(Vector3)harpoonDir.normalized * knockbackDistance;

        transform.DOMove(
            transform.position + knockDir,
            knockbackDuration
        ).SetEase(Ease.OutQuad);

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
    
}
