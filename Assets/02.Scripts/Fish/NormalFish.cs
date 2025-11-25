using System;
using DG.Tweening;
using UnityEngine;

public class NormalFish : FishBase
{
    [Header("참조")]
    //[SerializeField] private Transform _visualTransform;
    
    [Header("Hit Effect Settings")]
    [SerializeField] private float _flashDuration = 0.07f;

    [SerializeField] private float _scalePunch = 0.15f;
    [SerializeField] private float _scalePunchDuration = 0.12f;

    [SerializeField] private float _knockbackDistance = 0.35f;
    [SerializeField] private float _knockbackDuration = 0.1f;

    [SerializeField] private float _hitShakeStrength = 0.1f;
    [SerializeField] private float _hitShakeDuration = 0.12f;

    private SpriteRenderer _spriteRenderer;
    
    private Vector3 _originalScale;
    private Tweener _flashTween, _shakeTween;
    
    private void Awake()
    {
       Init();
    }

    private void Init()
    {
        health = maxHealth;
        
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        _originalScale = transform.localScale;
    }
    public override void TakeHarpoonHit(float damage,Vector2 harpoonDir)
    {
        health = Mathf.Max(0, health - damage);
        
        // TODO : 피격 연출
        ApplyHitVisualEffect();
        ApplyHitPhysics(harpoonDir);
    }
    

    
    public override void Capture()
    {
        // 포획 처리
        // TODO : 인벤토리 추가 ,이펙트 
        Debug.Log($"{name} captured!");
        gameObject.SetActive(false);
    }

    public override void OnCaptureFailed()
    {
        Debug.Log($"{name} escaped!");
    }

    private void ApplyHitVisualEffect()
    {
        // ========================
        // (A) 색 반짝(Hit Flash)
        // ========================
        _flashTween?.Kill();
        Color original = _spriteRenderer.color;
        _spriteRenderer.color = Color.white;

        _flashTween = _spriteRenderer
            .DOColor(original, _flashDuration)
            .SetEase(Ease.OutQuad);

        // ========================
        // (B) 스케일 펀치
        // ========================
        transform.DOKill(true); // scale 관련 tween clean
        transform.localScale = _originalScale;
        transform.DOPunchScale(
            Vector3.one * _scalePunch,
            _scalePunchDuration,
            vibrato: 8,
            elasticity: 0.4f
        );
    }

    private void ApplyHitPhysics(Vector2 harpoonDir)
    {
        // ========================
        // (C) 넉백(살짝 튕김)
        // ========================
        Vector3 knockDir = -(Vector3)harpoonDir.normalized * _knockbackDistance;

        transform.DOMove(
            transform.position + knockDir,
            _knockbackDuration
        ).SetEase(Ease.OutQuad);

        // ========================
        // (F) 작살 박힌 위치 기준 흔들림(Shake)
        // ========================
        _shakeTween?.Kill();
        _shakeTween = transform
            .DOShakePosition(
                _hitShakeDuration,
                strength: _hitShakeStrength,
                vibrato: 18,
                randomness: 90,
                snapping: false,
                fadeOut: true
            );
    }
}
