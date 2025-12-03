using System;
using UnityEngine;

/// <summary>
/// 물고기 추상 클래스
/// </summary>
public abstract class FishBase : MonoBehaviour, IFishCapturable
{
    [Header("Fish 타입")]
    [SerializeField] private ResourceType _fishType;
    
    [Header("QTE 난이도"), Range(0f, 1f)]
    [SerializeField] private float _qteDifficulty01 = 0.5f;
    
    // 참조
    protected Transform diver;
    protected FishVisualController visualController;
    private FishHealth _health;
    private FishHitFeedback _hitFeedback;
    private FishCaptureStruggle _captureStruggle;
  
   
    
    public bool CanBeCaptured => _health.CanBeCaptured;
    public ResourceType FishType => _fishType;
    public Transform Transform => this.transform;
    public float BaseQteDifficulty01 => _qteDifficulty01;
    
    /// <summary>
    /// 현재 체력 비례 QTE 난이도
    /// </summary>
    public float CurrentQteDifficulty01
    {
        get
        {
            if (_health == null) return _qteDifficulty01;

            float ratio = _health.HealthRatio;
            float scaled = _qteDifficulty01 * ratio;
            
            const float minDifficulty = 0.1f;
            return Mathf.Clamp(scaled, minDifficulty, _qteDifficulty01);
        }
    }
    
    
    protected void Awake()
    {
       Init();
    }

    public virtual void TakeHarpoonHit(float damage, Vector2 harpoonDir)
    {
        _health.TakeDamage(damage);
        _hitFeedback?.Play(harpoonDir);
        
    }

    /// <summary>
    /// QTE 포획 성공 판정 시 호출
    /// </summary>
    public abstract void OnCapture();
   
 
    /// <summary>
    /// QTE 포획 실패 판정 시 호출
    /// </summary>
    public abstract void OnCaptureFailed();

    public void Stored()
    {
        //Debug.Log($"{name} is Stored");
        Destroy(gameObject);
    }

    public void BeginCaptureStruggle()
    {
        _captureStruggle?.Begin(diver);


        if (visualController != null)
        {
            visualController.ForceLookAwayFrom(diver.position, true);
        }
       
    }
    
    public void UpdateCaptureStruggle(float struggle01) =>
        _captureStruggle?.UpdateIntensity(struggle01);

    public void EndCaptureStruggle()
    {
        _captureStruggle?.End();
        
        if (visualController != null)
        {
            visualController.ForceLookAwayFrom(diver.position, false);
        }
    }

    private void Init()
    {
        _health = GetComponent<FishHealth>();
        _hitFeedback = GetComponent<FishHitFeedback>();
        _captureStruggle = GetComponent<FishCaptureStruggle>();
    }
    
    // 물고기 스폰 관련 --------------------------------------------
    private void OnEnable()
    {
        if (FishSpawnManager.Instance != null)
        {
            FishSpawnManager.Instance.RegisterFish(this);
        }
    }

    private void OnDisable()
    {
        if (FishSpawnManager.Instance != null)
        {
            FishSpawnManager.Instance.UnregisterFish(this);
        }
    }
    
    // -----------------------------------------------------------
}