using UnityEngine;

[RequireComponent(typeof(FishMoveController))]
public class FishAIController : MonoBehaviour
{
    private enum EFishState
    {
        Idle,
        Wander,
        Escape
    }

    [Header("State 결정 주기")]
    [SerializeField] private float _minDecideStateDuration = 2.0f;
    [SerializeField] private float _maxDecideStateDuration = 4.0f;
    
    [Header("Wander 설정")]
    [SerializeField] private float _minDecideDirDuration = 0.8f;
    [SerializeField] private float _maxDecideDirDuration = 1.4f;
    [SerializeField] private float _wanderChangeInterval = 1.0f;
    [SerializeField] private float _wanderJitter = 70f; 
    
    [Header("Idle 설정")]
    [SerializeField] private float _idleMinDuration = 1.0f;
    [SerializeField] private float _idleMaxDuration = 3.0f;
    [SerializeField] private float _idleChanceAfterWander = 0.4f;

    private EFishState _eFishState = EFishState.Wander;
    private float _stateTimer;
    private float _stateDuration;
    
    [Header("장애물 회피")]
    [SerializeField] private LayerMask _obstacleMask;
    [SerializeField] private float _rayDistance = 1.2f;
    
    [Header("플레이어 회피")]
    private Transform _diver;
    [SerializeField] private float _fleeRadius = 4f;
    [SerializeField] private float _fleeStrength = 1.5f;
    
    [Header("Escape 설정")]
    [SerializeField] private float _escapeDuration = 3f;
    
    private FishMoveController _move;
    private Vector2 _wanderDir = Vector2.right;
    private float _wanderTimer;

    private Vector2 _escapeDir;
    
    private const float FullAngle = 360f;
    private const float EpsilonNum = 0.001f;
    private const float DirCorrection = 0.3f;
    private void Awake()
    {
        Init();
        EnterWander();
    }
    

    private void Update()
    {
        if (_move.IsMovementLocked)  return;
           

        _stateTimer += Time.deltaTime;

        if (_stateTimer >= _stateDuration)
        {
            SwitchState();
        }

        // 플레이어 회피
        /*if (_diver != null && TryGetFleeDir(out Vector2 fleeDir))
        {
            _move.DesiredDir = ApplyObstacleAvoidance(fleeDir);
            return;
        }*/

        DecideMoveDir();
    }

    private void Init()
    {
        _move = GetComponent<FishMoveController>();
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

        // 장애물 탐색 방향 (왼45, 오른45, 왼90, 오90)
        float[] angles = { 0f, 45f, -45f, 90f, -90f };

        foreach (float ang in angles)
        {
            Vector2 dir = Quaternion.Euler(0, 0, ang) * forward;

            bool hit = Physics2D.Raycast(origin, dir, _rayDistance, _obstacleMask);
            if (!hit)
            {
                // 열린 방향을 찾으면 바로 그쪽으로 간다
                return dir.normalized;
            }
        }

        // 완전 막힌 경우 → 위로 살짝 치켜오르기 
        return (forward + Vector2.up * DirCorrection).normalized;
    }
    
   

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying) return;

        Vector2 origin = transform.position;
        Vector2 dir =  _move.DesiredDir.normalized;

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(origin, origin + dir * _rayDistance);
    }
#endif
}