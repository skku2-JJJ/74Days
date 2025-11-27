using UnityEngine;

/// <summary>
/// 물고기 물리 이동 컨트롤러
/// </summary>

[RequireComponent(typeof(Rigidbody2D))]
public class FishMoveController : MonoBehaviour
{
    [Header("물리 이동 요소")]
    [SerializeField] private float _maxSpeed = 2.5f;
    [SerializeField, Range(8,13)] private float _responsiveness = 10f;
    [SerializeField] private float _buoyancy = 0.2f;
    [SerializeField] private float _maxVerticalSpeed = 3f;

    private float _burstTimer;
    private Vector2 _burstVelocity;
    
    private Rigidbody2D _rigid;
    
    public Vector2 DesiredDir { get; set; } // 이동 방향
    
    public bool IsMovementLocked { get; set; } // QTE, Capture 등의 상황에서 이동 막기

    public Vector2 CurrentVelocity => _rigid.linearVelocity;

   
    // Escape/QTE 등에서 잠깐 속도를 덮어쓰는 용도
    private float _overrideSpeed = -1f;
    private float _overrideSpeedTimer = 0f;
    
    private void Awake()
    {
        Init();
    }

    private void FixedUpdate()
    {
        if (IsMovementLocked)
        {
            _rigid.linearVelocity = Vector2.zero;
            return;
        }
        
        // Escape 등에서 설정해 둔 속도 오버라이드 타이머 갱신
        if (_overrideSpeedTimer > 0f)
        {
            _overrideSpeedTimer -= Time.fixedDeltaTime;
            if (_overrideSpeedTimer <= 0f)
            {
                _overrideSpeed = -1f;
            }
        }
        
        Move();
    }

    private void Init()
    {
        _rigid = GetComponent<Rigidbody2D>();
    }
    
    /// <summary>
    /// 일정 시간 동안 기본 _maxSpeed 대신 이 속도로 이동
    /// </summary>
    public void SetOverrideSpeed(float speed, float duration)
    {
        _overrideSpeed = speed;
        _overrideSpeedTimer = duration;
    }
    

    private void Move()
    {
        Vector2 currentVel = _rigid.linearVelocity;

        Vector2 dir = DesiredDir;
        if (dir.sqrMagnitude > 1f)
            dir.Normalize();

        // Escape 상태일 때는 오버라이드 속도 사용
        float speed = (_overrideSpeedTimer > 0f && _overrideSpeed > 0f)
            ? _overrideSpeed
            : _maxSpeed;
        
        Vector2 targetVel = dir * speed;

        float t = 1f - Mathf.Exp(-_responsiveness * Time.fixedDeltaTime);
        currentVel = Vector2.Lerp(currentVel, targetVel, t);

        // 부력
        currentVel.y += _buoyancy * Time.fixedDeltaTime;
        currentVel.y = Mathf.Clamp(currentVel.y, -_maxVerticalSpeed, _maxVerticalSpeed);

        _rigid.linearVelocity = currentVel;
    }
    
   
    
   
}