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
    
    [Header("타겟(플레이어)")]
    private Transform _diver;
    
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
    [SerializeField] private float _aggroRadius = 7f;
    [SerializeField] private float _chaseStateDuration = 5f;
    [SerializeField] private float _attackRange = 4f;    
    [SerializeField] private float _attackCooltime = 1.5f;
    [SerializeField] private float _attackDashSpeedMultiplier = 2.5f; 
    [SerializeField] private float _attackDashDuration = 0.25f;       
    private Vector2 _attackDir;                                 
    private float _attackCoolTimer = 0f;
    private float _rangeOffsetFactor = 1.2f;
    
    
    
    [Header("장애물 회피")]
    [SerializeField] private LayerMask _obstacleMask;
    [SerializeField] private float _rayDistance = 1.2f;
    private Vector2 _lastPos;
    private float _stuckTimer;
    
    
    
    [Header("Escape 설정")]
    [SerializeField] private float _escapeDuration = 3f;
    private float _escapeSpeedFactor = 1.5f;
    private Vector2 _escapeDir;
    
    private const float FullAngle = 360f;
    private const float EpsilonNum = 0.001f;
    private const float StuckCriterion = 0.01f;
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

        // 1순위 -> Escape
        if (_eFishState == EFishState.Escape)
        {
            _stateTimer += Time.deltaTime;

            if (_stateTimer >= _stateDuration)
            {
                EnterWander();
            }

            DecideMoveDir();
            CheckStuck();
            return; 
        }
        
        // 2순위 -> Attack
        if (_isAggressive)
        {
            HandleAggro();  
        }
        
        if (_isAggressive && _eFishState != EFishState.Escape)
        {
            UpdateAggroState();  
        }

        // 3순위 -> Idle/Wander
        if (_eFishState == EFishState.Idle || _eFishState == EFishState.Wander)
        {
            _stateTimer += Time.deltaTime;

            if (_stateTimer >= _stateDuration)
            {
                SwitchState();
            }
        }

        
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

    private void HandleAggro()
    {
        if (_diver == null)  return;
        
        Vector2 toDiver = (Vector2)_diver.position - (Vector2)transform.position;
        float dist = toDiver.magnitude;
        
        if (dist <= _attackRange && _attackCoolTimer <= 0f)
        {
            EnterAttack();
            return;
        }
        
        if (dist <= _aggroRadius)
        {
            if (_eFishState != EFishState.Chase && _eFishState != EFishState.Attack)
            {
                EnterChase();  
            }
            return;
        }
        
        if (_eFishState == EFishState.Chase || _eFishState == EFishState.Attack)
        {
            EnterWander();
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
                if (_diver != null)
                {
                    dir = ((Vector2)_diver.position - (Vector2)transform.position).normalized;
                }
                break;
            
            case EFishState.Attack:
                dir = _attackDir;
                break;
        }

        
        if (_eFishState == EFishState.Attack)
        {
            _move.DesiredDir = _attackDir;
        }
        else
        {
            dir = ApplyObstacleAvoidance(dir);
            _move.DesiredDir = dir;
        }
       
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
        _stateDuration = _chaseStateDuration; 
    }

    private void EnterAttack()
    {
        _eFishState = EFishState.Attack;
        _stateTimer = 0f;
        _stateDuration = _attackDashDuration; 
        _attackCoolTimer = 0f;

        
        _attackDir = ((Vector2)_diver.position - (Vector2)transform.position).normalized;
        _move.SetOverrideSpeed(_move.MaxSpeed * _attackDashSpeedMultiplier, _attackDashDuration);
        
        Invoke("TryHitPlayer", _attackDashDuration);
        _animator.SetTrigger("Attack");
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
        if (dist > _attackRange * _rangeOffsetFactor) return; 
        
        // 데미지 적용
        DiverStatus diverStatus = _diver.GetComponent<DiverStatus>();
        if (diverStatus != null)
        {
            diverStatus.TakeDamage();  
            Debug.Log("Attack!");
        }
        
        // 넉백
        DiverMoveController diverMove = _diver.GetComponent<DiverMoveController>();
        if (diverMove != null)
        {
            Vector2 knockDir = ((Vector2)_diver.position - (Vector2)transform.position).normalized;
            diverMove.AddRecoil(knockDir);
        }
        
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

    
    private Vector2 ApplyObstacleAvoidance(Vector2 desired)
    {
        if (desired.magnitude < EpsilonNum) return Vector2.zero;
        
        Vector2 origin = transform.position;
        Vector2 forward = desired.normalized;
        
        RaycastHit2D hit = Physics2D.Raycast(origin, forward, _rayDistance, _obstacleMask);
        
        if (!hit)  return forward;
        
        // 벽을 따라 미끄러지는 방향 계산 
        Vector2 normal  = hit.normal;
        Vector2 tangent = new Vector2(-normal.y, normal.x); // Perpendicular

        // 원래 가려던 쪽과 더 비슷한 방향으로 선택
        if (Vector2.Dot(tangent, forward) < 0f)
            tangent = -tangent;

        return tangent.normalized;
    }
    
    private void CheckStuck()
    {
        Vector2 currPos = transform.position;
        float moved = (currPos - _lastPos).magnitude;

        // 거의 안 움직였으면 타이머 증가
        if (moved < StuckCriterion)
            _stuckTimer += Time.deltaTime;
        else
            _stuckTimer = 0f;

        _lastPos = currPos;

        if (_stuckTimer > MaxStuckTime)
        {
            // 끼임 탈출
            Vector2 escapeDir = GetEscapeDirectionFromWalls();
            
            EnterEscape(escapeDir,   _move.MaxSpeed * _escapeSpeedFactor);
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
                
                accumulated += hit.normal; 
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
        
       
        if (_isAggressive)
        {
            // 어그로 범위
            Gizmos.color = Color.magenta; 
            Gizmos.DrawWireSphere(origin, _aggroRadius);

            // 공격 범위
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(origin, _attackRange);
        }
    }
#endif
}