using System;
using System.Collections;
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
    [SerializeField] private float _oxygenConsumePerSecond = 1.6f;
    
    [Header("가방 UI")]
    [SerializeField] private DiverbagUI _bagUI;
    
    [Header("산소 고갈 시 체력 손실 설정")]
    [SerializeField] private float  _oxygenDepletedDamageInterval = 1f;   
    [SerializeField] private int _oxygenDepletedDamagePerTick = 10;

    [Header("UI")]
    [SerializeField] private GetItemUIUpdate _getUI;

    [Header("SFX 참조")]
    [SerializeField] private UnderwaterSFXManager _sfxManager;
    // 참조
    private Animator _animator;
    private DiverVFXController _diverVFXController;
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
        _diverVFXController = GetComponent<DiverVFXController>();
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
                _currentOxygen = Mathf.Max(0, _currentOxygen - ticks);
                _oxygenConsumeAccumulator -= ticks;
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
                int totalDamage = ticks * _oxygenDepletedDamagePerTick;
                _oxygenDepletedTimer -= ticks * _oxygenDepletedDamageInterval;
                TakeDamage(totalDamage);
            }
        }
    }

    public void TakeDamage(int amount = 10)
    {
        if (IsDead) return;
        if (amount <= 0) return;

        _currentHp -= amount;

        if (_currentHp <= 0)
        {
            _currentHp = 0;
            Die();
            return;
        }
        
        _animator?.SetTrigger("Hit");
        
    }

    public void Heal(int amount)
    {
        if (IsDead) return;
        if (amount <= 0) return;

        _currentHp = Mathf.Clamp(_currentHp + amount, 0, _maxHp);
        
    }

    public void RestoreOxygen(int amount)
    {
        if (amount <= 0) return;

        _currentOxygen = Mathf.Clamp(_currentOxygen + amount, 0, _maxOxygen);

        // 산소가 다시 차면, 산소 고갈 데미지 타이머 리셋
        if (_currentOxygen > 0f)
        {
            _oxygenDepletedTimer = 0f;
        }
        
    }

    private void Die()
    {
        if (IsDead) return;
        _isDead = true;

        _animator.SetTrigger("Die");

        // 사망 시퀀스 시작
        StartCoroutine(HandleDeathSequence());
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
        _getUI.UIUpdate(type); //Get UI update

        DiveBag.Add(type, amount);

        if (_bagUI != null)
        {
            _bagUI.Refresh();
        }
        
        _diverVFXController?.PlayResourceGetVFX();
        _sfxManager?.Play(ESfx.ResourcePickup, false);
    }

    /// <summary>
    /// 플레이어 사망 시 실행되는 시퀀스
    /// DiverBag 비우기 → 애니메이션 대기 → Fade Out → Ship Scene 전환
    /// </summary>
    private IEnumerator HandleDeathSequence()
    {
        Debug.Log("[DiverStatus] 플레이어 사망 - 사망 시퀀스 시작");

        // 1. DiverBag 즉시 비우기 (모든 아이템 손실)
        _diveBag.Clear();
        Debug.Log("[DiverStatus] DiverBag 비워짐 - 모든 아이템 손실");

        // 2. DayManager의 오늘 수확량도 초기화 (UI 혼란 방지)
        if (DayManager.Instance != null)
        {
            DayManager.Instance.ClearTodayHarvest();
        }

        // 3. 사망 애니메이션 재생 대기 (1.5초)
        yield return new WaitForSeconds(2f);

        // 4. Ship Scene으로 전환 (DiverBag은 이미 비어있음)
        if (SceneTransitionManager.Instance != null)
        {
            SceneTransitionManager.Instance.GoToShip();
        }
        else
        {
            Debug.LogError("[DiverStatus] SceneTransitionManager를 찾을 수 없습니다!");
        }
    }

}
