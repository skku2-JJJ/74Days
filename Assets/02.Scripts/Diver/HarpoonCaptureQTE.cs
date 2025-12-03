using Unity.Cinemachine;
using UnityEngine;

/// <summary>
/// QTE 컨트롤러
/// </summary>
[RequireComponent(typeof(CinemachineImpulseSource), typeof(HarpoonShooter), typeof(InputController))]
public class HarpoonCaptureQTE : MonoBehaviour
{
    [Header("포획 QTE 설정")]
    [SerializeField] private float _captureDuration = 3f;
    [SerializeField] private float _captureGaugeDecayPerSecond = 0.4f;
    [SerializeField] private float _captureGaugeGainPerPress = 0.15f;
    [SerializeField] private float _startCaptureGuage = 0.3f;
    [SerializeField] private float _minCaptureGuage = 0.05f;

    [Header("난이도 스케일 (0=쉬움, 1=어려움)")] 
    [SerializeField] private QteDifficultyScale _durationScale;
    [SerializeField] private QteDifficultyScale _decayScale;
    [SerializeField] private QteDifficultyScale _gainScale;
    [SerializeField] private QteDifficultyScale _startGaugeScale;
    [SerializeField] private QteDifficultyScale _minGaugeScale;
    
    [Header("카메라 진동 설정")]
    [SerializeField] private float _shakeInterval = 0.1f;
    [SerializeField] private float _basicShakeStrength = 0.15f;
    [SerializeField] private float _minShakeStrengthFactor = 0.6f; 
    [SerializeField] private float _maxShakeStrengthFactor = 1.3f; 
    [SerializeField] private float _hitShakeStrength = 0.5f;
    [SerializeField] private float _successShakeStrength = 0.7f;
    [SerializeField] private float _failShakeStrength = 0.5f;
    
    private float _shakeTimer;

    // QTE 현재 적용 난이도
    private float _curCaptureDuration;
    private float _curGaugeDecayPerSecond;
    private float _curGaugeGainPerPress;
    private float _curStartGauge;
    private float _curMinCaptureGauge;
    
    // 참조
    private HarpoonShooter _shooter;
    private InputController _input;
    private CinemachineImpulseSource _impulseSource;

    // QTE 상태
    private bool _isCapturing;
    private float _captureGauge;
    private float _captureTimer;
    private IFishCapturable _targetFish;
    private HarpoonProjectile _projectile;

    public bool IsCapturing => _isCapturing;
    public float CaptureGauge01 => Mathf.Clamp01(_captureGauge);
    
    private void Awake()
    {
        Init();
    }

    private void Update()
    {
        if (!_isCapturing) return;
        
        UpdateCaptureQTE();
    }
    
    /// <summary>
    /// HarpoonShooter에서 QTE 시작 요청
    /// </summary>
    public void BeginCapture(IFishCapturable fish, HarpoonProjectile projectile)
    {
        if (fish == null || projectile == null) return;

        SetupDifficultyForFish(fish);
        
        _isCapturing = true;
        _captureGauge = _curStartGauge;
        _captureTimer = 0f;
        _targetFish = fish;
        _projectile = projectile;
        
        Time.timeScale = 1f;
        
        // 물고기 버둥 시작
        _targetFish.BeginCaptureStruggle();

        
        Vector3 playerPos = _shooter.transform.position;
        Vector3 fishPos = _targetFish.Transform.position;
        Vector3 dir = (playerPos - fishPos).normalized;

        // 카메라 진동
        _impulseSource.GenerateImpulse(dir * _hitShakeStrength);
        
    }
    
    /// <summary>
    /// QTE 강제 종료 시 (플레이어 피격 등) 호출
    /// </summary>
    public void ForceFailCapture()
    {
        if (!_isCapturing) return;
        
        FinishCapture(false);
    }

    private void Init()
    {
        _shooter = GetComponent<HarpoonShooter>();
        _input = GetComponent<InputController>();
        _impulseSource = GetComponent<CinemachineImpulseSource>();
        
    }
    
    private void SetupDifficultyForFish(IFishCapturable fish)
    {
        float difficulty01 = 0.5f;
        
        // 물고기별 기본 난이도
        if (fish is FishBase baseFish)
        {
            difficulty01 = baseFish.CurrentQteDifficulty01;
        }

        // QTE 난이도 적용
        _curCaptureDuration =
            _captureDuration * _durationScale.Evaluate(difficulty01);

        _curGaugeDecayPerSecond =
            _captureGaugeDecayPerSecond * _decayScale.Evaluate(difficulty01);

        _curGaugeGainPerPress =
            _captureGaugeGainPerPress * _gainScale.Evaluate(difficulty01);

        _curStartGauge =
            _startCaptureGuage * _startGaugeScale.Evaluate(difficulty01);

        _curMinCaptureGauge =
            _minCaptureGuage * _minGaugeScale.Evaluate(difficulty01);
    }
    
    private void UpdateCaptureQTE()
    {
        Time.timeScale = 1f;

        // 타이머
        _captureTimer += Time.unscaledDeltaTime;
        if (_captureTimer >= _curCaptureDuration)
        {
            FinishCapture(false);
            return;
        }

        // 게이지 자연 감소
        _captureGauge -= _curGaugeDecayPerSecond * Time.unscaledDeltaTime;
        
        // 게이지 올리기
        if (_input.IsPullKeyPressed)
        {
            _captureGauge += _curGaugeGainPerPress;
            
            /*
             보류
             FishHitFeedback fishHitFeedback = _targetFish.Transform.GetComponent<FishHitFeedback>();
            if (fishHitFeedback != null)
            {
                fishHitFeedback.PlayPunchScale();
            }*/
            
        }
        
        _captureGauge = Mathf.Clamp01(_captureGauge);
        
        if (_captureGauge <= _curMinCaptureGauge)
        {
            FinishCapture(false);
            return;
        }
        
        float struggle =  1f - Mathf.Clamp01(_captureGauge);
        _targetFish.UpdateCaptureStruggle(struggle);
        
        // 카메라 진동
        _shakeTimer += Time.unscaledDeltaTime;
        if (_shakeTimer >= _shakeInterval)
        {
            _shakeTimer = 0f;

            Vector3 player = _shooter.transform.position;
            Vector3 fishPos = _targetFish.Transform.position;
            Vector3 dir = (player - fishPos).normalized;

            float shakeStrength = _basicShakeStrength * Mathf.Lerp(_minShakeStrengthFactor, _maxShakeStrengthFactor, 1f - _captureGauge);
            _impulseSource?.GenerateImpulse(dir * shakeStrength);
        }
        
        
        
        if (_captureGauge >= 1f)
        {
            FinishCapture(true);
        }
    }

    private void FinishCapture(bool success)
    {
        Debug.Log($"FinishCapture success={success}, targetFish={_targetFish}, proj={_projectile}");
        
        _targetFish.EndCaptureStruggle();
        
        _shooter.HandleCaptureResult(_projectile, _targetFish, success);
        
        Vector3 playerPos = _shooter.transform.position;
        Vector3 fishPos = _targetFish.Transform.position;

        // 카메라 진동
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