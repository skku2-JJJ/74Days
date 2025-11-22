using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(Collider2D))]
public class HarpoonProjectile : MonoBehaviour
{
    [SerializeField] private float _lifeTime = 3f;

    private Rigidbody2D _rigid;
    private Animator _animator;
    
    private float _timer;
    private bool _isHit;

    private void Awake()
    {
        Init();
    }

    public void Launch(Vector2 dir, float speed)
    {
        dir.Normalize();
        _rigid.linearVelocity = dir * speed;
        transform.right = -dir;
    }

    private void Init()
    {
        _rigid = GetComponent<Rigidbody2D>();
        _animator = GetComponent<Animator>();
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
