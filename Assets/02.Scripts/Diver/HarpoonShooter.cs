using System.Collections;
using UnityEngine;

/// <summary>
/// 조준/발사 컨트롤러
/// </summary>
[RequireComponent(typeof(DiverMoveController),typeof(InputController))]
public class HarpoonShooter : MonoBehaviour
{
    [Header("작살 프리펩")]
    [SerializeField] private GameObject _harpoonPrefab;

    [Header("발사 설정")]
    [SerializeField] private float _minHarpoonSpeed = 8f;
    [SerializeField] private float _maxHarpoonSpeed = 18f;
    [SerializeField] private float _maxChargeTime = 1.2f;
    [SerializeField] private float _fireCoolTime = 0.4f;
    [SerializeField] private Vector2 _firePosOffset = new Vector2(1f, 0.5f); // 다이버 기준 발사 위치

    [Tooltip("차지량(0~1)을 속도/데미지에 어떻게 반영할지 커브")]
    [SerializeField] private AnimationCurve _chargeCurve;
    
    [Header("조준 / 슬로우 모션")]
    [SerializeField] private float _aimTimeScale = 0.4f;       
    [SerializeField] private float _timeScaleLerpSpeed = 10f; 
    
    [Header("포획 QTE 설정")]
    [SerializeField] private float _captureDuration = 3f;
    [SerializeField] private float _captureGaugeDecayPerSecond = 0.4f;
    [SerializeField] private float _captureGaugeGainPerPress = 0.15f;
    
    // 컴포넌트 / 참조
    private Animator _animator;
    private InputController _inputController;
    private DiverMoveController _moveController;
    private Camera _mainCam;
    
    // 타이머
    private float _coolTimer;
    private float _chargeTimer;
    
    private HarpoonProjectile _currentProjectile;
    
    // 플래그 변수
    private bool _isAiming;
    private bool _isCharging;
    private bool _hasHarpoonOut;      
    private bool _canAim = true;       
    
    // QTE 상태
    private bool _isCapturing;
    private float _captureGauge;
    private float _captureTimer;
    private FishBase _targetFish;
    
    
    // 프로퍼티
    public bool IsAiming => _isAiming;
    public bool IsCharging => _isCharging;
    public bool IsCapturing => _isCapturing;
    public float CaptureGauge01 => Mathf.Clamp01(_captureGauge);

    
    public HarpoonProjectile CurrentProjectile => _currentProjectile;
    public Vector3 HarpoonMuzzleWorldPos => (Vector2)transform.position + _firePosOffset;
    public Vector3 HarpoonReturnPoint => transform.position + (Vector3)_firePosOffset;
    public float ChargeRatio
    {
        get
        {
            if (!_isCharging) return 0f;
            if (_maxChargeTime <= 0f) return 1f;
            return Mathf.Clamp01(_chargeTimer / _maxChargeTime);
        }
    }
    public float CaptureTimeRatio01
    {
        get
        {
            return _captureDuration > 0f ? Mathf.Clamp01(_captureTimer / _captureDuration) : 0f;
        }
    }
    

    // 상수
    private const float MinShootDistance = 0.0001f;

    private void Awake()
    {
        Init();
    }

    private void Update()
    {
        _coolTimer += Time.unscaledDeltaTime; 

        // QTE 진행 중이면 여기서만 처리
        if (_isCapturing)
        {
            UpdateCaptureQTE();
            return;
        }
        
        UpdateAimState();
        UpdateTimeScale();
        UpdateCharge();
    }

    private void Init()
    {
        _animator =  GetComponentInChildren<Animator>();
        _inputController = GetComponent<InputController>();
        _moveController = GetComponent<DiverMoveController>();
        
        _mainCam = Camera.main;
        
        if (_chargeCurve == null || _chargeCurve.length == 0)
        {
            _chargeCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
        }
    }
    private void UpdateAimState()
    {
        bool aimInput  = _inputController.IsAimButtonHeld;
        bool prevAiming = _isAiming;
        
        _isAiming = aimInput && !_hasHarpoonOut;
        
        
        // 상태 전환 시 애니 트리거
        if (_isAiming)
        {
            _animator.SetTrigger("Aim");
        }
        else 
        {
            _animator.SetTrigger("AimEnd");
        }
        
        // 조준 종료 시 차지 초기화
        if(!_isAiming && _isCharging)
        {
            _isCharging = false;
            _chargeTimer = 0f;
        }
    }

    private void UpdateTimeScale()
    {
        float target = 1f;
        
        if (!_hasHarpoonOut && _isAiming)
        {
            target = _aimTimeScale;
        }
        
        float newScale = Mathf.Lerp(Time.timeScale, target, _timeScaleLerpSpeed * Time.unscaledDeltaTime);
        Time.timeScale = newScale;
    }

