using UnityEngine;
using UnityEngine.UI;

public class HarpoonAimUI : MonoBehaviour
{
    [Header("참조")]
    [SerializeField] private Transform _diverTransform;
    [SerializeField] private RectTransform _rectTransform;
    
    
    [Header("표시 설정")]
    [SerializeField] private float _distanceFromPlayer = 80f; // 플레이어 기준 픽셀 거리
    [SerializeField] private bool _hideWhenNotAiming = true;

    [Header("차지 색")]
    [SerializeField] private Color _colorStart = Color.white;                      // 0%
    [SerializeField] private Color _colorMid   = new Color(1f, 0.9f, 0.2f, 1f);    // 50%
    [SerializeField] private Color _colorEnd   = new Color(0.2f, 1f, 1f, 1f);      // 100%

    [Header("알파 / 등장 퇴장")]
    [Range(0f, 1f)] [SerializeField] private float _alphaWhenHidden = 0f;
    [Range(0f, 1f)] [SerializeField] private float _alphaWhenAiming = 1f;
    [SerializeField] private float _showHideLerpSpeed = 10f;   // 알파 보간 속도

    [Header("스케일 / 펄스")]
    [SerializeField] private float _baseScale = 1f;            // 미조준 / 0% 차지 기준 스케일
    [SerializeField] private float _maxScaleAtFullCharge = 1.3f;
    [SerializeField] private float _pulseSpeed = 4f;
    [SerializeField] private float _pulseAmplitude = 0.05f;
    
    private HarpoonShooter _harpoonShooter;
    private Camera _mainCam;
    private Image _image;

    private float _currentAlpha;
    
    private DiverMoveController _moveController;
    
    private Canvas _canvas;
    

    private void Awake()
    {
       Init();
    }

    private void Update()
    {
        if (_diverTransform == null || _rectTransform == null || _canvas == null || _image == null)
            return;
        if (_mainCam == null)
            _mainCam = Camera.main;
        if (_mainCam == null)
            return;

        // 1) 조준 상태 / 차지값 가져오기
        bool isAiming   = _harpoonShooter != null && _harpoonShooter.IsAiming;
        bool isCharging = _harpoonShooter != null && _harpoonShooter.IsCharging;

        float charge = 0f;
        if (_harpoonShooter != null)
        {
            // HarpoonShooter에 이미 ChargeRatio 프로퍼티 있는 구조 기준
            charge = Mathf.Clamp01(_harpoonShooter.ChargeRatio);
        }

        // 2) 알파(등장/퇴장) 업데이트
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

        // 3) 색상(차지에 따라 그라데이션)
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

        // 4) 스케일 (차지 + 펄스)
        float baseScale = Mathf.Lerp(_baseScale, _maxScaleAtFullCharge, charge);
        float pulse     = Mathf.Sin(Time.unscaledTime * _pulseSpeed) * _pulseAmplitude;
        float finalScale = baseScale * (1f + pulse);

        _rectTransform.localScale = Vector3.one * finalScale;

        // 5) 방향 계산 (마우스 기준)
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

        // 6) 다이버 월드 → 스크린 → UI 위치로 변환
        Vector3 diverScreenPos = _mainCam.WorldToScreenPoint(diverWorldPos);
        Vector2 uiScreenPos = (Vector2)diverScreenPos + dir * _distanceFromPlayer;

        RectTransform canvasRect = _canvas.transform as RectTransform;
        if (canvasRect == null)
            return;

        Vector2 anchoredPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            uiScreenPos,
            _canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : _mainCam,
            out anchoredPos
        );

        _rectTransform.anchoredPosition = anchoredPos;

        // 7) 방향에 맞게 회전
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        _rectTransform.localRotation = Quaternion.Euler(0f, 0f, angle);
    }

    private void Init()
    {
        _rectTransform = GetComponent<RectTransform>();
        _canvas = GetComponentInParent<Canvas>();
       
        
        _moveController = _diverTransform.GetComponent<DiverMoveController>();
        _harpoonShooter = _diverTransform.GetComponent<HarpoonShooter>();
        
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
    
    private void RotateHarpoonAimUI()
    {
        // 1) 다이버 월드 → 스크린 좌표
        Vector3 diverWorld = _moveController.transform.position;
        Vector3 diverScreen = _mainCam.WorldToScreenPoint(diverWorld);

        // 2) 마우스 스크린 좌표
        Vector3 mouseScreen = Input.mousePosition;

        Vector2 dir = (mouseScreen - diverScreen);
        
        if (dir.sqrMagnitude < 0.001f)
            return;

        dir.Normalize();

        // 3) 다이버 기준으로 일정 거리 떨어진 지점에 UI 배치
        Vector2 uiScreenPos = (Vector2)diverScreen + dir * _distanceFromPlayer;

        // 4) 스크린 좌표 → 캔버스 로컬 좌표
        RectTransform canvasRect = _canvas.transform as RectTransform;
        Vector2 anchoredPos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            uiScreenPos,
            _canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : _mainCam,
            out anchoredPos
        );

        _rectTransform.anchoredPosition = anchoredPos;

        // 5) 방향에 맞게 회전
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        _rectTransform.localRotation = Quaternion.Euler(0f, 0f, angle);
    }
}
