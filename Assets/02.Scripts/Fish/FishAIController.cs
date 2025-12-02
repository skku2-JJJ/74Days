using UnityEngine;

[RequireComponent(typeof(FishMoveController))]
public class FishAIController : MonoBehaviour
{
    private enum EFishState
    {
        Idle,
        Wander,
        Escape,
        Chase,
        Attack
    }

    [Header("State 결정 주기")]
    [SerializeField] private float _minDecideStateDuration = 2.0f;
    [SerializeField] private float _maxDecideStateDuration = 4.0f;
    private EFishState _eFishState = EFishState.Wander;
    private float _stateTimer;
    private float _stateDuration;
    
    [Header("Wander 설정")]
    [SerializeField] private float _minDecideDirDuration = 0.8f;
    [SerializeField] private float _maxDecideDirDuration = 1.4f;
    [SerializeField] private float _wanderChangeInterval = 1.0f;
    [SerializeField] private float _wanderJitter = 70f; 
    private FishMoveController _move;
    private Vector2 _wanderDir = Vector2.right;
    private float _wanderTimer;
    
    [Header("Idle 설정")]
    [SerializeField] private float _idleMinDuration = 1.0f;
    [SerializeField] private float _idleMaxDuration = 3.0f;
    [SerializeField] private float _idleChanceAfterWander = 0.4f;

    [Header("공격형 설정")]
    [SerializeField] private bool _isAggressive = false;   
    [SerializeField] private float _aggroRadius = 4f;      
    [SerializeField] private float _attackRange = 0.8f;    
    [SerializeField] private float _attackCooltime = 1.5f; 
    private float _attackCoolTimer = 0f;
    
    
    
    [Header("장애물 회피")]
    [SerializeField] private LayerMask _obstacleMask;
    [SerializeField] private float _rayDistance = 1.2f;
    private Vector2 _lastPos;
    private float _stuckTimer;
    
    [Header("플레이어 회피")]
    private Transform _diver;
    [SerializeField] private float _fleeRadius = 3f;
    [SerializeField] private float _fleeStrength = 1.5f;
    
    [Header("Escape 설정")]
    [SerializeField] private float _escapeDuration = 3f;
    private Vector2 _escapeDir;
    
    private const float FullAngle = 360f;
    private const float EpsilonNum = 0.001f;
    private const float MaxStuckTime = 1.5f;
    
    private static readonly Vector2[] _escapeSampleDirs = {
        Vector2.up, Vector2.down,
        Vector2.left, Vector2.right,
        (Vector2.up + Vector2.right).normalized,
        (Vector2.up + Vector2.left).normalized,
        (Vector2.down + Vector2.right).normalized,
        (Vector2.down + Vector2.left).normalized,
    };

    private Animator _animator;
    
    private void Awake()
    {
        Init();
        EnterWander();
    }
    

    private void Update()
    {
        if (_move.IsMovementLocked)  return;
           
        _attackCoolTimer += Time.deltaTime;

        if (_isAggressive && _eFishState != EFishState.Escape)
        {
            UpdateAggroState();   // ★ 플레이어와 거리 보고 Chase/Attack 상태 전환
        }
        

        _stateTimer += Time.deltaTime;

        if (_stateTimer >= _stateDuration)
        {
            SwitchState();
        }

        /*// 플레이어 회피
        if (_diver != null && TryGetFleeDir(out Vector2 fleeDir))
        {
            _move.DesiredDir = ApplyObstacleAvoidance(fleeDir);
            return;
        }*/

        DecideMoveDir();
        CheckStuck();
    }

    private void Init()
    {
        _move = GetComponent<FishMoveController>();
        _animator = GetComponentInChildren<Animator>();
        _diver = GameObject.FindGameObjectWithTag("Player").transform;
      
        // 초기 방향 설정
        float angle = Random.Range(0, FullAngle);
        _wanderDir = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
    }
    
    private void SwitchState()
    {
        _stateTimer = 0f;

        switch (_eFishState)
        {
            case EFishState.Wander:
                if (Random.value < _idleChanceAfterWander)
                    EnterIdle();
                else
                    EnterWander();
                break;
            
            case EFishState.Idle:
            case EFishState.Escape:
            default:
                EnterWander();
                break;
            
            // 추격 / 공격 상태는 UpdateAggroState에서 관리
            case EFishState.Chase:
            case EFishState.Attack:
                break;
        }
    }
    
