using UnityEngine;

/// <summary>
/// Ship 씬에서 플레이어 이동을 담당하는 컨트롤러
/// Top-down 2D 이동 (WASD/화살표 키)
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class ShipPlayerController : MonoBehaviour
{
    public static ShipPlayerController Instance { get; private set; }

    [Header("Movement Settings")]
    [SerializeField] private float _moveSpeed = 5f;
    [SerializeField] private float _acceleration = 20f;
    [SerializeField] private float _deceleration = 15f;

    // Components
    private Rigidbody2D _rb;
    private ShipPlayerVisualController _visualController;

    // State
    private Vector2 _moveInput;
    private Vector2 _currentVelocity;
    private bool _canMove = true;

    public bool IsMoving => _moveInput.sqrMagnitude > 0.01f;

    void Awake()
    {
        // 싱글톤 (씬 단위, DontDestroyOnLoad 없음)
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Components
        _rb = GetComponent<Rigidbody2D>();
        _visualController = GetComponent<ShipPlayerVisualController>();

        // Rigidbody2D 설정
        _rb.gravityScale = 0f;
        _rb.linearDamping = 0f;
        _rb.freezeRotation = true;
        _rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
    }

    void Update()
    {
        if (!_canMove) return;

        // 입력 처리
        HandleInput();
    }

    void FixedUpdate()
    {
        if (!_canMove)
        {
            // 이동 불가 시 감속
            _currentVelocity = Vector2.Lerp(_currentVelocity, Vector2.zero, _deceleration * Time.fixedDeltaTime);
            _rb.linearVelocity = _currentVelocity;
            return;
        }

        // 물리 기반 이동
        HandleMovement();
    }

    /// <summary>
    /// 키보드 입력 처리
    /// </summary>
    private void HandleInput()
    {
        // WASD / 화살표 키
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        _moveInput = new Vector2(horizontal, vertical).normalized;
    }

    /// <summary>
    /// 물리 기반 이동 처리
    /// </summary>
    private void HandleMovement()
    {
        if (_moveInput.sqrMagnitude > 0.01f)
        {
            // 가속
            Vector2 targetVelocity = _moveInput * _moveSpeed;
            _currentVelocity = Vector2.Lerp(_currentVelocity, targetVelocity, _acceleration * Time.fixedDeltaTime);
        }
        else
        {
            // 감속
            _currentVelocity = Vector2.Lerp(_currentVelocity, Vector2.zero, _deceleration * Time.fixedDeltaTime);
        }

        // 속도 적용
        _rb.linearVelocity = _currentVelocity;

        // 애니메이션 업데이트
        if (_visualController != null)
        {
            _visualController.UpdateMovement(_moveInput, IsMoving);
        }
    }
}
