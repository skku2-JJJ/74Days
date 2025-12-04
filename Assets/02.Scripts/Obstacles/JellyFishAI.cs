using UnityEngine;

[RequireComponent(typeof(FishMoveController))]
public class JellyFishAI : Obstacle
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
    
    private GameObject _diver;
    private DiverStatus _diverStatus;
    private DiverMoveController _diverMove;
    private HarpoonCaptureQTE _qte;
    

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
        
        _diver = GameObject.FindGameObjectWithTag("Player");
        _diverStatus = _diver.GetComponent<DiverStatus>();
        _diverMove = _diver.GetComponent<DiverMoveController>();
        _qte = _diverStatus.GetComponent<HarpoonCaptureQTE>();

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
        
        
        if (_diverStatus != null)
            _diverStatus.TakeDamage(_damageAmount);
        
        if (_diverMove != null)
        {
            Vector2 knockDir =  ((Vector2)_diver.transform.position - (Vector2)transform.position).normalized;
            _diverMove.AddRecoil(knockDir, 2f);
        }
        
        if (_qte != null && _qte.IsCapturing)
            _qte.ForceFailCapture();
        
        _animator.SetTrigger("Attack");
    }
}