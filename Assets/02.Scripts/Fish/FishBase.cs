using UnityEngine;

/// <summary>
/// 물고기 추상 클래스
/// </summary>
public abstract class FishBase : MonoBehaviour
{
    [Header("체력 / 포획 조건")]
    [SerializeField] private float _health;
    [SerializeField] private float _maxHealth;
    [SerializeField, Range(0f, 1f)] private float _captureHpRatioThreshold = 0.5f;
    
    public bool CanBeCaptured => _health <= _maxHealth * _captureHpRatioThreshold;

    public abstract void TakeHarpoonHit(float damage);
    public abstract void Capture();
    public abstract void OnCaptureFailed();
}
