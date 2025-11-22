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
    [SerializeField] private Vector2 _firePosOffset = new Vector2(0.5f, 0f); // 다이버 기준 발사 위치

    [Tooltip("차지량(0~1)을 속도/데미지에 어떻게 반영할지 커브")]
    [SerializeField] private AnimationCurve _chargeCurve;
    
    [Header("조준 / 슬로우 모션")]
    [SerializeField] private float _aimTimeScale = 0.4f;       
    [SerializeField] private float _timeScaleLerpSpeed = 10f; 
    
    // 컴포넌트
    Animator _animator;
    
    // 참조
    private InputController _inputController;
    private DiverMoveController _moveController;
    
    private Camera _mainCam;
    
    // 타이머
    private float _coolTimer;
    private float _chargeTimer;
    
    // 플래그 변수
    private bool _isAiming;
    private bool _isCharging;
    private bool _lockNormalTime;
    
    // 프로퍼티
    public bool IsAiming => _isAiming;
    public bool IsCharging => _isCharging;
    public float ChargeRatio
    {
        get
        {
            if (!_isCharging) return 0f;
            if (_maxChargeTime <= 0f) return 1f;
            return Mathf.Clamp01(_chargeTimer / _maxChargeTime);
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
        bool aimHeld = _inputController.IsAimButtonHeld;
        bool prevAiming = _isAiming;
        
        // 발사 이후에는 에임 버튼을 한 번 떼어야만 슬로모 재허용
        if (!aimHeld && _lockNormalTime)
        {
            _lockNormalTime = false;
        }

        _isAiming = aimHeld;
        
        // 상태 전환 시 애니 트리거
        if (_isAiming && !prevAiming)
        {
            _animator.SetTrigger("Aim");
        }
        else if (!_isAiming && prevAiming)
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
        float target;

        if (_lockNormalTime)
        {
            target = 1f; // 발사 이후, 에임 버튼을 뗄 때까지는 1배속 고정
        }
        else
        {
            target = _isAiming ? _aimTimeScale : 1f;
        }
        
        float newScale = Mathf.Lerp(Time.timeScale, target, _timeScaleLerpSpeed * Time.unscaledDeltaTime);
        Time.timeScale = newScale;
    }

    private void UpdateCharge()
    {
        if (!_isAiming) return;             
        if (_coolTimer < _fireCoolTime) return;

        // 차지 시작
        if (!_isCharging && _inputController.IsChargeButtonPressed)
        {
            _isCharging = true;
            _chargeTimer = 0;
            
        }
        
        // 차지 중
        if (_isCharging && _inputController.IsChageButtonHeld)
        {
            _chargeTimer += Time.unscaledDeltaTime;
            _chargeTimer = Mathf.Min(_chargeTimer, _maxChargeTime);
        }
        
        // 차지 끝, 발사
        if (_isCharging && _inputController.IsChargeButtonReleased)
        {
            Time.timeScale = 1f;
            _lockNormalTime = true;
            
            float charge = ChargeRatio;
            FireToMouse(charge);

            _isCharging = false;
            _chargeTimer = 0f;
            _coolTimer = 0f;
            
           
        }
    }

    private void FireToMouse(float charge)
    {
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
        proj.Launch(dir, speed, charge);

        _animator.SetTrigger("Shoot");
        // TODO:  카메라 셰이크 / 발사 사운드 / 이펙트 호출
    }

    private void OnDisable()
    {
        Time.timeScale = 1f;
    }
}
