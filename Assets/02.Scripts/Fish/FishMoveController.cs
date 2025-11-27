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

    private Rigidbody2D _rigid;
    
    public Vector2 DesiredDir { get; set; } // 이동 방향
    
    public bool IsMovementLocked { get; set; } // QTE, Capture 등의 상황에서 이동 막기

    public Vector2 CurrentVelocity => _rigid.linearVelocity;

    private bool _isVelocityInit = false; // lock 된 첫 순간만 velocity 초기화
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
        
        Move();
    }

    private void Init()
    {
        _rigid = GetComponent<Rigidbody2D>();
    }

    private void Move()
    {
        Vector2 currentVel = _rigid.linearVelocity;

        Vector2 dir = DesiredDir;
        if (dir.sqrMagnitude > 1f)
            dir.Normalize();

        Vector2 targetVel = dir * _maxSpeed;

        float t = 1f - Mathf.Exp(-_responsiveness * Time.fixedDeltaTime);
        currentVel = Vector2.Lerp(currentVel, targetVel, t);

        // 부력
        currentVel.y += _buoyancy * Time.fixedDeltaTime;
        currentVel.y = Mathf.Clamp(currentVel.y, -_maxVerticalSpeed, _maxVerticalSpeed);

        _rigid.linearVelocity = currentVel;
    }
}