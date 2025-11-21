using UnityEngine;

/// <summary>
/// 조준/발사 컨트롤러
/// </summary>
[RequireComponent(typeof(DiverMoveController))]
public class HarpoonShooter : MonoBehaviour
{
    [Header("참조")]
    [SerializeField] private InputController _inputController;
    [SerializeField] private DiverMoveController _moveController;
    //[SerializeField] private HarpoonProjectile _harpoonPrefab;

    [Header("발사 설정")]
    [SerializeField] private float _harpoonSpeed = 12f;
    [SerializeField] private float _fireCoolTime = 0.4f;
    [SerializeField] private Vector2 _fireOffset = new Vector2(0.5f, 0f); // 다이버 기준 발사 위치

    [Header("조준 / 슬로우 모션")]
    [SerializeField] private float _aimTimeScale = 0.4f;       // 조준 중 타임스케일
    [SerializeField] private float _timeScaleLerpSpeed = 10f;  // 타임스케일 보간 속도

    private Camera _mainCam;
    private float _coolTimer;
    private bool _isAiming;

    private void Awake()
    {
        if (_moveController == null)
            _moveController = GetComponent<DiverMoveController>();

        _mainCam = Camera.main;
    }

    private void Update()
    {
        _coolTimer += Time.unscaledDeltaTime; 

        UpdateAimState();
        UpdateTimeScale();
        HandleFire();
    }

    private void UpdateAimState()
    {
        bool aimHeld = Input.GetMouseButton(1);

        _isAiming = aimHeld;
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

        if (Input.GetMouseButtonDown(0)) 
        {
            FireToMouse();
            _coolTimer = 0f;
        }
    }

    private void FireToMouse()
    {
        // 마우스를 향해 쏘기
        Vector3 mouseWorld = _mainCam.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = 0;

        Vector2 origin = (Vector2)transform.position + _fireOffset;
        Vector2 dir = (mouseWorld - (Vector3)origin);
        if (dir.sqrMagnitude < 0.0001f) return;

        dir.Normalize();

        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        Quaternion rot = Quaternion.Euler(0f, 0f, angle);

        //HarpoonProjectile proj = Instantiate(_harpoonPrefab, origin, rot);
        //proj.Launch(dir, _harpoonSpeed);

        // TODO:  카메라 셰이크 / 발사 사운드 / 이펙트 호출
    }

    private void OnDisable()
    {
        Time.timeScale = 1f;
    }
}
