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
    
    [Header("QTE 캡처 관련")]
    [SerializeField] protected float escapeMoveSpeed = 3f;   // 도망 속도
    [SerializeField] protected float maxStruggleAngle = 15f; // 몸통 좌우 흔들 각도
    [SerializeField] protected float struggleFrequency = 7f; // 초당 흔들림 횟수
    
    // 참조
    [SerializeField] protected Transform visualTransform;
    
    protected bool isCapturedByHarpoon;
    protected Transform capturedDiver;
    protected float currentStruggleIntensity; // 0~1 (QTE 게이지 참고해서 세기 조절)

    protected Rigidbody2D rigid;
    protected Animator animator;

    
    
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
    
    // QTE 시작 시 호출
    public void BeginCaptureStruggle(Transform diver)
    {
        isCapturedByHarpoon = true;
        capturedDiver = diver;
        currentStruggleIntensity = 0.5f; // 시작 기본값

        
        animator?.SetTrigger("Hit"); 
        
    }

    // QTE 진행 중: 게이지 비율(0~1)에 따라 버둥 세기 조절
    public void UpdateCaptureStruggle(float struggle01)
    {
        currentStruggleIntensity = Mathf.Clamp01(struggle01);
        
    }

    // QTE 종료 시
    public void EndCaptureStruggle()
    {
        isCapturedByHarpoon = false;
        capturedDiver = null;
        currentStruggleIntensity = 0f;

       
        animator?.SetTrigger("Escape"); 
        
        /*// 회전/속도 복원
        visualTransform.localRotation = Quaternion.identity;
        rigid.linearVelocity = Vector2.zero;*/
        
    }

    /// <summary>
    /// 잡혔을 때 HarpoonShooter에서 호출
    /// </summary>
    public void Captured()
    {
        rigid.simulated = false;
    }
    protected void UpdateCapturedMovement()
    {
        if (capturedDiver == null) return;
        
        Vector2 dirAway = (transform.position - capturedDiver.position).normalized;
        
        float speed = escapeMoveSpeed * (0.5f + currentStruggleIntensity);
        rigid.linearVelocity = dirAway * speed;
    }

    protected void UpdateCapturedWiggle()
    {
        // 몸 흔들기 (좌우 회전)
        float t = Time.time * struggleFrequency;
        float angle = Mathf.Sin(t) * maxStruggleAngle * currentStruggleIntensity;
        visualTransform.localRotation = Quaternion.Euler(0f, 0f, angle);
    }
    
}
