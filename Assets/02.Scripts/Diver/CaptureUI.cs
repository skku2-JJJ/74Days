using UnityEngine;
using UnityEngine.UI;

public class CaptureUI : MonoBehaviour
{
    [Header("플레이어")]
    [SerializeField] private Transform _player;
    
    [Header("위치 설정 (Screen Space 기준 오프셋)")]
    [SerializeField] private Vector2 _screenOffset = new Vector2(120f, 40f); // x, y 픽셀 단위
    
    // 참조
    private Slider _gaugeSlider;
    private HarpoonShooter _shooter;
    private DiverStatus _diverStatus;
    private SpriteRenderer _diverSprite;
    private CanvasGroup _canvasGroup; 
    private RectTransform _rectTransform;
    private Camera _camera;
    
    private void Awake()
    {
        Init();
    }

    private void Update()
    {
        if (_shooter == null || _gaugeSlider == null) return;
        if ( _diverStatus.IsDead)
        {
            _canvasGroup.alpha = 0f;
            return;
        }
        
        if (!_shooter.IsCapturing)
        {
            _canvasGroup.alpha = 0f;
            return;
        }
        
        _canvasGroup.alpha = 1f;
       
        UpdatePosition();
        UpdateGauge();
    }

    private void Init()
    {
        if (_player == null)
        {
            _player = GameObject.FindGameObjectWithTag("Player").transform;
            if (_player != null)
            {
                _player = _player.transform;
            }
            else
            {
                Debug.LogError("'Player' 태그를 가진 오브젝트를 찾을 수 없습니다.", this);
                enabled = false;
                return;
            }
        }
           
        
        _canvasGroup = GetComponent<CanvasGroup>();
        _gaugeSlider = GetComponent<Slider>();
        
        _shooter = _player.GetComponent<HarpoonShooter>();
        _diverSprite = _player.GetComponentInChildren<SpriteRenderer>();
        _diverStatus = _player.GetComponent<DiverStatus>();
        
        _rectTransform = GetComponent<RectTransform>();
        _camera = Camera.main;
    }
    private void UpdatePosition()
    {
     
        Vector3 screenPos = _camera.WorldToScreenPoint(_player.position);
        
        bool facingRight = true;

        if (_diverSprite != null)
        {
            facingRight = !_diverSprite.flipX;
        }
        
        float dir = facingRight ? -1f : 1f;
        
        screenPos.x += dir * _screenOffset.x;
        screenPos.y += _screenOffset.y;

        _rectTransform.position = screenPos;
    }

    private void UpdateGauge()
    {
        if (_gaugeSlider == null) return;
        
        _gaugeSlider.value = _shooter.CaptureGauge01;
    }
}
