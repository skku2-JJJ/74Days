using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class HarpoonProjectile : MonoBehaviour
{
    
    [Header("데미지 설정")]
    [SerializeField] private float _baseDamage = 10f;
    [SerializeField] private float _extraDamageAtFullCharge = 20f;
    
    private float _lifeTime = 3f;

    private Rigidbody2D _rigid;
    private Animator _animator;
    
    private float _timer;
    private bool _isHit;
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
    public void Launch(Vector2 dir, float speed, float charge)
    {
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
        _timer += Time.deltaTime;
        if (_timer >= _lifeTime)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_isHit) return;
        if (!other.CompareTag("Fish")) return;
        
        _isHit = true;

        // TODO: 여기서 적/지형 판정해서 데미지, 피격 이펙트 처리
        // var enemy = other.GetComponent<Enemy>();
        // if (enemy != null) enemy.TakeDamage(_damage);

        _rigid.linearVelocity = Vector2.zero;
        _animator.SetTrigger("Hit");
        
        //Destroy(gameObject, 0.05f);
    }
}
