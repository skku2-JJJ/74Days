using UnityEngine;

/// <summary>
/// 작살총 투사체에 할당하는 스크립트
/// </summary>
[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class HarpoonProjectile : MonoBehaviour
{
    [Header("지속 시간")]
    [SerializeField] private float _lifeTime = 3f;
    
    [Header("데미지")]
    [SerializeField] private float _baseDamage = 10f;
    [SerializeField] private float _extraDamageAtFullCharge = 20f;
    
    [Header("회수 설정")]
    [SerializeField] private float _returnSpeed = 18f;   // 플레이어 쪽으로 감기는 속도
    [SerializeField] private float _returnStopDistance = 0.05f;
    

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
        transform.right = -dir;
        
        // 차지 정도에 따른 데미지 적용
        _damage = _baseDamage + _extraDamageAtFullCharge * Mathf.Clamp01(charge);
    }

    private void Init()
    {
        _rigid = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
        
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
            if (!_isHit && _owner != null)
            {
                StartReturning();
            }
        }
    }
    
    private void StartReturning()
    {
        if (_isReturning) return;

        
        _isReturning = true;
        _rigid.linearVelocity = Vector2.zero;
        _timer = 0f;
        
        _animator.SetTrigger("Return");
       
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
        
        _isHit = true;

        // TODO: 여기서 나중에 물고기 판별해서 Hit/Miss 구분 가능
       
        _rigid.linearVelocity = Vector2.zero;
        _animator.SetTrigger("Hit");
        
        StartReturning();
    }
}
