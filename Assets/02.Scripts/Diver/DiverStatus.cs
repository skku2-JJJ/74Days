using System;
using Unity.VisualScripting;
using UnityEngine;

public class DiverStatus : MonoBehaviour
{
    [Header("체력")]
    [SerializeField] private float _currentHp = 100f;
    [SerializeField] private float _maxHp = 100f;

    [Header("산소")]
    [SerializeField] private float _currentOxygen = 100f;
    [SerializeField] private float _maxOxygen = 100f;
    [SerializeField] private float _oxygenConsumePerSecond = 1f;
    
    [Header("가방 UI")]
    [SerializeField] private DiverbagUI _bagUI;
    
    [Header("산소 고갈 시 체력 손실 설정")]
    [SerializeField] private float _oxygenDepletedDamageInterval = 1f;   
    [SerializeField] private float _oxygenDepletedDamagePerTick = 10f;  
    
    // 참조
    private DiverVisualController _visualController;
    
    // 타이머
    private float _oxygenDepletedTimer = 0f;
    
    // 플래그
    private bool _isDead = false;
    
    // 가방
    private Inventory _diveBag  = new Inventory();
    
    // 프로퍼티
    public float MaxHp => _maxHp;
    public float CurrentHp => _currentHp;
    public float MaxOxygen => _maxOxygen;
    public float CurrentOxygen => _currentOxygen;

    public Inventory DiveBag => _diveBag;

    public bool IsDead => _isDead;


    private void Awake()
    {
        
    }

    private void Update()
    {
        if (IsDead) return;

        HandleOxygen();
    }

    private void Init()
    {
        _visualController = GetComponentInChildren<DiverVisualController>();
    }
    
    private void HandleOxygen()
    {
        if (_currentOxygen > 0f)
        {
            float oxygenConsume = _oxygenConsumePerSecond * Time.deltaTime;
            _currentOxygen = Mathf.Max(0f, _currentOxygen - oxygenConsume);
        }

        // 산소가 0이면, 일정 간격으로 체력 손실
        if (_currentOxygen <= 0f)
        {
            _oxygenDepletedTimer += Time.deltaTime;

            if (_oxygenDepletedTimer >= _oxygenDepletedDamageInterval)
            {
                _oxygenDepletedTimer = 0f;
                TakeDamage(_oxygenDepletedDamagePerTick);
            }
        }
        else
        {
            // 산소가 다시 차면 타이머 리셋
            _oxygenDepletedTimer = 0f;
        }
    }

    public void TakeDamage(float amount)
    {
        if (IsDead) return;
        if (amount <= 0f) return;

        _currentHp -= amount;

        if (_currentHp <= 0f)
        {
            _currentHp = 0f;
            Die();
        }

        // TODO: HP UI 갱신
    }

    public void Heal(float amount)
    {
        if (IsDead) return;
        if (amount <= 0f) return;

        _currentHp = Mathf.Clamp(_currentHp + amount, 0f, _maxHp);
        
        // TODO: HP UI 갱신
    }

    public void RestoreOxygen(float amount)
    {
        if (amount <= 0f) return;

        _currentOxygen = Mathf.Clamp(_currentOxygen + amount, 0f, _maxOxygen);

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
        
        DropAllItems();
    }

    private void DropAllItems()
    {
        // 여기서 DiveBag에 있는 아이템들을 실제 월드에 드랍
        // Inventory에 Items 프로퍼티가 있다고 가정 (IReadOnlyDictionary<ResourceType, int> Items)

        foreach (var kvp in _diveBag.Items)
        {
            ResourceType type = kvp.Key;
            int amount = kvp.Value;

            // TODO: type/amount를 기반으로 드랍 프리팹 생성
            // 예: DropManager.Instance.Spawn(type, amount, transform.position);
        }

        _diveBag.Clear(); // 다 떨궜으니 가방 비우기
    }

    
    public void GainResource(ResourceType type, int amount = 1)
    {
        DiveBag.Add(type, amount);

        if (_bagUI != null)
        {
            _bagUI.Refresh();
        }
    }
    
}
