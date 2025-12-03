using UnityEngine;

/// <summary>
/// Ship 플레이어의 스프라이트/애니메이션을 담당하는 컨트롤러
/// 8방향 이동 애니메이션 지원 (상하좌우 + 대각선)
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
public class ShipPlayerVisualController : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Animator _animator;

    [Header("Direction Settings")]
    [SerializeField] private bool _flipSpriteOnLeft = true;  // 왼쪽 이동 시 스프라이트 뒤집기

    // Animator Parameters
    private static readonly int IsMoving = Animator.StringToHash("IsMoving");

    // State
    private bool _facingRight = true;

    void Awake()
    {
        // Components 자동 할당
        if (_spriteRenderer == null)
            _spriteRenderer = GetComponent<SpriteRenderer>();

        if (_animator == null)
            _animator = GetComponent<Animator>();
    }

    /// <summary>
    /// 이동 입력에 따라 애니메이션 업데이트
    /// ShipPlayerController에서 호출
    /// </summary>
    public void UpdateMovement(Vector2 moveInput, bool isMoving)
    {
        // 애니메이터가 없으면 종료
        if (_animator == null) return;

        // 이동 중일 때만 방향 업데이트
        if (isMoving && moveInput.sqrMagnitude > 0.01f)
        {
            // 스프라이트 뒤집기 (좌우 이동 시)
            if (_flipSpriteOnLeft)
            {
                HandleSpriteFlip(moveInput.x);
            }
        }

        // Animator Parameters 업데이트
        _animator.SetBool(IsMoving, isMoving);
    }

    /// <summary>
    /// 스프라이트 좌우 뒤집기
    /// </summary>
    private void HandleSpriteFlip(float moveX)
    {
        if (moveX > 0.01f && !_facingRight)
        {
            // 오른쪽으로 전환
            _facingRight = true;
            _spriteRenderer.flipX = false;
        }
        else if (moveX < -0.01f && _facingRight)
        {
            // 왼쪽으로 전환
            _facingRight = false;
            _spriteRenderer.flipX = true;
        }
    }
}