    private void UpdateAggroState()
    {
        if (_diver == null) return;

        float dist = Vector2.Distance(_diver.position, transform.position);
        
        if (dist > _aggroRadius)
        {
            if (_eFishState == EFishState.Chase || _eFishState == EFishState.Attack)
            {
                EnterWander();
            }
            return;
        }

        // 어그로 범위 안
        if (dist <= _attackRange && _attackCoolTimer >= _attackCooltime)
        {
            EnterAttack();
        }
        else
        {
            EnterChase();
        }
    }

    private void DecideMoveDir()
    {
        Vector2 dir = Vector2.zero;

        switch (_eFishState)
        {
            case EFishState.Idle:
                dir = Vector2.zero; 
                break;
            case EFishState.Wander:
                dir = GetWanderDirection();    
                break;
            case EFishState.Escape:
                dir = _escapeDir;       
                break;
            
            case EFishState.Chase:
            case EFishState.Attack:
                if (_diver != null)
                {
                    dir = ((Vector2)_diver.position - (Vector2)transform.position).normalized;
                }
                break;
        }

        dir = ApplyObstacleAvoidance(dir);
        _move.DesiredDir = dir;
    }
    
    

    private void EnterIdle()
    {
        _eFishState = EFishState.Idle;
        _stateDuration = Random.Range(_idleMinDuration, _idleMaxDuration);
        _wanderTimer = 0f; 
    }

    private void EnterWander()
    {
        _eFishState = EFishState.Wander;
        _stateDuration = Random.Range(_minDecideStateDuration, _maxDecideStateDuration);
        _wanderTimer = 0f;
    }
    
    private void EnterChase()
    {
        if (_eFishState == EFishState.Chase) return;

        _eFishState = EFishState.Chase;
        _stateTimer = 0f;
        _stateDuration = 5f; // 추격 상태 유지 시간 설정
    }

    private void EnterAttack()
    {
        _eFishState = EFishState.Attack;
        _stateTimer = 0f;
        _stateDuration = 0.3f; // 공격 모션동안 공격상태 유지
        _attackCoolTimer = 0f;

        TryHitPlayer(); 
    }
    
    
    /// <summary>
    /// QTE 실패 등으로 인해 도망 상태로 전환
    /// </summary>
    public void EnterEscape(Vector2 fleeDir, float escapeSpeed)
    {
        _eFishState = EFishState.Escape;
        _stateTimer = 0f;
        _stateDuration = _escapeDuration;

        if (fleeDir.sqrMagnitude > EpsilonNum)
        {
            _escapeDir = fleeDir.normalized;
        }
        else
        {
            _escapeDir = _wanderDir; 
        }
      
           

        if (_move != null)
        {
          // Escape 속도 적용
            _move.SetOverrideSpeed(escapeSpeed, _escapeDuration);
        }
    }
    
    private void TryHitPlayer()
    {
        if (_diver == null) return;

        float dist = Vector2.Distance(_diver.position, transform.position);
        if (dist > _attackRange * 1.2f) return; // 약간 여유
        
        DiverStatus diverStatus = _diver.GetComponent<DiverStatus>();
        if (diverStatus != null)
        {
            diverStatus.TakeDamage(1);  
            Debug.Log("Attack!");
        }

        // TODO : 공격 애니메이션 
        _animator.SetTrigger("Attack");
    }
    
    private Vector2 GetWanderDirection()
    {
        _wanderTimer += Time.deltaTime;
        if (_wanderTimer >= _wanderChangeInterval)
        {
            _wanderTimer = 0f;
            _wanderChangeInterval = Random.Range(_minDecideDirDuration, _maxDecideDirDuration);

            float jitter = Random.Range(-_wanderJitter, _wanderJitter);
            _wanderDir = (Quaternion.Euler(0, 0, jitter) * _wanderDir).normalized;
        }

        return _wanderDir;
    }

    private bool TryGetFleeDir(out Vector2 fleeDir)
    {
        fleeDir = Vector2.zero;

        Vector2 toDiver = _diver.position - transform.position;
        float dist = toDiver.magnitude;
        if (dist >= _fleeRadius) return false;

        fleeDir = -toDiver.normalized;
        return true;
    }

