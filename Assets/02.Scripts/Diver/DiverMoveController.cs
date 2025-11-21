using System;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 수중 이동 컨트롤러
/// </summary>
[RequireComponent(typeof(Rigidbody2D), typeof(InputController))]
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
    
    // 상수
    private const float MinInputMagnitude = 0.01f;
    
    // 프로퍼티
    public Vector2 MoveInput => _moveInput;
    private bool IsMoving => _moveInput.sqrMagnitude > MinInputMagnitude; //입력 기준으로 이동 판단
   
    
    // 컴포넌트
    private Rigidbody2D _rigid;
    
    // 입력
    private InputController _inputController;
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
        
    }

    private void GetMoveInput()
    {
        float x = _inputController.XMove;
        float y = _inputController.YMove;  

        _moveInput = new Vector2(x, y);
        
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
        
        _rigid.linearVelocity = currentVel;
        
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
