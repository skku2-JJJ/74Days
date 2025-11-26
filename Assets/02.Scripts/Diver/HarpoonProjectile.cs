using UnityEngine;

/// <summary>
/// 작살총 투사체에 할당하는 스크립트
/// </summary>
[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class HarpoonProjectile : MonoBehaviour
{
    
    [Header("지속 시간")]
    [SerializeField] private float _lifeTime = 1f;
    
    [Header("데미지")]
    [SerializeField] private float _baseDamage = 10f;
    [SerializeField] private float _extraDamageAtFullCharge = 20f;
    
    [Header("비행 속도 커브")]
    [SerializeField] private AnimationCurve _flightSpeedCurve =  AnimationCurve.Linear(0f, 1f, 1f, 1f);
    [SerializeField] private float _flightCurveDuration = 0.6f; // 커브 적용 시간
    
    [Header("회수 설정")]
    [SerializeField] private float _returnSpeed = 18f;  
    [SerializeField] private float _returnStopDistance = 1f;
    

    // 프로퍼티
    public bool IsReturning => _isReturning;
    public Vector3 Position => transform.position;
    
    // 참조
    private HarpoonShooter _owner;     
    
    // 컴포넌트
    private Rigidbody2D _rigid;
    
    private float _timer;
    
    private bool _isHit;
    private bool _isReturning;
   
    private Vector2 _moveDir;    
    private float _damage;
    private float _baseSpeed; 
    private float _flightElapsed;   // 날아간 시간
    

    private void Awake()
    {
        Init();
    }

    /// <summary>
    /// 투사체 생성 시 호출
    /// </summary>
    /// <param name="dir"> 발사 방향 </param>
    /// <param name="speed"> 발사 속도 </param>
    /// <param name="charge"> 차지 비율 </param>
    public void Launch(Vector2 dir, float speed, float charge, HarpoonShooter owner)
    {
        _owner = owner;
        
        _moveDir = dir.normalized;
        _baseSpeed = speed;
        _flightElapsed = 0f;
        
        // 차지 정도에 따른 데미지 적용
        _damage = _baseDamage + _extraDamageAtFullCharge * Mathf.Clamp01(charge);
        
        ApplyFlightVelocity(0f);
    }
    
    /// <summary>
    /// Hit 시 호출
    /// </summary>
    /// <param name="fish"></param>
    public void AttachToFish(Transform fish)
    {
        transform.SetParent(fish, worldPositionStays: true);

        // 물리 멈추기
        _rigid.linearVelocity = Vector2.zero;
        _rigid.angularVelocity = 0f;
        _rigid.simulated = false;
    }

    /// <summary>
    /// QTE 끝나고 다시 분리할 때 호출
    /// </summary>
    public void DetachFromFish()
    {
        transform.SetParent(null, worldPositionStays: true);
        _rigid.simulated = true;  
    }

    private void Init()
    {
        _rigid = GetComponent<Rigidbody2D>();
        _rigid.gravityScale = 0f;
    }

    private void Update()
    {
        if (_isReturning)
        {
            UpdateReturning();
            return;
        }
        
        if (_owner.IsCapturing)  return; // QTE 진행 중
        
        if (!_isHit)
        {
            _flightElapsed += Time.deltaTime;

            float t = 1f;
            if (_flightCurveDuration > 0f)
            {
                t = Mathf.Clamp01(_flightElapsed / _flightCurveDuration);
            }

            ApplyFlightVelocity(t);
        }
        
        _timer += Time.deltaTime;
        if (_timer >= _lifeTime)
        {
            // Hit 실패
            if (!_isHit)
            {
                BeginReturn();
            }
        }
    }
    
    /// <summary>
    /// HarpoonShooter에서 QTE 끝난 뒤 호출하는 공개 메서드
    /// </summary>
    public void BeginReturn()
    {
        StartReturning();
    }
    
    private void StartReturning()
    {
        if (_isReturning) return;

        
        _isReturning = true;
        _rigid.linearVelocity = Vector2.zero;
        _timer = 0f;
        
    }

    
    
    private void UpdateReturning()
    {
        if (_owner == null)
        {
            Destroy(gameObject);
            return;
        }

        Vector3 target = _owner.HarpoonMuzzleWorldPos; 
        transform.position = Vector3.MoveTowards(transform.position, target, _returnSpeed * Time.deltaTime);

        if (Vector3.Distance(transform.position, target) < _returnStopDistance)
        {
            _owner.OnHarpoonRetrieved(this, _isHit);
            Destroy(gameObject);
        }
    }
    
    private void ApplyFlightVelocity(float t01)
    {
        float mul = 1f;

        if (_flightSpeedCurve != null && _flightSpeedCurve.length > 0)
        {
            mul = _flightSpeedCurve.Evaluate(t01);   
        }

        float finalSpeed = _baseSpeed * mul;
        _rigid.linearVelocity = _moveDir * finalSpeed;
    }

    

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_isHit || _isReturning) return;
        if (!other.CompareTag("Fish")) return;
        
        IFishCapturable  fish = other.GetComponent<IFishCapturable >();
        if (fish == null) return;
        
        _isHit = true;
        _rigid.linearVelocity = Vector2.zero;
        
        // 1) 데미지 적용
        fish.TakeHarpoonHit(_damage, _moveDir);
        
        // 2) 캡처 가능 HP 이하라면 → QTE 없이 바로 포획
        if (fish.CanBeCaptured)
        {
            // 회수
            _owner.HandleCaptureResult(this, fish, true);
        }
        else
        {
            // QTE 진입
            _owner.StartCapture(fish, this);
        }
        
    }
}
