using UnityEngine;

[RequireComponent(typeof(FishMoveController))]
public class JellyFishAI : MonoBehaviour
{
      [Header("Movement Settings")]
    [SerializeField] private float _wanderMaxSpeed = 0.5f;            
    [SerializeField] private float _directionChangeInterval = 4.0f;  
    [SerializeField] private float _wanderJitter = 40f;              

    private FishMoveController _moveController;
    private Vector2 _wanderDir = Vector2.up;
    private float _timer;

    [Header("Damage Settings")]
    [SerializeField] private int _damageAmount = 10;
    [SerializeField] private float _hitCooldown = 0.5f;

    private float _lastHitTime = -999f;
    private Animator _animator;

    void Awake()
    {
        Init();
    }
   

    private void Update()
    {
        if (_moveController.IsMovementLocked) return;

        _timer += Time.deltaTime;
        
        if (_timer >= _directionChangeInterval)
        {
            SetNewRandomDirection();
            _timer = 0f;
        }

       
        _moveController.DesiredDir = _wanderDir;
    }

    private void Init()
    {
        _moveController = GetComponent<FishMoveController>();
        _animator = GetComponent<Animator>();
        _moveController.SetOverrideSpeed(_wanderMaxSpeed, float.MaxValue);
        
        _wanderDir = Random.insideUnitCircle.normalized;
        if (_wanderDir == Vector2.zero)
            _wanderDir = Vector2.right;

        _timer = 0f;
    }

    private void SetNewRandomDirection()
    {
        float jitterAngle = Random.Range(-_wanderJitter, _wanderJitter);
        _wanderDir = (Quaternion.Euler(0, 0, jitterAngle) * _wanderDir).normalized;

        if (_wanderDir == Vector2.zero)
            _wanderDir = Random.insideUnitCircle.normalized;
    }
    

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (!collision.collider.CompareTag("Player")) return;

        if (Time.time - _lastHitTime < _hitCooldown) return;
        _lastHitTime = Time.time;

        GameObject playerObj = collision.gameObject;
        
        DiverStatus diverStatus = playerObj.GetComponent<DiverStatus>();
        if (diverStatus != null)
            diverStatus.TakeDamage(_damageAmount);
        
        var diverMove = playerObj.GetComponent<DiverMoveController>();
        if (diverMove != null)
        {
            Vector2 knockDir =  ((Vector2)playerObj.transform.position - (Vector2)transform.position).normalized;
            diverMove.AddRecoil(knockDir);
        }
        
        HarpoonCaptureQTE qte = playerObj.GetComponent<HarpoonCaptureQTE>();
        if (qte != null && qte.IsCapturing)
            qte.ForceFailCapture();
        
        _animator.SetTrigger("Attack");
    }
}