    private void UpdateCharge()
    {
        if (!_isAiming) return;             
        if (_coolTimer < _fireCoolTime) return;
        if (_hasHarpoonOut) return;
        
        bool chargePressed = _inputController.IsChargeButtonPressed;
        bool chargeHeld = _inputController.IsChageButtonHeld;
        bool chargeReleased = _inputController.IsChargeButtonReleased;
        
        // 차지 시작
        if (!_isCharging && chargePressed)
        {
            _isCharging = true;
            _chargeTimer = 0f;
            
        }
        
        // 차지 중
        if (_isCharging && chargeHeld)
        {
            _chargeTimer += Time.unscaledDeltaTime;
            _chargeTimer = Mathf.Min(_chargeTimer, _maxChargeTime);
        }
        
        // 차지 끝, 발사
        if (_isCharging && chargeReleased)
        {
            
            float charge = ChargeRatio;
            FireToMouse(charge);

            _isCharging = false;
            _chargeTimer = 0f;
            _coolTimer = 0f;
            
           
        }
    }

    private void FireToMouse(float charge)
    {
        Time.timeScale = 1f;
        
        Vector3 mouseWorld = _mainCam.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0;

        Vector2 origin = (Vector2)transform.position + _firePosOffset;
        Vector2 dir = (mouseWorld - (Vector3)origin);
        if (dir.sqrMagnitude < MinShootDistance) return;

        dir.Normalize();

        // 차지 커브 적용
        float curved = _chargeCurve.Evaluate(charge);
        float speed = Mathf.Lerp(_minHarpoonSpeed, _maxHarpoonSpeed, curved);
        
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Quaternion rot = Quaternion.Euler(0f, 0f, angle);

        GameObject projObj = Instantiate(_harpoonPrefab, origin, rot);
        HarpoonProjectile proj = projObj.GetComponent<HarpoonProjectile>();
        proj.Launch(dir, speed, charge, this);

        
        
        _currentProjectile = proj;
        _hasHarpoonOut = true;      
            
                
        
        // TODO:  카메라 셰이크 / 발사 사운드 / 이펙트 호출
        _animator.SetTrigger("Shoot");
       
    }

    /// <summary>
    /// 작살이 플레이어에게 완전히 돌아왔을 때 호출
    /// (reloading == returning : 돌아온 순간부터 다시 조준 가능)
    /// </summary>
    public void OnHarpoonRetrieved(HarpoonProjectile proj, bool hit)
    {
        if (_currentProjectile == proj)
        {
            _currentProjectile = null;
        }

        _hasHarpoonOut = false;
        
        // projectile에 물고기가 붙어 있다면 → 물고기 획득!
        FishBase fish = proj.GetComponentInChildren<FishBase>();
        if (fish != null)
        {
            fish.Capture();
        }
       
    }
    
    
    public void StartCapture(FishBase fish, HarpoonProjectile projectile)
    {
        if (fish == null || projectile == null) return;

        _isCapturing = true;
        _captureGauge = 0f;
        _captureTimer = 0f;
        _targetFish = fish;

        _currentProjectile = projectile;
        _hasHarpoonOut = true; 
        
        Time.timeScale = 1f;

        // TODO: QTE UI , 애니메이션 트리거 
    }

    private void UpdateCaptureQTE()
    {
        Time.timeScale = 1f;

        _captureTimer += Time.unscaledDeltaTime;
        if (_captureTimer >= _captureDuration)
        {
            EndCapture(false);
            return;
        }

        // 게이지 자연 감소
        _captureGauge -= _captureGaugeDecayPerSecond * Time.unscaledDeltaTime;
        _captureGauge = Mathf.Max(0f, _captureGauge);

        // 스페이스 연타로 게이지 올리기
        if (_inputController.IsPullKeyPressed)
        {
            _captureGauge += _captureGaugeGainPerPress;
        }

        // TODO: QTE 게이지 UI 업데이트

        if (_captureGauge >= 1f)
        {
            EndCapture(true);
        }
    }

    private void EndCapture(bool success)
    {
        Debug.Log($"EndCapture success={success}, targetFish={_targetFish}, proj={_currentProjectile}");
        _isCapturing = false;

        if (_targetFish != null)
        {
            if (success)
            {
                
                _targetFish.transform.SetParent(_currentProjectile.transform, true);
            }
            else
            {
                _targetFish.transform.SetParent(null, true);
                _targetFish.OnCaptureFailed();
            }
        }

        if (_currentProjectile != null)
        {
            // 맞았든 실패했든, 이제는 플레이어 쪽으로 돌아오게 한다
            _currentProjectile.BeginReturn();
        }

        _targetFish = null;
        _captureGauge = 0f;
        _captureTimer = 0f;

        // QTE UI 끄기
    }
    
    
    private void OnDisable()
    {
        Time.timeScale = 1f;
    }
}
