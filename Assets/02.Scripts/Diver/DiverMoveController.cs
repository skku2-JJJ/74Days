using System;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 수중 이동 컨트롤러
/// </summary>
[RequireComponent(typeof(Rigidbody2D), typeof(InputController), typeof(HarpoonShooter))]
public class DiverMoveController : MonoBehaviour
{
    [Header("움직임 관련 변수")]
    [SerializeField] private float _maxSpeed = 5f;          
    [SerializeField] private float _responsiveness = 4f; 

    [Header("부력 설정")]
    [SerializeField] private float _buoyancy = 0.4f;        
    [SerializeField] private float _maxVerticalSpeed = 4f;  

    [Header("부스트")]
    [SerializeField] private float _boostMultiplier = 1.8f; 
    [SerializeField] private float _boostDuration = 0.35f;  
    [SerializeField] private float _boostCoolTime = 1.0f;   
    
    [Header("몸체 반동")]
    [SerializeField] private float _recoilDamping = 6f;
    private Vector2 _recoilVelocity;                      
    
    // 상수
    private const float MinInputMagnitude = 0.01f;
    private const float RecoilEpsilon = 0.0001f;
    private const float BasicRecoilStrength = 1f;
    // 프로퍼티
    public Vector2 MoveInput => _moveInput;
    private bool IsMoving => _moveInput.sqrMagnitude > MinInputMagnitude; //입력 기준으로 이동 판단
    public bool IsBoosting => _isBoosting;
    
    public float BoostRemainRatio
    {
        get
        {
            if (!_isBoosting || _boostDuration <= 0f) return 0f;
            return 1f - Mathf.Clamp01(_boostTimer / _boostDuration);
        }
    }
    public float BoostCooldownRatio
    {
        get
        {
            if (_boostCoolTime <= 0f) return 1f;
            return Mathf.Clamp01(_boostCoolTimer / _boostCoolTime);
        }
    }
    
    // 컴포넌트
    private Rigidbody2D _rigid;
    
    // 참조
    private InputController _inputController;
    private HarpoonShooter _harpoonShooter; 
    private DiverStatus _diverStatus;
    
    private Vector2 _moveInput;
    
    // 부스트 관련
    private bool _isBoosting;
    private float _boostCoolTimer;
    private float _boostTimer;
    
    private void Awake()
    {
        Init();
    }

    private void Update()
    {
        if (_diverStatus.IsDead)
        {
            _moveInput = Vector2.zero;
            return;
        }
        
        GetMoveInput();
        HandleBoostState();
    }
    private void FixedUpdate()
    {
        ApplyMovement();
    }
    
    private void Init()
    {
        _rigid = GetComponent<Rigidbody2D>();
        _inputController = GetComponent<InputController>();
        _harpoonShooter = GetComponent<HarpoonShooter>();
        _diverStatus = GetComponent<DiverStatus>();
    }

    private void GetMoveInput()
    {
        if (_harpoonShooter.IsAiming || _harpoonShooter.HasHarpoonOut)
        {
            _moveInput = Vector2.zero;
            return;
        }
        
        float x = _inputController.XMove;
        float y = _inputController.YMove;  

        _moveInput = new Vector2(x, y);
        
    }

    private void HandleBoostState()
    {
        if (_harpoonShooter.IsAiming)
        {
            _isBoosting = false;   
            _boostTimer = 0f;
            return;
        }
        
        bool isBoostHeld      = _inputController.IsBoostKeyHeld;
        bool isBoostPressed   =  _inputController.IsBoostKeyPressed; 

        
        if (_isBoosting)
        {
            _boostTimer += Time.deltaTime;

            // 부스트 종료
            if (_boostTimer >= _boostDuration)
            {
                _isBoosting = false;
                _boostTimer = 0f;
                _boostCoolTimer = 0f;  
            }

            return;
        }

        // 부스트 중이 아니면 키를 떼고 있을 때만 쿨타임 증가
        if (!isBoostHeld)
        {
            _boostCoolTimer += Time.deltaTime;
        }
        
        if (_boostCoolTimer >= _boostCoolTime &&
            isBoostPressed &&
            IsMoving)
        {
            _isBoosting = true;
            _boostTimer = 0f;
        }
    }

    private void ApplyMovement()
    {
        Vector2 currentVel = _rigid.linearVelocity;

        Vector2 moveInputDir = _moveInput;
        if (moveInputDir.sqrMagnitude > 1f)
        {
            moveInputDir = moveInputDir.normalized;
        }
           
        
        float applySpeed = _isBoosting ? (_maxSpeed * _boostMultiplier) : _maxSpeed;
        
        Vector2 targetVel = moveInputDir * applySpeed;

        // 지수 감쇠 Lerp
        float t = 1f - Mathf.Exp(-_responsiveness * Time.fixedDeltaTime);
        currentVel = Vector2.Lerp(currentVel, targetVel, t);

        // 부력 적용
        currentVel.y += _buoyancy * Time.fixedDeltaTime;
        currentVel.y = Mathf.Clamp(currentVel.y, -_maxVerticalSpeed, _maxVerticalSpeed);
        
        // 반동 적용
        if (_recoilVelocity.sqrMagnitude > RecoilEpsilon)
        {
            currentVel += _recoilVelocity;

            _recoilVelocity = Vector2.MoveTowards(
                _recoilVelocity,
                Vector2.zero,
                _recoilDamping * Time.fixedDeltaTime
            );
        }
        
        _rigid.linearVelocity = currentVel;
        
    }
    
    /// <summary>
    /// 외부에서 호출하여 플레이어 몸체에 반동 적용
    /// </summary>
    /// <param name="direction"> 반동 방향 </param>
    /// <param name="strength"> 반동 강도 </param>
    public void AddRecoil(Vector2 direction, float strength = BasicRecoilStrength)
    {
        if (strength <= 0f || direction.sqrMagnitude < RecoilEpsilon)
            return;

        direction.Normalize();
        
        _recoilVelocity += direction * strength;
    }

    
    
#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (_rigid == null) return;
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, transform.position + (Vector3)_rigid.linearVelocity);
    }
#endif
}
