using UnityEngine;

/// <summary>
/// 물고기 추상 클래스
/// </summary>
public abstract class FishBase : MonoBehaviour
{
    [Header("체력 / 포획 조건")]
    [SerializeField] protected float health;
    [SerializeField] protected float maxHealth;
    [SerializeField, Range(0f, 1f)] protected float captureHpRatioThreshold = 0.5f;
    
    public bool CanBeCaptured => health <= maxHealth * captureHpRatioThreshold;

    public abstract void TakeHarpoonHit(float damage, Vector2 harpoonDir);
    public abstract void Capture();
    public abstract void OnCaptureFailed();
}
