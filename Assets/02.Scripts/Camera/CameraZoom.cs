using DG.Tweening;
using Unity.Cinemachine;
using UnityEngine;



public class CameraZoom : MonoBehaviour
{
    [SerializeField] private HarpoonShooter _shooter;

    [Header("Zoom Settings")]
    [SerializeField] private float _zoomFactor = 0.65f;     
    [SerializeField] private float _zoomTime = 0.25f;       
    [SerializeField] private Ease _zoomEase = Ease.OutQuad; 
    
    private CinemachineCamera _cinemachineCamera;
    private float _baseSize;
    private float _currentTargetSize;
    private Tweener _zoomTween;

    private void Awake()
    {
        _cinemachineCamera = GetComponent<CinemachineCamera>();
        
        _baseSize = _cinemachineCamera.Lens.OrthographicSize;
        _currentTargetSize = _baseSize;
    }

    private void Update()
    {
        if (_shooter == null)
            return;

        bool zoomActive = _shooter.IsAiming;
        
        float targetSize = zoomActive ? (_baseSize * _zoomFactor) : _baseSize;
        
        if (Mathf.Abs(targetSize - _currentTargetSize) < 0.01f)
            return;

        _currentTargetSize = targetSize;

        // 이전 트윈 정리
        _zoomTween?.Kill();

        // DOTween으로 vcam Lens.OrthographicSize 트윈
        _zoomTween = DOTween.To(
                () => _cinemachineCamera.Lens.OrthographicSize,
                value =>
                {
                    var lens = _cinemachineCamera.Lens;
                    lens.OrthographicSize = value;
                    _cinemachineCamera.Lens = lens;
                },
                targetSize,
                _zoomTime
            )
            .SetEase(_zoomEase)
            .SetUpdate(true); // unscaled time (타임슬로우에 영향 안 받게)
    }
}
