using Unity.Cinemachine;
using UnityEngine;

/// <summary>
/// QTE 컨트롤러
/// </summary>
public class HarpoonCaptureQTE : MonoBehaviour
{
    [Header("포획 QTE 설정")]
    [SerializeField] private float _captureDuration = 3f;
    [SerializeField] private float _captureGaugeDecayPerSecond = 0.4f;
    [SerializeField] private float _captureGaugeGainPerPress = 0.15f;
    
    [Header("카메라 진동 설정")]
    [SerializeField] private float _shakeInterval = 0.1f;
    [SerializeField] private float _basicShakeStrength = 0.15f; 
    [SerializeField] private float _hitShakeStrength = 0.5f;
    [SerializeField] private float _successShakeStrength = 0.7f;
    [SerializeField] private float _failShakeStrength = 0.5f;
    
    private float _shakeTimer;

    // 참조
    private HarpoonShooter _shooter;
    private InputController _input;
    private CinemachineImpulseSource _impulseSource;

    // QTE 상태
    private bool _isCapturing;
    private float _captureGauge;
    private float _captureTimer;
    private FishBase _targetFish;
    private HarpoonProjectile _projectile;

    public bool IsCapturing => _isCapturing;
    public float CaptureGauge01 => Mathf.Clamp01(_captureGauge);
    
    private void Awake()
    {
        _shooter = GetComponent<HarpoonShooter>();
        _input = GetComponent<InputController>();
        _impulseSource = GetComponent<CinemachineImpulseSource>();
    }

    private void Update()
    {
        if (!_isCapturing) return;
        
        UpdateCaptureQTE();
    }
    
    /// <summary>
    /// HarpoonShooter에서 QTE 시작 요청
    /// </summary>
    public void BeginCapture(FishBase fish, HarpoonProjectile projectile)
    {
        if (fish == null || projectile == null) return;

        _isCapturing = true;
        _captureGauge = 0f;
        _captureTimer = 0f;
        _targetFish = fish;
        _projectile = projectile;
        
        Time.timeScale = 1f;
        
        Vector3 playerPos = _shooter.transform.position;
        Vector3 fishPos = _targetFish.transform.position;
        Vector3 dir = (playerPos - fishPos).normalized;

        _impulseSource.GenerateImpulse(dir * _hitShakeStrength);
        
    }

    private void UpdateCaptureQTE()
    {
        Time.timeScale = 1f;

        // 타이머
        _captureTimer += Time.unscaledDeltaTime;
        if (_captureTimer >= _captureDuration)
        {
            FinishCapture(false);
            return;
        }

        // 게이지 자연 감소
        _captureGauge -= _captureGaugeDecayPerSecond * Time.unscaledDeltaTime;
        _captureGauge = Mathf.Max(0f, _captureGauge);

        // 스페이스 연타로 게이지 올리기
        if (_input.IsPullKeyPressed)
        {
            _captureGauge += _captureGaugeGainPerPress;
        }
        
        _shakeTimer += Time.unscaledDeltaTime;
        if (_shakeTimer >= _shakeInterval)
        {
            _shakeTimer = 0f;

            Vector3 player = _shooter.transform.position;
            Vector3 fishPos = _targetFish.transform.position;
            Vector3 dir = (player - fishPos).normalized;

            _impulseSource?.GenerateImpulse(dir * _basicShakeStrength);
        }
        
        if (_captureGauge >= 1f)
        {
            FinishCapture(true);
        }
    }

    private void FinishCapture(bool success)
    {
        Debug.Log($"FinishCapture success={success}, targetFish={_targetFish}, proj={_projectile}");
        
       
        Vector3 playerPos = _shooter.transform.position;
        Vector3 fishPos = _targetFish.transform.position;

        if (success)
        {
            Vector3 dir = (playerPos - fishPos).normalized;
            _impulseSource.GenerateImpulse(dir * _successShakeStrength);
        }
        else
        {
            Vector3 dir = (fishPos - playerPos).normalized;
            _impulseSource.GenerateImpulse(dir * _failShakeStrength);
        }
        

        _isCapturing = false;

        // Shooter에게 결과 전달 
        _shooter.HandleCaptureResult(_projectile, _targetFish, success);

        _targetFish = null;
        _projectile = null;
        _captureGauge = 0f;
        _captureTimer = 0f;
        
    }

    private void OnDisable()
    {
        if (_isCapturing)
        {
            FinishCapture(false);
        }
    }
}