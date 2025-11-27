using UnityEngine;

public interface IFishCapturable
{
    bool CanBeCaptured { get; }
    ResourceType Type { get; }
    // 유니티 Transform 노출
    Transform Transform { get; }
    
    
    // 피격, 포획 처리 메서드 ---------------------------------------------------------
    /// <summary>
    /// 투사체 Hit 시 호출
    /// </summary>
    /// <param name="damage"> 적용 데미지 </param>
    /// <param name="harpoonDir"> 투사체 방향 </param>
    void TakeHarpoonHit(float damage, Vector2 harpoonDir);
    void OnCapture();
    void OnCaptureFailed();

    void Stored();
    
    // -------------------------------------------------------------------------------
    
    // QTE 처리 ---------------------------------------------------------------------
    
    /// <summary>
    /// QTE 시작 시 호출
    /// </summary>
    /// <param name="diver"></param>
    void BeginCaptureStruggle();
    
    /// <summary>
    /// QTE 진행 동안 호출
    /// </summary>
    /// <param name="struggle01"></param>
    void UpdateCaptureStruggle(float struggle01);
    
    /// <summary>
    /// QTE 종료 시 호출
    /// </summary>
    void EndCaptureStruggle();
    
    // -------------------------------------------------------------------------------
}