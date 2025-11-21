using System;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 수중 이동 컨트롤러
/// </summary>
public class DiverMoveController : MonoBehaviour
{
    [Header("움직임 관련 변수")]
    [SerializeField] private float _maxSpeed = 5f;          
    [SerializeField] private float _responsiveness = 4f; 

    [Header("부력 설정")]
    [SerializeField] private float _buoyancy = 0.4f;        // 부력
    [SerializeField] private float _maxVerticalSpeed = 4f;  

    [Header("부스트")]
    [SerializeField] private float _boostMultiplier = 1.8f; 
    [SerializeField] private float _boostDuration = 0.35f;  
    [SerializeField] private float _boostCoolTime = 1.0f;   
    
    [Header("애니메이터 설정")]
    [SerializeField] private float _animSpeedLerp = 10f; // Locomotion 보간
    [SerializeField] private float _turnInputThreshold = 0.2f; //방향 전환 애니메이션 시 최소 입력값
    [SerializeField] private float _turnFlipTime = 0.5f; // Turn 애니의 몇 % 지점에서 flip할지 (0~1)
    private static readonly int TurnStateHash = Animator.StringToHash("Base Layer.Turn"); // Turn State의 해시값
    
    // 프로퍼티
    public bool IsMoving => _moveInput.sqrMagnitude > 0.01f;
    
    // 컴포넌트
    private Rigidbody2D _rigid;
    private SpriteRenderer _spriteRenderer;
    private Animator _animator;
    
    // 입력
    private InputController _inputController;
    private Vector2 _moveInput;

    
    // 부스트 관련
    private bool _isBoosting;
    private float _boostCoolTimer;
    private float _boostTimer;
    
    //방향 전환 관련
    private bool _isRightForward = true;
    private bool _pendingFlip = false;        // Turn 끝날 때 적용할지 여부
    private bool _hasFlippedThisTurn = false;
    
    // 상수
    private const float FlipThreshold = 0.05f;
    
    
    
    private void Awake()
    {
        Init();
    }

    private void Update()
    {
        GetInput();
        HandleBoostState();
        HandleFacing();
        UpdateAnimator();
    }

   


    private void FixedUpdate()
    {
        ApplyMovement();
    }


    private void LateUpdate()
    {
        UpdateTurnFlip(); // 방향전환 애니메이션과 함께 sprite flip
    }
    private void UpdateTurnFlip()
    {
        if (!_pendingFlip) return;
        
        AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);

        // 현재 애니메이션 상태가 아직 Turn 상태가 아니거나 이미 flip한 경우 
        if (stateInfo.fullPathHash != TurnStateHash || _hasFlippedThisTurn)  return;
        
        // normalizedTime [0, 1] -> 애니 시작 시 0 
        if (stateInfo.normalizedTime >= _turnFlipTime)
        {
            _spriteRenderer.flipX = !_isRightForward;
            _hasFlippedThisTurn = true;
            _pendingFlip = false;
        }
    }

    private void Init()
    {
        _rigid = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
        
        _inputController = GetComponent<InputController>();
    }

    private void GetInput()
    {
        float x = _inputController.XMove;
        float y = _inputController.YMove;  

        _moveInput = new Vector2(x, y).normalized;
        
    }

    private void HandleBoostState()
    {
        bool isBoostHeld      = _inputController._isBoostKeyHeld;
        bool isBoostPressed   =  _inputController._isBoostKeyPressed; 

        
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

        
        float applySpeed = _isBoosting ? (_maxSpeed * _boostMultiplier) : _maxSpeed;
        Vector2 targetVel = _moveInput * applySpeed;

        // 지수 감쇠 Lerp
        float t = 1f - Mathf.Exp(-_responsiveness * Time.fixedDeltaTime);

        currentVel = Vector2.Lerp(currentVel, targetVel, t);

        // 부력 적용
        currentVel.y += _buoyancy * Time.fixedDeltaTime;
        currentVel.y = Mathf.Clamp(currentVel.y, -_maxVerticalSpeed, _maxVerticalSpeed);
        
        _rigid.linearVelocity = currentVel;
        
        _animator.SetFloat("Speed", _rigid.linearVelocity.magnitude);
    }

    private void HandleFacing()
    {
        // X축 입력이 거의 없으면 방향 유지
        if (Mathf.Abs(_moveInput.x) < _turnInputThreshold) return;
           

        bool isRightInput = _moveInput.x > 0f;

        // 이미 그 방향을 보고 있으면 turn 하지 않음
        if (isRightInput == _isRightForward)  return;
           

        // 방향 전환 감지
        _isRightForward = isRightInput;
        _pendingFlip = true;
        _hasFlippedThisTurn = false;

        _animator.SetTrigger("Turn");
    }
    
    private void UpdateAnimator()
    {
        float targetSpeed = _moveInput.magnitude;  

        // 부드럽게 보간
        float current = _animator.GetFloat("Speed");
        float smoothed = Mathf.Lerp(current, targetSpeed, _animSpeedLerp * Time.deltaTime);

        _animator.SetFloat("Speed", smoothed);
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
