using System;
using UnityEngine;

/// <summary>
/// 플레이어 비주얼 처리 컨트롤러
/// </summary>
[RequireComponent(typeof(SpriteRenderer), typeof(Animator))]
public class DiverVisualController : MonoBehaviour
{
    [Header("적용 대상")]
    [SerializeField] private Transform _visualTransform;
    
    [Header("기울기(틸트)")]
    [SerializeField] private float _verticalTiltAngle = 60f;   // 수직이동 최대 회전각
    [SerializeField] private float _diagonalTiltAngle   = 30f;     // 대각선 이동 최대 회전각
    [SerializeField] private float _tiltLerpSpeed = 10f;     // 기울기 보간 속도
    
    [Header("애니메이터 설정")]
    [SerializeField] private float _animSpeedLerp = 10f; // Locomotion 보간
    [SerializeField] private float _turnInputThreshold = 0.2f; //방향 전환 애니메이션 시 최소 입력값
    [SerializeField] private float _animMovingThreshold = 0.15f; // Idle, Swim 구분 기준값
    [SerializeField] private float _turnFlipTime = 0.5f; // Turn 애니의 몇 % 지점에서 flip할지 (0~1)
    
    // 상수
    private const float HorizontalInputDeadZone = 0.01f;
    private const float VerticalInputDeadZone = 0.01f;
    
    private static readonly int SwimTurnHash = Animator.StringToHash("SwimTurn");
    
    // 프로퍼티
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
    private HarpoonShooter _harpoonShooter;
    private Camera _mainCam;
    
   
    
    private Vector2 _moveInput;
    private void Awake()
    {
       Init();
    }

    private void Update()
    {
        _moveInput = _moveController.MoveInput;
        
        HandleFacing();
        UpdateAnimator();
    }

    private void LateUpdate()
    {
        AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
        
        UpdateTilt(stateInfo);
    }

    private void Init()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
        
        _moveController = GetComponentInParent<DiverMoveController>();
        _harpoonShooter = GetComponentInParent<HarpoonShooter>();
        
        _mainCam = Camera.main;
    }

    
    private void HandleFacing()
    {
        bool desiredRight = _isRightForward;
        
        if (_harpoonShooter.IsAiming)
        {
            Vector3 mouseWorld = _mainCam.ScreenToWorldPoint(Input.mousePosition);
            mouseWorld.z = 0f;

            desiredRight = mouseWorld.x >= _moveController.transform.position.x;
        }
        else
        {
           
            if (Mathf.Abs(_moveInput.x) >= _turnInputThreshold)
            {
                desiredRight = _moveInput.x > 0f;
            }
            
        }

        ApplyFacing(desiredRight);
    }
    private void ApplyFacing(bool isRight)
    {
        if (isRight == _isRightForward)
            return;

        bool wasMoving = IsAnimationMoving;

        _isRightForward = isRight;

        // 턴 애니는 "방향이 바뀐 프레임에서만" 트리거
        if (wasMoving)
        {
            _animator.SetTrigger(SwimTurnHash);
        }
        
        _spriteRenderer.flipX = isRight ? false : true; 
    }
   
    
    private void UpdateAnimator()
    {
        float targetSpeed = Mathf.Clamp01(_moveInput.magnitude);  

        // 부드럽게 보간
        float current = _animator.GetFloat("Speed");
        float smoothed = Mathf.Lerp(current, targetSpeed, _animSpeedLerp * Time.deltaTime);

        _animator.SetFloat("Speed", smoothed);
    }
    
   
    
    private void UpdateTilt(AnimatorStateInfo stateInfo)
    {
        
        // 1) 조준 중에는 0도
        if (_harpoonShooter.IsAiming)
        {
            _visualTransform.localRotation = Quaternion.identity;
            return;
        }
        
        // Turn 애니 중에는 회전 고정 (턴 모션이랑 충돌 방지)
        if (stateInfo.IsTag("Turn"))
        {
            SetVisualTilt(0f);
            return;
        }

        float horizontalMove = _moveInput.x;
        float verticalMove = _moveInput.y;
        
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
        
        //  화면 기준으로 위/아래가 항상 일관되게 보이도록
        float baseAngle = tiltDir * facingSign * maxTilt;
        
        // 입력 강도에 따라 조금씩만 차이나게
        float magnitude = Mathf.Clamp01(Mathf.Abs(verticalMove));
        float targetAngle = baseAngle * magnitude;

        SetVisualTilt(targetAngle);
    }

    private void SetVisualTilt(float targetAngle)
    {
        Quaternion targetRotation = Quaternion.Euler(0f, 0f, targetAngle);
        _visualTransform.localRotation = Quaternion.Slerp(_visualTransform.localRotation, targetRotation, _tiltLerpSpeed * Time.deltaTime);
    }
    
}
