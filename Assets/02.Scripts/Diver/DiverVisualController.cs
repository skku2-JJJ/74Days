using System;
using UnityEngine;

public class DiverVisualController : MonoBehaviour
{
    [Header("적용 대상")]
    [SerializeField] private Transform _visualTransform;
    
    [Header("기울기(틸트)")]
    [SerializeField] private float _verticalTiltAngle = 25f;   // 수직이동 최대 회전각
    [SerializeField] private float _diagonalTiltAngle   = 15f;     // 대각선 이동 최대 회전각
    [SerializeField] private float _tiltLerpSpeed = 10f;     // 기울기 보간 속도
    
    [Header("애니메이터 설정")]
    [SerializeField] private float _animSpeedLerp = 10f; // Locomotion 보간
    [SerializeField] private float _turnInputThreshold = 0.2f; //방향 전환 애니메이션 시 최소 입력값
    [SerializeField] private float _animMovingThreshold = 0.15f; // Idle, Swim 구분 기준값
    [SerializeField] private float _turnFlipTime = 0.5f; // Turn 애니의 몇 % 지점에서 flip할지 (0~1)
    
    private bool IsAnimationMoving => _animator.GetFloat("Speed") > _animMovingThreshold; //애니 기준으로 이동 판단 
    
    // 컴포넌트
    private SpriteRenderer _spriteRenderer;
    private Animator _animator;
    
    // 방향 전환 관련
    private bool _isRightForward = true;
    private bool _pendingFlip = false;        // Turn 끝날 때 적용할지 여부
    private bool _hasFlippedThisTurn = false;

    // 참조
    private DiverMoveController _moveController;
    
    // 상수
    private const float HorizontalInputDeadZone = 0.01f;
    private const float VerticalInputDeadZone = 0.01f;
    private const float HalfTurnAngle = 180f;
    private const float MaxTurnAngle = 360f;
    
    private void Awake()
    {
       Init();
    }

    private void Update()
    {
        Vector2 moveInput = _moveController.MoveInput;
        
        HandleFacing(moveInput);
        UpdateAnimator(moveInput);
    }

    private void LateUpdate()
    {
        Vector2 moveInput = _moveController.MoveInput;
        
        UpdateTurnFlip(); // 방향전환 애니메이션과 함께 sprite flip
        UpdateTilt(moveInput);
    }

    private void Init()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
        _moveController = GetComponentInParent<DiverMoveController>();
    }

    private void HandleFacing(Vector2 moveInput)
    {
        // X축 입력이 거의 없으면 방향 유지
        if (Mathf.Abs(moveInput.x) < _turnInputThreshold) return;
           

        bool isRightInput = moveInput.x > 0f;

        // 이미 그 방향을 보고 있으면 turn 하지 않음
        if (isRightInput == _isRightForward)  return;
        
        _isRightForward = isRightInput;
        
        if (IsAnimationMoving)
        {
            // 방향 전환 감지
            _pendingFlip = true;
            _hasFlippedThisTurn = false;
            
            _animator.SetTrigger("SwimTurn");
        }
        else
        {
            _spriteRenderer.flipX = !_isRightForward;

            // Turn 플립 로직 안 타도록 OFF 처리
            _pendingFlip = false;
            _hasFlippedThisTurn = true;
        }
    }
    
    private void UpdateAnimator(Vector2 moveInput)
    {
        float targetSpeed = Mathf.Clamp01(moveInput.magnitude);  

        // 부드럽게 보간
        float current = _animator.GetFloat("Speed");
        float smoothed = Mathf.Lerp(current, targetSpeed, _animSpeedLerp * Time.deltaTime);

        _animator.SetFloat("Speed", smoothed);
    }
    
    private void UpdateTurnFlip()
    {
        if (!_pendingFlip) return;
        
        AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);

        // 현재 애니메이션 상태가 "Turn" 태그가 아니거나 이미 flip한 경우
        if (!stateInfo.IsTag("Turn") || _hasFlippedThisTurn)  return;
           
        
        // normalizedTime [0, 1] -> 애니 시작 시 0 
        if (stateInfo.normalizedTime >= _turnFlipTime)
        {
            _spriteRenderer.flipX = !_isRightForward;
            _hasFlippedThisTurn = true;
            _pendingFlip = false;
        }
    }
    
    private void UpdateTilt(Vector2 moveInput)
    {
        // Turn 애니 중에는 회전 고정 (턴 모션이랑 충돌 방지)
        AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsTag("Turn"))
        {
            SetVisualTilt(0f);
            return;
        }

        float horizontalMove = moveInput.x;
        float verticalMove = moveInput.y;
        
        // 수직 입력이 없으면 서서히 0도로 복귀
        if (Mathf.Abs(verticalMove) < VerticalInputDeadZone)
        {
            SetVisualTilt(0f);
            return;
        }
        
        bool hasHorizontal = Mathf.Abs(horizontalMove) >= HorizontalInputDeadZone;

        // 수직이동 / 대각선 이동 구분하여 회전 최대각 선택 
        float maxTilt = hasHorizontal ? _diagonalTiltAngle : _verticalTiltAngle;
        
        
        float tiltDir = Mathf.Sign(verticalMove);   // 위,아래 방향 (+1 / -1)
        float facingSign = _isRightForward ? 1f : -1f; // 좌,우 방향 (+1 / -1)
        
        // 👉 화면 기준으로 "위/아래"가 항상 일관되게 보이도록
        //    facingSign을 곱해줌
        float baseAngle = tiltDir * facingSign * maxTilt;
        
        // 입력 강도에 따라 조금씩만 차이나게
        float magnitude = Mathf.Clamp01(Mathf.Abs(verticalMove));
        float targetAngle = baseAngle * magnitude;

        SetVisualTilt(targetAngle);
    }

    private void SetVisualTilt(float targetAngle)
    {
        float currentZ = _visualTransform.localEulerAngles.z;
        if (currentZ > HalfTurnAngle) currentZ -= MaxTurnAngle; //[-180f, 180f] 사이 유지

        float newZ = Mathf.Lerp(currentZ, targetAngle, _tiltLerpSpeed * Time.deltaTime);
        _visualTransform.localRotation = Quaternion.Euler(0f, 0f, newZ);
    }
    
}
