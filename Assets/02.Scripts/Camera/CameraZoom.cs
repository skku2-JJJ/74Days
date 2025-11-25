using DG.Tweening;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Rendering.Universal;



public class CameraZoom : MonoBehaviour
{
    [SerializeField] private HarpoonShooter _shooter;

    [Header("Zoom Settings")]
    [SerializeField] private float _zoomFactor = 0.65f;
    [SerializeField] private float _zoomTime = 0.25f;
    [SerializeField] private Ease _zoomEase = Ease.OutQuad;

    private PixelPerfectCamera _pixelPerfectCamera;
    private int _baseAssetsPPU;
    private int _currentTargetPPU;
    private Tweener _zoomTween;

    private void Awake()
    {
        // PixelPerfectCamera는 MainCamera에 붙어있으므로 Camera.main에서 가져옴
        _pixelPerfectCamera = Camera.main?.GetComponent<PixelPerfectCamera>();

        if (_pixelPerfectCamera == null)
        {
            Debug.LogError("[CameraZoom] PixelPerfectCamera를 찾을 수 없습니다. MainCamera에 PixelPerfectCamera가 있는지 확인하세요.");
            enabled = false;
            return;
        }

        _baseAssetsPPU = _pixelPerfectCamera.assetsPPU;
        _currentTargetPPU = _baseAssetsPPU;
    }

    private void Update()
    {
        if (_shooter == null || _pixelPerfectCamera == null)
            return;

        bool zoomActive = _shooter.IsAiming;

        // assetsPPU를 조정하여 픽셀 퍼펙트 유지하면서 줌
        // 줌인: PPU 증가 (더 많은 픽셀을 보여줌)
        // 예: zoomFactor=0.65 → targetPPU = 16/0.65 ≈ 24.6
        int targetPPU = zoomActive
            ? Mathf.RoundToInt(_baseAssetsPPU / _zoomFactor)
            : _baseAssetsPPU;

        if (targetPPU == _currentTargetPPU)
            return;

        _currentTargetPPU = targetPPU;

        // 이전 트윈 정리
        _zoomTween?.Kill();

        // DOTween으로 assetsPPU 트윈 (부드러운 줌 효과)
        _zoomTween = DOTween.To(
                () => _pixelPerfectCamera.assetsPPU,
                value => _pixelPerfectCamera.assetsPPU = value,
                targetPPU,
                _zoomTime
            )
            .SetEase(_zoomEase)
            .SetUpdate(true); // unscaled time (타임슬로우에 영향 안 받게)
    }
}
