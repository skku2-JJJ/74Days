using UnityEngine;

[RequireComponent(typeof(FishMoveController))]
public class JellyFishAI : MonoBehaviour
{
     [Header("이동 설정")]
    [SerializeField] private float _moveSpeed = 1.2f;

    [Tooltip("방향을 바꾸는 최소 간격(초)")]
    [SerializeField] private float _minDirChangeInterval = 1.5f;

    [Tooltip("방향을 바꾸는 최대 간격(초)")]
    [SerializeField] private float _maxDirChangeInterval = 3.5f;

    [Header("위아래 흔들림(bob)")]
    [SerializeField] private float _bobAmplitude = 0.3f;
    [SerializeField] private float _bobFrequency = 2f;

    [Header("피해 설정")]
    [SerializeField] private int _damage = 10;
    [SerializeField] private float _hitCooldown = 0.6f;

    private Vector2 _moveDir;
    private float _dirTimer;
    private float _currentDirInterval;

    private float _time;
    private float _currentBobOffset;

    private float _lastHitTime = -999f;

    private void Awake()
    {
        PickNewDirection();
    }

    private void Update()
    {
        _time += Time.deltaTime;

        // 방향 전환 타이머
        _dirTimer += Time.deltaTime;
        if (_dirTimer >= _currentDirInterval)
        {
            PickNewDirection();
        }

        UpdateMovement();
    }

    private void PickNewDirection()
    {
        _dirTimer = 0f;
        _currentDirInterval = Random.Range(_minDirChangeInterval, _maxDirChangeInterval);

        // 완전 랜덤 방향
        _moveDir = Random.insideUnitCircle.normalized;
        if (_moveDir == Vector2.zero)
            _moveDir = Vector2.right;
    }

    private void UpdateMovement()
    {
        Vector3 pos = transform.position;

        // 기본 이동
        pos += (Vector3)(_moveDir * _moveSpeed * Time.deltaTime);

        // bobbing: 위아래 살짝 흔들기 (기존 위치 기준으로 오차 누적 안 되게)
        float newBob = Mathf.Sin(_time * _bobFrequency) * _bobAmplitude;
        pos.y += newBob - _currentBobOffset;
        _currentBobOffset = newBob;

        transform.position = pos;
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        if (Time.time - _lastHitTime < _hitCooldown) return;
        _lastHitTime = Time.time;

        // 1) 체력 데미지
        var diverStatus = other.GetComponent<DiverStatus>();
        if (diverStatus != null)
        {
            diverStatus.TakeDamage(_damage);
        }

        // 2) 넉백
        var diverMove = other.GetComponent<DiverMoveController>();
        if (diverMove != null)
        {
            Vector2 dir = ((Vector2)other.transform.position - (Vector2)transform.position).normalized;
            diverMove.AddRecoil(dir);
        }

        // 3) QTE 중이면 강제 실패
        var qte = other.GetComponent<HarpoonCaptureQTE>();
        if (qte != null && qte.IsCapturing)
        {
            qte.ForceFailCapture();
        }
    }
}
