using System.Collections;
using Unity.Cinemachine;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 조준/발사/회수 컨트롤러
/// </summary>
[RequireComponent(typeof(DiverMoveController),typeof(InputController), typeof(HarpoonCaptureQTE))]
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

    [Header("반동 설정")]
    [SerializeField] private float _minRecoilStrength = 1.2f;
    [SerializeField] private float _maxRecoilStrength = 2.5f;
    
    [Tooltip("차지량(0~1)을 속도/데미지에 어떻게 반영할지 커브")]
    [SerializeField] private AnimationCurve _chargeCurve;
    
    [Header("조준 / 슬로우 모션")]
    [SerializeField] private float _aimTimeScale = 0.4f;       
    [SerializeField] private float _timeScaleLerpSpeed = 10f;
    
    [Header("UI")]
    [SerializeField] private GetItemUIUpdate _getUI;

    [Header("SFX 참조")]
    [SerializeField] private UnderwaterSFXManager _sfxManager;


    // 컴포넌트 / 참조
    private Animator _animator;
    private InputController _inputController;
    private DiverMoveController _moveController;
    private DiverStatus _diverStatus;
    private Camera _mainCam;
    private HarpoonCaptureQTE _captureQTE; // 포획 QTE
    private DiverVFXController _vfx;
    
    public bool IsCapturing => _captureQTE != null && _captureQTE.IsCapturing;
    public float CaptureGauge01 => _captureQTE != null ? _captureQTE.CaptureGauge01 : 0f;
    
    
    // 타이머
    private float _coolTimer;
    private float _chargeTimer;
    
    private HarpoonProjectile _currentProjectile;
    
    // 플래그 변수
    private bool _isAiming;
    private bool _isCharging;
    private bool _hasHarpoonOut;      
    private bool _canAim = true;       
    private bool _isHitStopping;
    
    // 프로퍼티
    public bool IsAiming => _isAiming;
    public bool IsCharging => _isCharging;
    public bool HasHarpoonOut => _hasHarpoonOut;
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

    public DiverVFXController VFX => _vfx;


    // 상수
    private const float MinShootDistance = 0.0001f;

    private void Awake()
    {
        Init();
    }

    private void Update()
    {
        _coolTimer += Time.unscaledDeltaTime; 
        
        if (_diverStatus.IsDead)
        {
            if (Time.timeScale != 1f)
                Time.timeScale = 1f;

            _isAiming = false;
            _isCharging = false;
            return;
        }
        
        // QTE 진행 중이면 여기서만 처리
        if (IsCapturing)
        {
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
        _diverStatus = GetComponent<DiverStatus>();
        _captureQTE = GetComponent<HarpoonCaptureQTE>();
        _vfx = GetComponent<DiverVFXController>();
        
        _mainCam = Camera.main;
        
        if (_chargeCurve == null || _chargeCurve.length == 0)
        {
            _chargeCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
        }
    }
    
    public void RequestHitStop(float duration, float scaledTime)
    {
        if (_isHitStopping) return;
        StartCoroutine(HitStopRoutine(duration, scaledTime));
    }

    private IEnumerator HitStopRoutine(float duration, float scaledTime)
    {
        _isHitStopping = true;

        float prev = Time.timeScale;
        Time.timeScale = scaledTime;
        
        yield return new WaitForSecondsRealtime(duration);

        Time.timeScale = prev;
        _isHitStopping = false;
    }
    
    private void UpdateAimState()
    {
        
        bool aimInput  = _inputController.IsAimButtonHeld;
        bool prevAiming = _isAiming;
        
        _isAiming = aimInput && !HasHarpoonOut;
        
        
        // 상태 전환 시 애니 트리거
        if (_isAiming)
        {
            _animator.SetTrigger("Aim");
            // TODO : 에임 시 주변 사운드 약하게, 줌인 sfx
        }
        else 
        {
            _animator.SetTrigger("AimEnd");
            // TODO : 볼륨 복구, 줌아웃 sfx
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
        if (_isHitStopping) return;
        
        float target = 1f;
        
        if (!HasHarpoonOut && _isAiming)
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
        if (HasHarpoonOut) return;
        
        bool chargePressed = _inputController.IsChargeButtonPressed;
        bool chargeHeld = _inputController.IsChargeButtonHeld;
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
            
            _sfxManager?.SetVolume(ESfx.Fire, Mathf.Max(charge * 0.02f, 0.05f));
            _sfxManager?.SetVolume(ESfx.Bubble, Mathf.Max(charge * 0.5f, 0.1f));
            
            _sfxManager?.Play(ESfx.Fire);
            _sfxManager?.Play(ESfx.Bubble);
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
        
        VFX?.PlayFireBubbleBurst(origin, dir, curved);
        
        // 반동 적용 
        float recoilStrength = Mathf.Lerp(_minRecoilStrength, _maxRecoilStrength, curved);
        _moveController.AddRecoil(-dir, recoilStrength);
        
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Quaternion rot = Quaternion.Euler(0f, 0f, angle);

        GameObject projObj = Instantiate(_harpoonPrefab, origin, rot);
        HarpoonProjectile proj = projObj.GetComponent<HarpoonProjectile>();
        proj.Launch(dir, speed, charge, this, _sfxManager);

        
        
        _currentProjectile = proj;
        _hasHarpoonOut = true;      
            
                
        
        // TODO:  발사 사운드 / 이펙트 호출
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
        IFishCapturable fish = proj.GetComponentInChildren<IFishCapturable>();
        if (fish != null)
        {
            _diverStatus.GainResource(fish.FishType);
            fish.Stored();
            Debug.Log(fish);  
        }
        
       
    }
    
    /// <summary>
    /// 투사체가 호출하여 QTE 시작
    /// </summary>
    /// <param name="fish"> hit 대상 </param>
    /// <param name="projectile"> 투사체 </param>
    public void StartCapture(IFishCapturable fish, HarpoonProjectile projectile)
    {
        _captureQTE.BeginCapture(fish, projectile);
        
        _currentProjectile = projectile;
        _hasHarpoonOut = true;
        
        _currentProjectile.AttachToFish(fish.Transform);
        
        _animator.SetBool("Pull", true);
    }
    
    /// <summary>
    /// QTE 종료 시 HarpoonCaptureQTE에서 호출
    /// </summary>
    /// <param name="proj"></param>
    /// <param name="fish"></param>
    /// <param name="success"></param>
    public void HandleCaptureResult(HarpoonProjectile proj, IFishCapturable fish, bool success)
    {
        
        proj.DetachFromFish();
        
        if (success)
        {
            fish.OnCapture();
            fish.Transform.SetParent(proj.transform, true);
        }
        else
        {
            fish.OnCaptureFailed();
        }
        
        
        proj.BeginReturn();
        
        _currentProjectile = null;
        _hasHarpoonOut = false;
        
        _animator.SetBool("Pull", false);
    }

    
    private void OnDisable()
    {
        Time.timeScale = 1f;
    }
}