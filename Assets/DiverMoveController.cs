using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 수중 이동 컨트롤러
/// </summary>
public class DiverMoveController : MonoBehaviour
{
    [Header("움직임 관련 변수")]
    [SerializeField] private float _maxSpeed = 5f;          
    [SerializeField] private float _acceleration = 12f;     
    [SerializeField] private float _waterDrag = 8f;        

    [Header("부력 설정")]
    [SerializeField] private float _buoyancy = 0.4f;        // 항상 위로 살살 밀어올리기
    [SerializeField] private float _maxVerticalSpeed = 4f;  // 수직이동 속도 제한

    [Header("부스트")]
    [SerializeField] private float _boostMultiplier = 1.8f; 
    [SerializeField] private float _boostDuration = 0.35f;  
    [SerializeField] private float _boostCoolTime = 1.0f;   
    
    // 프로퍼티
    public Vector2 Velocity => _rigid.linearVelocity;
    public bool IsBoosting => _isBoosting;
    public bool IsMoving => _moveInput.sqrMagnitude > 0.01f;

    // 컴포너트
    private Rigidbody2D _rigid;
    private SpriteRenderer _spriteRenderer; 
    
    // 입력
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
        GetInput();
        HandleBoostState();
        UpdateSpriteFlip();
    }

    private void FixedUpdate()
    {
        ApplyMovement();
    }

    private void Init()
    {
        _rigid = GetComponent<Rigidbody2D>();
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        
    }

    private void GetInput()
    {
        float x = Input.GetAxisRaw("Horizontal"); 
        float y = Input.GetAxisRaw("Vertical");  

        _moveInput = new Vector2(x, y).normalized;
        
    }

    private void HandleBoostState()
    {
        _boostCoolTimer += Time.deltaTime;
        if (_boostCoolTimer < _boostCoolTime) return;
        
        if (Input.GetKey(KeyCode.LeftShift) && _moveInput.sqrMagnitude > 0.01f) // 정지 중에는 부스트 불가
        {
            _isBoosting = true;
            _boostTimer += Time.deltaTime;
        }
        else
        {
            _isBoosting = false;
            _boostTimer = 0f;
            _boostCoolTimer = 0f;
        }
        

        // 부스트 종료
        if (_isBoosting && _boostTimer >= _boostDuration)
        {
            _isBoosting = false;
            _boostTimer = 0f;
            _boostCoolTimer = 0f;
        }
    }

    private void ApplyMovement()
    {
        Vector2 currentVel = _rigid.linearVelocity;

        
        float applySpeed = _isBoosting ? (_maxSpeed * _boostMultiplier) : _maxSpeed;
        Vector2 desiredVel = _moveInput * applySpeed;

       // 관성에 의한 가속 이동
        currentVel = Vector2.MoveTowards(
            currentVel,
            desiredVel,
            _acceleration * Time.fixedDeltaTime
        );

        // 입력 거의 없으면 물 저항으로 감속
        if (_moveInput.sqrMagnitude < 0.01f)
        {
            currentVel = Vector2.MoveTowards(
                currentVel,
                Vector2.zero,
                _waterDrag * Time.fixedDeltaTime
            );
        }

        // 부력 적용
        currentVel.y += _buoyancy * Time.fixedDeltaTime;
        currentVel.y = Mathf.Clamp(currentVel.y, -_maxVerticalSpeed, _maxVerticalSpeed);
        
        _rigid.linearVelocity = currentVel;
    }

    private void UpdateSpriteFlip()
    {
        // 이동 입력 기준으로 좌우 반전
        if (_moveInput.x > 0.05f)
            _spriteRenderer.flipX = false;
        else if (_moveInput.x < -0.05f)
            _spriteRenderer.flipX = true;
    }
}
