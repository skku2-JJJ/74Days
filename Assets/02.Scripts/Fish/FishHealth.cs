using UnityEngine;
using UnityEngine.Events;

public class FishHealth : MonoBehaviour
{
    [Header("체력 / 포획 조건")]
    [SerializeField] protected float health;
    [SerializeField] protected float maxHealth;
    [SerializeField, Range(0f, 1f)] protected float captureHpRatioThreshold = 0.1f;
    
    // 이벤트
    public UnityEvent OnDamaged;
    public UnityEvent OnDead;
    
    // 프로퍼티
    public float Health => health;
    public float MaxHealth => maxHealth;
    public bool CanBeCaptured => health <= maxHealth * captureHpRatioThreshold;

    private void Awake()
    {
        health = maxHealth;
    }
    
    public void TakeDamage(float damage)
    {
        health = Mathf.Max(0, Health - damage);
        OnDamaged?.Invoke();
        
        if (Health == 0)
            OnDead?.Invoke();
    }
}