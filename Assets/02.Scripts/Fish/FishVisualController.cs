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

    [Header("기울기")]
    [SerializeField] private float _maxTiltAngle = 25f;
    [SerializeField] private float _tiltLerpSpeed = 10f;

    [Header("애니메이터")]
    [SerializeField] private float _animSpeedLerp = 10f;

    
    [SerializeField] private float _flipDirThreshold = 0.25f;   // 이 정도는 넘어야 방향 인정
    [SerializeField] private float _minSpeedForFlip = 0.2f;     // 거의 안 움직이면 flip 안 함
    
    private SpriteRenderer _sprite;
    private Animator _anim;

    private bool _isRightForward = true;

    private void Awake()
    {
        Init();
    }

    private void LateUpdate()
    {
        Vector2 vel = _moveController.CurrentVelocity;

        //TransitAnimation(vel);
        HandleFlipX(vel);
        UpdateTilt(vel);
    }

    private void Init()
    {
        _sprite = GetComponent<SpriteRenderer>();
        _anim = GetComponent<Animator>();
    }

    private void TransitAnimation(Vector2 currentVelocity)
    {
        // 1) Speed 파라미터
        float targetSpeed = Mathf.Clamp01(currentVelocity.magnitude);
        float current = _anim.GetFloat("Speed");
        float smoothed = Mathf.Lerp(current, targetSpeed, _animSpeedLerp * Time.deltaTime);
        _anim.SetFloat("Speed", smoothed);
    }
    private void HandleFlipX(Vector2 currentVelocity)
    {
        if (currentVelocity.magnitude < _minSpeedForFlip) return;
        
        Vector2 desired = _moveController.DesiredDir;  
        if (desired.sqrMagnitude < 0.0001f) return;
        
        float x = desired.x;
        if (Mathf.Abs(x) < _flipDirThreshold) return; // 너무 정면(거의 수직)일 땐 방향 바꾸지 않음
            

        bool right = x > 0f;
        if (right == _isRightForward) return;
        
        _isRightForward = right;
        _sprite.flipX = !_isRightForward;
    }

    private void UpdateTilt(Vector2 currentVelocity)
    {
        // 3) 기울기 (이동 방향 각도로 회전)
        float targetAngle = 0f;
        if (currentVelocity.sqrMagnitude > 0.001f)
        {
            
            // 화면 상에서의 이동 방향
            float angle = Mathf.Atan2(currentVelocity.y, currentVelocity.x) * Mathf.Rad2Deg;

            // flipX 보정해서, 항상 "머리 방향" 기준으로 기울도록
            if (!_isRightForward)
                angle = 180f - angle;
            
            targetAngle = Mathf.Clamp(angle, -_maxTiltAngle, _maxTiltAngle);
        }

        SetVisualTilt(targetAngle);
    }
    
    private void SetVisualTilt(float targetAngle)
    {
        Quaternion targetRot = Quaternion.Euler(0f, 0f, targetAngle);
        _visualTransform.localRotation = Quaternion.Slerp(_visualTransform.localRotation, targetRot, _tiltLerpSpeed * Time.deltaTime);
    }
}