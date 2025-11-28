using System;
using Unity.VisualScripting;
using UnityEngine;

public class DiverStatus : MonoBehaviour
{
    [Header("체력")]
    [SerializeField] private int _currentHp = 100;
    [SerializeField] private int _maxHp = 100;

    [Header("산소")]
    [SerializeField] private int _currentOxygen = 100;
    [SerializeField] private int _maxOxygen = 100;
    [SerializeField] private int _oxygenConsumePerSecond = 1;
    
    [Header("가방 UI")]
    [SerializeField] private DiverbagUI _bagUI;
    
    [Header("산소 고갈 시 체력 손실 설정")]
    [SerializeField] private float  _oxygenDepletedDamageInterval = 1f;   
    [SerializeField] private int _oxygenDepletedDamagePerTick = 5;  
    
    // 참조
    private Animator _animator;
    
    // 타이머
    private float _oxygenConsumeAccumulator = 0f; 
    private float _oxygenDepletedTimer = 0f;
   
    
    // 플래그
    private bool _isDead = false;
    
    // 가방
    private Inventory _diveBag  = new Inventory();
    
    // 프로퍼티
    public int MaxHp => _maxHp;
    public int CurrentHp => _currentHp;
    public int MaxOxygen => _maxOxygen;
    public int CurrentOxygen => _currentOxygen;

    public Inventory DiveBag => _diveBag;

    public bool IsDead => _isDead;


    private void Awake()
    {
        Init();
    }

    private void Update()
    {
        if (IsDead) return;

        HandleOxygen();
    }

    private void Init()
    {
        _animator = GetComponentInChildren<Animator>();
    }
    
    private void HandleOxygen()
    {
        // 산소 소모
        if (_currentOxygen > 0)
        {
            _oxygenConsumeAccumulator += _oxygenConsumePerSecond * Time.deltaTime;

            if (_oxygenConsumeAccumulator >= 1f)
            {
                int ticks = Mathf.FloorToInt(_oxygenConsumeAccumulator);
                if (ticks > 0)
                {
                    int totalConsume = ticks;
                    _currentOxygen = Mathf.Max(0, _currentOxygen - totalConsume);
                    _oxygenConsumeAccumulator -= ticks;
                }
            }

            _oxygenDepletedTimer = 0f;
        }
        
        // 체력 소모
        else
        {
            _oxygenDepletedTimer += Time.deltaTime;

            if (_oxygenDepletedTimer >= _oxygenDepletedDamageInterval)
            {
                int ticks = Mathf.FloorToInt(_oxygenDepletedTimer / _oxygenDepletedDamageInterval);
                if (ticks > 0)
                {
                    int totalDamage = ticks * _oxygenDepletedDamagePerTick;
                    _oxygenDepletedTimer -= ticks * _oxygenDepletedDamageInterval;
                    TakeDamage(totalDamage);
                }
            }
        }
    }

    public void TakeDamage(int  amount)
    {
        if (IsDead) return;
        if (amount <= 0) return;

        _currentHp -= amount;

        if (_currentHp <= 0)
        {
            _currentHp = 0;
            Die();
        }

        // TODO: HP UI 갱신
    }

    public void Heal(int amount)
    {
        if (IsDead) return;
        if (amount <= 0) return;

        _currentHp = Mathf.Clamp(_currentHp + amount, 0, _maxHp);
        
        // TODO: HP UI 갱신
    }

    public void RestoreOxygen(int amount)
    {
        if (amount <= 0f) return;

        _currentOxygen = Mathf.Clamp(_currentOxygen + amount, 0, _maxOxygen);

        // 산소가 다시 차면, 산소 고갈 데미지 타이머 리셋
        if (_currentOxygen > 0f)
        {
            _oxygenDepletedTimer = 0f;
        }

        // TODO: 산소 UI 갱신
    }

    private void Die()
    {
        if (IsDead) return;
        _isDead = true;
        
        _animator.SetTrigger("Die");
        // DropAllItems();
    }

    /*
     보류
     private void DropAllItems()
    {
        foreach (var kvp in _diveBag.Items)
        {
            ResourceType type = kvp.Key;
            int amount = kvp.Value;

            // TODO: type/amount를 기반으로 드랍 프리팹 생성
            // 예: DropManager.Instance.Spawn(type, amount, transform.position);
        }

        _diveBag.Clear(); 
    }*/

    
    public void GainResource(ResourceType type, int amount = 1)
    {
        DiveBag.Add(type, amount);

        if (_bagUI != null)
        {
            _bagUI.Refresh();
        }
    }
    
}
