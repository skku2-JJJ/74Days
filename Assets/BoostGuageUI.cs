using UnityEngine;
using UnityEngine.UI;

public class BoostGuageUI : MonoBehaviour
{
    [Header("플레이어")]
    [SerializeField] private Transform _player;
    
    [Header("페이드 설정")]
    [SerializeField] private float _fadeSpeed = 10f; 

    private Slider _slider;
    private CanvasGroup _canvasGroup;
    private RectTransform _rectTransform;
    private Camera _camera;

    private DiverMoveController _moveController;
    private DiverStatus _diverStatus;

    private void Awake()
    {
        Init();
    }

    private void LateUpdate()
    {
        if (_player == null ||  _slider == null)  return;
           

        if (_diverStatus.IsDead)
        {
            _canvasGroup.alpha = 0f;
            return;
        }
        
        UpdateGaugeAndVisibility();
    }

    private void Init()
    {
        if (_player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            _player = playerObj.transform;
        }

        _slider       = GetComponent<Slider>();
        _canvasGroup  = GetComponent<CanvasGroup>();
        _rectTransform = GetComponent<RectTransform>();
        _camera = Camera.main;

        _moveController = _player.GetComponent<DiverMoveController>();
        _diverStatus    = _player.GetComponent<DiverStatus>();
        
    }
    

    private void UpdateGaugeAndVisibility()
    {
        bool isBoosting = _moveController.IsBoosting;
        float coolRatio = _moveController.BoostCooldownRatio;
        
        bool shouldShow = isBoosting || (coolRatio > 0f && coolRatio < 1f);

        float targetAlpha = shouldShow ? 1f : 0f;
        _canvasGroup.alpha = Mathf.Lerp(
            _canvasGroup.alpha,
            targetAlpha,
            _fadeSpeed * Time.unscaledDeltaTime
        );
        
        if (isBoosting)
        {
            _slider.value = _moveController.BoostRemainRatio;
        }
        else
        {
            _slider.value = coolRatio;
        }
    }
}
