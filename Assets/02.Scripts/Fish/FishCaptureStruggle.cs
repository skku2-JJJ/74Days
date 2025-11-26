using UnityEngine;

public class FishCaptureStruggle : MonoBehaviour
{
    [Header("비주얼 처리 오브젝트 참조")]
    [SerializeField] private Transform _visualTransform;
    
    [Header("QTE 캡처 관련")]
    [SerializeField] private float _escapeMoveSpeed = 3f;   // 도망 속도
    [SerializeField] private float _maxStruggleAngle = 15f; // 몸통 좌우 흔들 각도
    [SerializeField] private float _struggleFrequency = 7f; // 초당 흔들림 횟수
    
    private Transform _capturedDiver;
    private float _currentStruggleIntensity; 
    
    private Rigidbody2D _rigid;
    private Animator _animator;
    
    private bool _iscapturedByHarpoon;
    
    private void Awake()
    {
       Init();
    }

    private void Init()
    {
        _rigid = GetComponent<Rigidbody2D>();
        _animator = GetComponentInChildren<Animator>();
    }
    public void Begin(Transform diver)
    {
        _iscapturedByHarpoon = true;
        _capturedDiver = diver;
        _currentStruggleIntensity = 0.5f; // 시작 기본값
        
        _animator?.SetTrigger("Hit"); 
    }
    public void UpdateIntensity(float struggleIntensity)
    {
        _currentStruggleIntensity = Mathf.Clamp01(struggleIntensity);
    }

    public void End()
    {
        _iscapturedByHarpoon = false;
        _capturedDiver = null;
        _currentStruggleIntensity = 0f;

        _visualTransform.localRotation = Quaternion.identity;
        _rigid.linearVelocity = Vector2.zero;

        _animator?.SetTrigger("Escape"); 
    }
   
    private void Update()
    {
        if (!_iscapturedByHarpoon || _capturedDiver == null) return;

        UpdateCapturedMovement();
        UpdateCapturedWiggle();
    }
    
    private void UpdateCapturedMovement()
    {
        if (_capturedDiver == null) return;
        
        Vector2 dirAway = (transform.position - _capturedDiver.position).normalized;
        
        float speed = _escapeMoveSpeed * (0.5f + _currentStruggleIntensity);
        _rigid.linearVelocity = dirAway * speed;
    }

    private void UpdateCapturedWiggle()
    {
        // 몸 흔들기 (좌우 회전)
        float t = Time.time * _struggleFrequency;
        float angle = Mathf.Sin(t) * _maxStruggleAngle * _currentStruggleIntensity;
        _visualTransform.localRotation = Quaternion.Euler(0f, 0f, angle);
    }

}