using UnityEngine;
using System.Collections;

/// <summary>
/// Ship 씬에서 선원 NPC의 시각적 표현 및 순찰 행동
/// CrewMember 데이터와 연동되어 사망 시 자동으로 사라짐
/// </summary>
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
public class ShipCrewNPC : MonoBehaviour
{
    [Header("Crew Data")]
    [SerializeField] private int _crewID;  // 연동할 CrewMember의 ID

    [Header("Patrol Settings")]
    [SerializeField] private Transform[] _patrolPoints;  // 순찰 포인트 배열
    [SerializeField] private float _moveSpeed = 2f;
    [SerializeField] private float _waitTimeAtPoint = 2f;  // 각 포인트에서 대기 시간
    [SerializeField] private bool _loopPatrol = true;  // 순찰 루프 여부

    [Header("Visual Settings")]
    [SerializeField] private bool _flipSpriteOnLeft = true;

    // Components
    private SpriteRenderer _spriteRenderer;
    private Animator _animator;

    // State
    private CrewMember _crewData;
    private int _currentPatrolIndex = 0;
    private bool _isPatrolling = false;
    private bool _facingRight = true;
    private Coroutine _patrolCoroutine;

    // Animator Parameters
    private static readonly int IsMoving = Animator.StringToHash("IsMoving");

    void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
    }

    void Start()
    {
        // CrewManager에서 CrewMember 데이터 가져오기
        if (CrewManager.Instance != null)
        {
            _crewData = CrewManager.Instance.GetCrewByID(_crewID);

            if (_crewData != null)
            {
                // 사망 이벤트 구독
                CrewManager.Instance.OnCrewDied += OnCrewDied;

                // 순찰 시작
                if (_patrolPoints != null && _patrolPoints.Length > 0)
                {
                    StartPatrol();
                }
            }
            else
            {
                Debug.LogError($"[ShipCrewNPC] CrewID {_crewID}에 해당하는 선원을 찾을 수 없습니다!");
            }
        }
        else
        {
            Debug.LogError("[ShipCrewNPC] CrewManager가 없습니다!");
        }
    }

    void OnDestroy()
    {
        // 이벤트 구독 해제
        if (CrewManager.Instance != null)
        {
            CrewManager.Instance.OnCrewDied -= OnCrewDied;
        }
    }

    /// <summary>
    /// 순찰 시작
    /// </summary>
    public void StartPatrol()
    {
        if (_isPatrolling) return;

        _isPatrolling = true;
        _patrolCoroutine = StartCoroutine(PatrolRoutine());
    }

    /// <summary>
    /// 순찰 중지
    /// </summary>
    public void StopPatrol()
    {
        if (_patrolCoroutine != null)
        {
            StopCoroutine(_patrolCoroutine);
            _patrolCoroutine = null;
        }

        _isPatrolling = false;

        // Idle 애니메이션
        if (_animator != null)
        {
            _animator.SetBool(IsMoving, false);
        }
    }

    /// <summary>
    /// 순찰 루틴
    /// </summary>
    private IEnumerator PatrolRoutine()
    {
        while (_isPatrolling)
        {
            // 현재 목표 지점
            Transform targetPoint = _patrolPoints[_currentPatrolIndex];

            // 목표 지점으로 이동
            yield return StartCoroutine(MoveToPoint(targetPoint.position));

            // 대기
            yield return new WaitForSeconds(_waitTimeAtPoint);

            // 다음 포인트로 이동
            _currentPatrolIndex++;

            // 루프 처리
            if (_currentPatrolIndex >= _patrolPoints.Length)
            {
                if (_loopPatrol)
                {
                    _currentPatrolIndex = 0;
                }
                else
                {
                    // 순찰 종료
                    StopPatrol();
                    yield break;
                }
            }
        }
    }

    /// <summary>
    /// 특정 지점으로 이동
    /// </summary>
    private IEnumerator MoveToPoint(Vector3 targetPosition)
    {
        // 이동 애니메이션 시작
        if (_animator != null)
        {
            _animator.SetBool(IsMoving, true);
        }

        while (Vector3.Distance(transform.position, targetPosition) > 0.1f)
        {
            // 이동 방향 계산
            Vector3 direction = (targetPosition - transform.position).normalized;

            // 스프라이트 뒤집기
            if (_flipSpriteOnLeft)
            {
                HandleSpriteFlip(direction.x);
            }

            // 이동
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, _moveSpeed * Time.deltaTime);

            yield return null;
        }

        // 정확한 위치로 이동
        transform.position = targetPosition;

        // Idle 애니메이션
        if (_animator != null)
        {
            _animator.SetBool(IsMoving, false);
        }
    }

    /// <summary>
    /// 스프라이트 좌우 뒤집기
    /// </summary>
    private void HandleSpriteFlip(float moveX)
    {
        if (moveX > 0.01f && !_facingRight)
        {
            _facingRight = true;
            _spriteRenderer.flipX = false;
        }
        else if (moveX < -0.01f && _facingRight)
        {
            _facingRight = false;
            _spriteRenderer.flipX = true;
        }
    }

    /// <summary>
    /// 선원 사망 이벤트 핸들러
    /// </summary>
    private void OnCrewDied(CrewMember deadCrew)
    {
        if (deadCrew.CrewID == _crewID)
        {
            Debug.Log($"[ShipCrewNPC] 선원 {deadCrew.CrewName} (ID: {_crewID}) 사망 - NPC 제거");

            // 순찰 중지
            StopPatrol();

            // 오브젝트 제거 (페이드 아웃 효과 추가 가능)
            Destroy(gameObject);
        }
    }

    // ========== Public API ==========

    /// <summary>
    /// CrewID 설정 (외부에서 동적 생성 시)
    /// </summary>
    public void SetCrewID(int crewID)
    {
        _crewID = crewID;
    }

    /// <summary>
    /// 순찰 포인트 설정 (외부에서 동적 생성 시)
    /// </summary>
    public void SetPatrolPoints(Transform[] points)
    {
        _patrolPoints = points;
    }

    /// <summary>
    /// 이동 속도 설정
    /// </summary>
    public void SetMoveSpeed(float speed)
    {
        _moveSpeed = Mathf.Max(0f, speed);
    }

    // ========== 디버그용 ==========

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (_patrolPoints == null || _patrolPoints.Length == 0) return;

        // 순찰 경로 표시
        Gizmos.color = Color.cyan;

        for (int i = 0; i < _patrolPoints.Length; i++)
        {
            if (_patrolPoints[i] == null) continue;

            // 포인트 표시
            Gizmos.DrawWireSphere(_patrolPoints[i].position, 0.3f);

            // 다음 포인트로의 연결선
            if (i < _patrolPoints.Length - 1 && _patrolPoints[i + 1] != null)
            {
                Gizmos.DrawLine(_patrolPoints[i].position, _patrolPoints[i + 1].position);
            }
            else if (_loopPatrol && i == _patrolPoints.Length - 1 && _patrolPoints[0] != null)
            {
                // 루프인 경우 마지막에서 첫 포인트로 연결
                Gizmos.DrawLine(_patrolPoints[i].position, _patrolPoints[0].position);
            }
        }
    }
#endif
}
