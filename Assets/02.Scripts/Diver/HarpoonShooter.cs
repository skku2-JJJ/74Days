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
    [SerializeField] private float _harpoonSpeed = 12f;
    [SerializeField] private float _fireCoolTime = 0.4f;
    [SerializeField] private Vector2 _fireOffset = new Vector2(0.5f, 0f); // 다이버 기준 발사 위치

    [Header("조준 / 슬로우 모션")]
    [SerializeField] private float _aimTimeScale = 0.4f;       
    [SerializeField] private float _timeScaleLerpSpeed = 10f; 

    // 컴포넌트
    Animator _animator;
    
    // 참조
    private InputController _inputController;
    private DiverMoveController _moveController;
    
    private Camera _mainCam;
    private float _coolTimer;
    private bool _isAiming;
    
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
        HandleFire();
    }

    private void Init()
    {
        _animator =  GetComponentInChildren<Animator>();
        _inputController = GetComponent<InputController>();
        _moveController = GetComponent<DiverMoveController>();
        
        _mainCam = Camera.main;
    }
    private void UpdateAimState()
    {
        _isAiming = _inputController.IsAimButtonHeld;

        if (_isAiming)
        {
            _animator.SetTrigger("Aim");
        }
        else
        {
            _animator.SetTrigger("AimEnd");
        }
    }

    private void UpdateTimeScale()
    {
        float target = _isAiming ? _aimTimeScale : 1f;
        float newScale = Mathf.Lerp(Time.timeScale, target, _timeScaleLerpSpeed * Time.unscaledDeltaTime);
        Time.timeScale = newScale;
    }

    private void HandleFire()
    {
        if (!_isAiming) return;             
        if (_coolTimer < _fireCoolTime) return;

        if (_inputController.IsShootButtonPressed) 
        {
            Time.timeScale = 1f;
            
            FireToMouse();
            _coolTimer = 0f;
        }
    }

    private void FireToMouse()
    {
        Vector3 mouseWorld = _mainCam.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0;

        Vector2 origin = (Vector2)transform.position + _fireOffset;
        Vector2 dir = (mouseWorld - (Vector3)origin);
        if (dir.sqrMagnitude < MinShootDistance) return;

        dir.Normalize();

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Quaternion rot = Quaternion.Euler(0f, 0f, angle);

        GameObject projObj = Instantiate(_harpoonPrefab, origin, rot);
        HarpoonProjectile proj = projObj.GetComponent<HarpoonProjectile>();
        proj.Launch(dir, _harpoonSpeed);

        _animator.SetTrigger("Shoot");
        // TODO:  카메라 셰이크 / 발사 사운드 / 이펙트 호출
    }

    private void OnDisable()
    {
        Time.timeScale = 1f;
    }
}
