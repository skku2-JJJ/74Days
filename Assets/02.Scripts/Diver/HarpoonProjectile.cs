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
    private Animator _animator;
    
    private float _timer;
    
    private bool _isHit;
    private bool _isReturning;
   
    
    private float _damage;

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
        
        dir.Normalize();
        _rigid.linearVelocity = dir * speed;
        transform.right = dir;
        
        // 차지 정도에 따른 데미지 적용
        _damage = _baseDamage + _extraDamageAtFullCharge * Mathf.Clamp01(charge);
    }

    private void Init()
    {
        _rigid = GetComponent<Rigidbody2D>();
        //_animator = GetComponent<Animator>();
        
        _rigid.gravityScale = 0f;
    }

    private void Update()
    {
        if (_isReturning)
        {
            UpdateReturning();
            return;
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
        
        //_animator.SetTrigger("Return");
       
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
    

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_isHit || _isReturning) return;
        if (!other.CompareTag("Fish")) return;
        
        FishBase fish = other.GetComponent<FishBase>();
        if (fish == null) return;
        
        _isHit = true;
        _rigid.linearVelocity = Vector2.zero;
        
        //_animator.SetTrigger("Hit");
        
        // 1) 데미지 적용
        fish.TakeHarpoonHit(_damage);
        // 2) 캡처 가능 HP 이하라면 → QTE 없이 바로 포획
        if (fish.CanBeCaptured)
        {
            // 물고기 작살에 붙이기
            fish.transform.SetParent(transform, true);
            
            // 회수
            BeginReturn();
        }
        else
        {
            // QTE 진입
            transform.SetParent(fish.transform, true);
            _owner.StartCapture(fish, this);
        }
        
        StartReturning();
    }
}
