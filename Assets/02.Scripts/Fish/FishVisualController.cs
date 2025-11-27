using UnityEngine;

/// <summary>
/// 물고기 비주얼 처리 컨트롤러
/// </summary>
[RequireComponent(typeof(SpriteRenderer), typeof(Animator))]
public class FishVisualController : MonoBehaviour
{
    [Header("참조")]
    [SerializeField] private Transform _visualTransform;
    [SerializeField] private FishMoveController _moveController;

    [Header("기울기(틸트)")]
    [SerializeField] private float _verticalTiltAngle = 25f;  
    [SerializeField] private float _diagonalTiltAngle = 15f;  
    [SerializeField] private float _tiltLerpSpeed = 10f;
    

    [Header("애니메이터")]
    [SerializeField] private float _animSpeedLerp = 10f;

    [Header("플립 조건")]
    [SerializeField] private bool _textureFacesRight = false; // 에셋 기본 방향
    [SerializeField] private float _flipDirThreshold = 0.25f;   
    [SerializeField] private float _minSpeedForFlip = 0.2f;    
    
    
    private const float HorizontalInputDeadZone = 0.01f;
    private const float VerticalInputDeadZone   = 0.01f;
    
    private SpriteRenderer _sprite;
    private Animator _anim;

    private bool _isRightForward;
    private bool _isFacingLocked = false;

    private void Awake()
    {
        Init();
    }

    private void LateUpdate()
    {
        Vector2 vel = _moveController.CurrentVelocity;
        Vector2 desired  = _moveController.DesiredDir;
        
        TransitAnimation(vel);
        HandleFlipX(desired, vel);
        UpdateTilt(desired);
    }

    private void Init()
    {
        _sprite = GetComponent<SpriteRenderer>();
        _anim = GetComponent<Animator>();
        
        _isRightForward = _textureFacesRight? _sprite.flipX : !_sprite.flipX;
        ApplyFacing();
    }

    private void TransitAnimation(Vector2 currentVelocity)
    {
        // 1) Speed 파라미터
        float targetSpeed = Mathf.Clamp01(currentVelocity.magnitude);
        float current = _anim.GetFloat("Speed");
        float smoothed = Mathf.Lerp(current, targetSpeed, _animSpeedLerp * Time.deltaTime);
        _anim.SetFloat("Speed", smoothed);
    }
    private void HandleFlipX(Vector2 desiredDir, Vector2 currentVelocity)
    {
        if (_isFacingLocked) return;
        
        if (currentVelocity.magnitude < _minSpeedForFlip) return;
        if (desiredDir.sqrMagnitude < 0.0001f) return;
        
        float x = desiredDir.x;
        
        if (Mathf.Abs(x) < _flipDirThreshold) return; // 거의 수직이면 방향 유지
            

        bool right = x > 0f;
        if (right == _isRightForward) return;
        
        _isRightForward = right;
        ApplyFacing(); 
    }

    private void UpdateTilt(Vector2 desiredDir)
    {
        float horizontalMove = desiredDir.x;
        float verticalMove   = desiredDir.y;
        
        if (Mathf.Abs(verticalMove) < VerticalInputDeadZone && Mathf.Abs(horizontalMove) < HorizontalInputDeadZone)
        {
            SetVisualTilt(0f);
            return;
        }
        
        // 수직 입력 없으면 서서히 0도로 복귀
        if (Mathf.Abs(verticalMove) < VerticalInputDeadZone)
        {
            SetVisualTilt(0f);
            return;
        }

        bool hasHorizontal = Mathf.Abs(horizontalMove) >= HorizontalInputDeadZone;
        float maxTilt = hasHorizontal ? _diagonalTiltAngle : _verticalTiltAngle;

        float tiltDir    = Mathf.Sign(verticalMove);            // 위(+1) / 아래(-1)
        float facingSign = _isRightForward ? 1f : -1f;          // 오른쪽(+1) / 왼쪽(-1)
        
        // facingSign을 곱해줘서 "화면 기준"으로 위/아래 방향이 항상 일관되게 보이게 함
        float baseAngle = tiltDir * facingSign * maxTilt;

        // 입력 강도(vert 크기)에 따라 각도 비율 조정
        float magnitude   = Mathf.Clamp01(Mathf.Abs(verticalMove));
        float targetAngle = baseAngle * magnitude;

        SetVisualTilt(targetAngle);
    }
    
    private void SetVisualTilt(float targetAngle)
    {
        Quaternion targetRot = Quaternion.Euler(0f, 0f, targetAngle);
        _visualTransform.localRotation = Quaternion.Slerp(_visualTransform.localRotation, targetRot, _tiltLerpSpeed * Time.deltaTime);
    }
    
    
    // 🔥 QTE 등에서 방향을 강제로 고정할 때 호출
    public void ForceLookAwayFrom(Vector2 diverWorldPos, bool lockFacing)
    {
        bool shouldFaceRight = diverWorldPos.x < transform.position.x;

        _isRightForward = shouldFaceRight;
        ApplyFacing();

        _isFacingLocked = lockFacing;
    }
    public void SetFacingLock(bool locked)
    {
        _isFacingLocked = locked;
    }

    private void ApplyFacing()
    {
        _sprite.flipX = _textureFacesRight ? !_isRightForward : _isRightForward;
    }
}