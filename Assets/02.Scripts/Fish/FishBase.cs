using UnityEngine;

public enum EFishType
{
    
}
/// <summary>
/// 물고기 추상 클래스
/// </summary>
public abstract class FishBase : MonoBehaviour, IFishCapturable
{
    [Header("Fish 타입")]
    [SerializeField] private EFishType _fishType;
    
    [Header("다이버")]
    [SerializeField] protected Transform diver;
    
    [Header("Visual")]
    [SerializeField] protected FishVisualController visualController;
    
    [Header("Core References")]
    [SerializeField] private FishHealth _health;
    [SerializeField] private FishHitFeedback _hitFeedback;
    [SerializeField] private FishCaptureStruggle _captureStruggle;
    
   
    
    public bool CanBeCaptured => _health.CanBeCaptured;
    public Transform Transform => this.transform;

    public EFishType FishType => _fishType;

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

    
    /// <summary>
    /// 물고기 retrieve 성공 시 호출
    /// </summary>
    public void Get()
    {
        // 인벤토리 추가, 이펙트 등
        Debug.Log($"{name} captured!");
        gameObject.SetActive(false);
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
       
    
}