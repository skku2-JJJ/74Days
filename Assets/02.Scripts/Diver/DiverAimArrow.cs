using UnityEngine;

/// <summary>
/// 조준 화살표
///  - 다이버 중심으로 공전
/// </summary>
public class DiverAimArrow : MonoBehaviour
{
    [Header("위치 / 회전")]
    [SerializeField] private float _radius = 2f; 
    [SerializeField] private float _spriteForwardOffsetDegrees = 0f;
    
    [Header("차지 색")]
    [SerializeField] private Color _colorStart = Color.white;        // 0%           
    [SerializeField] private Color _colorMid = new Color(1f, 0.9f, 0.2f, 1f);   // 50%
    [SerializeField] private Color _colorEnd = new Color(0.2f, 1f, 1f, 1f);     // 100%
    
    
    [Header("스케일 / 펄스")]
    [SerializeField] private float _baseScale = 1f;               // 차지 0 기준 스케일
    [SerializeField] private float _maxScaleAtFullCharge = 1.3f;  // 풀차지 시 기본 스케일 배율
    [SerializeField] private float _pulseAmplitude = 0.08f;       // 펄스 폭
    [SerializeField] private float _pulseSpeed = 6f;   
    
    [Header("등장 / 퇴장 연출")]
    [SerializeField] private float _alphaLerpSpeed = 12f;    
    
    // 플래그 변수
    private bool _hideWhenNotAiming = true;

    // 참조
    private DiverStatus _diverStatus;
    private HarpoonShooter _harpoonShooter;
    private Transform _diverTransform;
    private Camera _mainCam;
    
    // 컴포넌트
    private SpriteRenderer _renderer;

    private float _currentAlpha = 0f;
    
    private void Awake()
    {
        Init();
    }
    
    

    private void LateUpdate()
    {
        if (_diverStatus.IsDead)
        {
            _renderer.enabled = false;
            return;
        }
        
        UpdateArrowRenderer();
        UpdateArrowTransform();
        UpdateArrowVisuals();
    }

    private void Init()
    {
        _mainCam = Camera.main;
        _renderer = GetComponent<SpriteRenderer>();
        
        _harpoonShooter = GetComponentInParent<HarpoonShooter>();
        _diverStatus = GetComponentInParent<DiverStatus>(); 
        
        _diverTransform = _harpoonShooter.transform;
    }

    private void UpdateArrowRenderer()
    {
        bool isAiming = _harpoonShooter.IsAiming;
        float targetAlpha = isAiming ? 1f : 0f;

        // 알파값 보간
        _currentAlpha = Mathf.Lerp(_currentAlpha, targetAlpha, _alphaLerpSpeed * Time.unscaledDeltaTime);

        if (_hideWhenNotAiming && _currentAlpha < 0.01f)
        {
            if (_renderer.enabled)
                _renderer.enabled = false;
            return;
        }
        else
        {
            if (!_renderer.enabled)
                _renderer.enabled = true;
        }
    }
    private void UpdateArrowTransform()
    {
       
        Vector3 mouseWorld = _mainCam.ScreenToWorldPoint(Input.mousePosition);
        mouseWorld.z = _diverTransform.position.z;

     
        Vector2 dir = (mouseWorld - _diverTransform.position);
        if (dir.sqrMagnitude < 0.0001f)
            return;

        dir.Normalize();

        
        Vector3 worldPos = _diverTransform.position + (Vector3)(dir * _radius);
        transform.position = worldPos;

        
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, angle + _spriteForwardOffsetDegrees);
    }

    private void UpdateArrowVisuals()
    {
        float charge = _harpoonShooter.ChargeRatio; 

       
        Color targetColor;
        if (charge <= 0.5f)
        {
            float t = charge / 0.5f; // 0~0.5 → 0~1
            targetColor = Color.Lerp(_colorStart, _colorMid, t);
        }
        else
        {
            float t = (charge - 0.5f) / 0.5f; // 0.5~1 → 0~1
            targetColor = Color.Lerp(_colorMid, _colorEnd, t);
        }

        // 등장/퇴장 알파 적용
        targetColor.a *= _currentAlpha;
        _renderer.color = targetColor;

        // --- 스케일 (차지 + 펄스) ---
        float baseScale = Mathf.Lerp(_baseScale, _maxScaleAtFullCharge, charge);
        float pulse = Mathf.Sin(Time.unscaledTime * _pulseSpeed) * _pulseAmplitude;
        float finalScale = baseScale * (1f + pulse);

        transform.localScale = Vector3.one * finalScale;
    }
    
}