    private Vector2 ApplyObstacleAvoidance(Vector2 desired)
    {
        if (desired.magnitude < EpsilonNum) return Vector2.zero;
        
        Vector2 origin = transform.position;
        Vector2 forward = desired.normalized;

        // 1. 정면으로 먼저 체크
        RaycastHit2D hit = Physics2D.Raycast(origin, forward, _rayDistance, _obstacleMask);

        // 앞에 아무것도 없으면 그냥 원하는 방향으로 감
        if (!hit)
            return forward;

        // 2. 벽을 따라 미끄러지는 방향 계산 (법선의 수직 방향 = 탄젠트)
        Vector2 normal  = hit.normal;
        Vector2 tangent = new Vector2(-normal.y, normal.x); // Perpendicular

        // 3. 원래 가려던 쪽과 더 비슷한 방향으로 선택
        if (Vector2.Dot(tangent, forward) < 0f)
            tangent = -tangent;

        return tangent.normalized;
    }
    
    private void CheckStuck()
    {
        Vector2 currPos = transform.position;
        float moved = (currPos - _lastPos).magnitude;

        // 거의 안 움직였으면 타이머 증가
        if (moved < 0.01f)
            _stuckTimer += Time.deltaTime;
        else
            _stuckTimer = 0f;

        _lastPos = currPos;

        if (_stuckTimer > MaxStuckTime)
        {
            // 끼임 탈출
            Vector2 escapeDir = GetEscapeDirectionFromWalls();
            
            EnterEscape(escapeDir,   _move.MaxSpeed * 1.5f);
            _stuckTimer = 0f;
        }
    }
   

    private Vector2 GetEscapeDirectionFromWalls()
    {
        Vector2 origin = transform.position;
        
        float checkDist = 1.5f; 

        Vector2 accumulated = Vector2.zero;
        bool foundWall = false;

        foreach (var dir in _escapeSampleDirs)
        {
            RaycastHit2D hit = Physics2D.Raycast(origin, dir, checkDist, _obstacleMask);
            if (hit)
            {
                foundWall = true;
                
                accumulated += hit.normal; //벽 표면의 법선 벡터
            }
        }

        if (!foundWall)
        {
            return Vector2.up;
        }
        
        return accumulated.normalized;
    }
    
#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying) return;

        Vector2 origin = transform.position;
        
        Vector2 dir =  _move.DesiredDir.normalized;
        Gizmos.color = Color.yellow;
        //Gizmos.DrawLine(origin, origin + dir * _rayDistance);
        
        // 2) 어그로 거리 / 공격 거리 원으로 표시
        if (_isAggressive)
        {
            // 어그로 범위
            Gizmos.color = new Color(1f, 0.5f, 0f, 0.4f); // 살짝 투명한 주황색
            Gizmos.DrawWireSphere(origin, _aggroRadius);

            // 공격 범위
            Gizmos.color = new Color(1f, 0f, 0f, 0.7f);   // 좀 더 진한 빨간색
            Gizmos.DrawWireSphere(origin, _attackRange);
        }

        // 3) 플레이어 방향 레이캐스트 (라인 색으로 막힘/안막힘 표시)
        if (_diver != null && _isAggressive)
        {
            Vector2 toDiver = (Vector2)_diver.position - origin;
            float distToDiver = toDiver.magnitude;

            if (distToDiver > 0.001f)
            {
                Vector2 dirToDiver = toDiver.normalized;

                // 실제로 레이캐스트 날려보기 (어그로 거리까지만)
                RaycastHit2D hit = Physics2D.Raycast(
                    origin,
                    dirToDiver,
                    _aggroRadius,
                    _obstacleMask
                );

                Color rayColor;

                if (hit.collider == null)
                {
                    // 아무것도 안 막고 있음 → 라인 초록색
                    rayColor = Color.green;
                }
                else if (hit.collider.transform == _diver)
                {
                    // 레이가 직접 플레이어에 닿음 → 초록색
                    rayColor = Color.green;
                }
                else
                {
                    // 중간에 벽 / 장애물이 막고 있음 → 빨간색
                    rayColor = Color.red;
                }

                Gizmos.color = rayColor;
                // 실제 레이 길이는 플레이어까지 or 어그로 반경까지만
                float lineLen = Mathf.Min(distToDiver, _aggroRadius);
                Gizmos.DrawLine(origin, origin + dirToDiver * lineLen);
            }
        }
    }
#endif
}