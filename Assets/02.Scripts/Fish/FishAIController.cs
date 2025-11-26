using UnityEngine;

[RequireComponent(typeof(FishMoveController))]
public class FishAIController : MonoBehaviour
{
    [Header("Wander 설정")]
    [SerializeField] private float _wanderChangeInterval = 2.5f;
    [SerializeField] private float _wanderJitter = 15f; 

    [Header("장애물 회피")]
    [SerializeField] private LayerMask _obstacleMask;
    [SerializeField] private float _rayDistance = 1.2f;
    [SerializeField] private float _sideRayAngle = 35f;
    
    [Header("플레이어 회피")]
    [SerializeField] private Transform _diver;
    [SerializeField] private float _fleeRadius = 4f;
    [SerializeField] private float _fleeStrength = 1.5f;
    

    private FishMoveController _move;
    private Vector2 _wanderDir = Vector2.right;
    private float _wanderTimer;

    private const float FullAngle = 360f;
    private const float EpsilonNum = 0.001f;
    private const float HalfRatio = 0.5f;
    private void Awake()
    {
        Init();
    }
    

    private void Update()
    {
        if (_move.IsMovementLocked)  return;
           

        Vector2 dir = GetBaseDirection();         // Wander + Flee
        dir = ApplyObstacleAvoidance(dir);        // 장애물 회피

        _move.DesiredDir = dir;
    }

    private void Init()
    {
        _move = GetComponent<FishMoveController>();
        
        // 초기 방향 설정
        float angle = Random.Range(0, FullAngle);
        _wanderDir = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));
    }
    private Vector2 GetBaseDirection()
    {
        _wanderTimer += Time.deltaTime;
        if (_wanderTimer >= _wanderChangeInterval)
        {
            _wanderTimer = 0f;

            float jitter = Random.Range(-_wanderJitter, _wanderJitter);
            float rad = jitter * Mathf.Deg2Rad;

            Vector2 rotated = new Vector2(
                _wanderDir.x * Mathf.Cos(rad) - _wanderDir.y * Mathf.Sin(rad),
                _wanderDir.x * Mathf.Sin(rad) + _wanderDir.y * Mathf.Cos(rad)
            );
            _wanderDir = rotated.normalized;
        }

        Vector2 result = _wanderDir;
        
        if (_diver != null)
        {
            Vector2 toDiver = _diver.position - transform.position;
            float dist = toDiver.magnitude;
            
            // 일정 거리 이내 플레이어 있다면 반대 방향으로
            if (dist < _fleeRadius)
            {
                Vector2 fleeDir = -toDiver.normalized;
                float weight = Mathf.Clamp01(1f - dist / _fleeRadius); // 가까울수록 강하게
                result = (result + fleeDir * _fleeStrength * weight).normalized;
            }
        }

        return result;
    }

    private Vector2 ApplyObstacleAvoidance(Vector2 desired)
    {
        if (desired.sqrMagnitude < EpsilonNum)  return desired;
           

        Vector2 origin = transform.position;
        Vector2 forward = desired.normalized;

        bool hitFront = Physics2D.Raycast(origin, forward, _rayDistance, _obstacleMask);
        if (!hitFront) return desired;
            

        // 왼쪽, 오른쪽 후보 중 더 여유 있는 쪽으로 회피
        Vector2 leftDir = Quaternion.Euler(0, 0, _sideRayAngle) * forward;
        Vector2 rightDir = Quaternion.Euler(0, 0, -_sideRayAngle) * forward;

        bool hitLeft = Physics2D.Raycast(origin, leftDir, _rayDistance, _obstacleMask);
        bool hitRight = Physics2D.Raycast(origin, rightDir, _rayDistance, _obstacleMask);

        if (!hitLeft && hitRight) return leftDir;
        if (!hitRight && hitLeft) return rightDir;
        if (!hitLeft && !hitRight)
        {
            // 둘 다 비면 랜덤
            return (Random.value < HalfRatio ? leftDir : rightDir);
        }

        // 셋 다 막혀 있으면 뒤로
        return -forward;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying) return;

        Vector2 origin = transform.position;
        Vector2 dir = _move != null ? _move.DesiredDir.normalized : Vector2.right;

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(origin, origin + dir * _rayDistance);
    }
#endif
}