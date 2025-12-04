using UnityEngine;
using UnityEngine.UI;

public class HarpoonAimUI : MonoBehaviour
{
    [Header("참조")]
    [SerializeField] private Transform _diverTransform;
    [SerializeField] private RectTransform _rectTransform;
    
    
    [Header("표시 설정")]
    [SerializeField] private float _distanceFromPlayer = 80f; 
    [SerializeField] private float _spriteForwardOffsetDegrees = 0f;
    
    [Header("차지 색")]
    [SerializeField] private Color _colorStart = Color.white;                      
    [SerializeField] private Color _colorMid   = new Color(1f, 0.9f, 0.2f, 1f);    
    [SerializeField] private Color _colorEnd   = new Color(0.2f, 1f, 1f, 1f);      

    [Header("알파 / 등장 퇴장")]
    [Range(0f, 1f)] [SerializeField] private float _alphaWhenHidden = 0f;
    [Range(0f, 1f)] [SerializeField] private float _alphaWhenAiming = 1f;
    [SerializeField] private float _showHideLerpSpeed = 10f;   

    [Header("스케일 / 펄스")]
    [SerializeField] private float _baseScale = 1f;            
    [SerializeField] private float _maxScaleAtFullCharge = 1.3f;
    [SerializeField] private float _pulseSpeed = 4f;
    [SerializeField] private float _pulseAmplitude = 0.05f;
    
    private HarpoonShooter _harpoonShooter;
    private Camera _mainCam;
    private Image _image;

    private float _currentAlpha;
    private bool _hideWhenNotAiming = true;
    
    
    private DiverStatus _diverStatus;
    private Canvas _canvas;
    

    private void Awake()
    {
       Init();
    }

    private void LateUpdate()
    {
        if (_diverStatus.IsDead)
        {
            _image.enabled = false;
            return;
        }
        
        UpdateArrowColor();
        UpdateArrowScale();
        UpdateArrowTransform();
    }

    private void Init()
    {
        _rectTransform = GetComponent<RectTransform>();
        _canvas = GetComponentInParent<Canvas>();
        
        _harpoonShooter = _diverTransform.GetComponent<HarpoonShooter>();
        _diverStatus = _diverTransform.GetComponent<DiverStatus>();
        _mainCam = Camera.main;
        
        _image = GetComponent<Image>();
        if (_image != null)
        {
            _currentAlpha = 0f;
            var c = _image.color;
            c.a = 0f;
            _image.color = c;
        }
        
    }

    private void UpdateArrowColor()
    {
        bool isAiming   = _harpoonShooter.IsAiming;
        bool isCharging = _harpoonShooter.IsCharging;

        float charge = Mathf.Clamp01(_harpoonShooter.ChargeRatio);
        float targetAlpha = _alphaWhenAiming;

        if (_hideWhenNotAiming && !isAiming && !isCharging)
        {
            targetAlpha = _alphaWhenHidden;
        }

        _currentAlpha = Mathf.Lerp(
            _currentAlpha,
            targetAlpha,
            Time.unscaledDeltaTime * _showHideLerpSpeed
        );
        
        Color targetColor;

        if (charge < 0.5f)
        {
            float t = charge / 0.5f;
            targetColor = Color.Lerp(_colorStart, _colorMid, t);
        }
        else
        {
            float t = (charge - 0.5f) / 0.5f;
            targetColor = Color.Lerp(_colorMid, _colorEnd, t);
        }

        targetColor.a *= _currentAlpha;
        _image.color = targetColor;

    }
    private void UpdateArrowScale()
    {
        float charge = Mathf.Clamp01(_harpoonShooter.ChargeRatio);
        
        float baseScale = Mathf.Lerp(_baseScale, _maxScaleAtFullCharge, charge);
        float pulse     = Mathf.Sin(Time.unscaledTime * _pulseSpeed) * _pulseAmplitude;
        float finalScale = baseScale * (1f + pulse);

        _rectTransform.localScale = Vector3.one * finalScale;
    }

    private void UpdateArrowTransform()
    {
        Vector3 diverWorldPos = _diverTransform.position;

        Vector3 mouseScreenPos = Input.mousePosition;
        Vector3 mouseWorldPos  = _mainCam.ScreenToWorldPoint(mouseScreenPos);

        Vector2 dir = (Vector2)(mouseWorldPos - diverWorldPos);
        if (dir.sqrMagnitude < 0.0001f)
        {
            dir = Vector2.right;
        }
        else
        {
            dir.Normalize();
        }
        
        Vector3 diverScreenPos = _mainCam.WorldToScreenPoint(diverWorldPos);
        Vector2 uiScreenPos = (Vector2)diverScreenPos + dir * _distanceFromPlayer;

        RectTransform canvasRect = _canvas.transform as RectTransform;
        if (canvasRect == null)   return;
          

        Vector2 anchoredPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            uiScreenPos,
            _canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : _mainCam,
            out anchoredPos
        );

        _rectTransform.anchoredPosition = anchoredPos;
        
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        _rectTransform.localRotation = Quaternion.Euler(0f, 0f, angle + _spriteForwardOffsetDegrees);
    }
}
