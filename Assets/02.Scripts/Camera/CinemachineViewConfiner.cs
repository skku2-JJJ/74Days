using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.Tilemaps;


/// <summary>
/// 시네머신 카메라 extension - 화면 전체가 타일 맵 밖으로 나가지 않도록 Clamp
/// </summary>
[ExecuteAlways]
[RequireComponent(typeof(CinemachineCamera))]
public class CinemachineViewConfiner : CinemachineExtension
{
    [Header("기준 타일맵")]
    [SerializeField] private Tilemap _tilemap;

    [Header("여유 패딩")]
    [SerializeField] private float _paddingX = 0f;
    [SerializeField] private float _paddingY = 0f;

    private bool _hasBounds;
    private Bounds _worldBounds;

    protected override void Awake()
    {
        base.Awake();
        CacheBounds();
    }

    private void OnValidate()
    {
        CacheBounds();
    }

    private void CacheBounds()
    {
        _hasBounds = false;
        if (_tilemap == null) return;

        // 타일맵 로컬 bounds를 월드 기준으로 변환
        var local = _tilemap.localBounds;

        Vector3 worldMin = _tilemap.transform.TransformPoint(local.min);
        Vector3 worldMax = _tilemap.transform.TransformPoint(local.max);

        _worldBounds = new Bounds();
        _worldBounds.SetMinMax(worldMin, worldMax);

        _hasBounds = true;
    }

    /// <summary>
    /// Cinemachine 파이프라인 마지막 단계(Body)에서 카메라 위치를 클램프한다.
    /// </summary>
    protected override void PostPipelineStageCallback(
        CinemachineVirtualCameraBase vcam,
        CinemachineCore.Stage stage,
        ref CameraState state,
        float deltaTime)
    {
        if (!_hasBounds || _tilemap == null)  return;
        
        if (stage != CinemachineCore.Stage.Finalize) return;

            
        Camera cam = Camera.main;
        if (cam == null || !cam.orthographic) return;
       
        
        // 뷰 크기는 PixelPerfect가 세팅한 메인 카메라 기준
        float halfHeight = cam.orthographicSize;
        float halfWidth  = halfHeight * cam.aspect;
        
        halfWidth  += _paddingX;
        halfHeight += _paddingY;
        
        Vector3 pos = state.RawPosition; 
        
        Vector3 min = _worldBounds.min;
        Vector3 max = _worldBounds.max;
        
        float minX = min.x + halfWidth;
        float maxX = max.x - halfWidth;
        float minY = min.y + halfHeight;
        float maxY = max.y - halfHeight;

        /*// 만약 맵이 카메라보다 작아서 min > max 되는 경우 방어
        if (minX > maxX)
            minX = maxX = (min.x + max.x) * 0.5f;

        if (minY > maxY)
            minY = maxY = (min.y + max.y) * 0.5f;*/

        pos.x = Mathf.Clamp(pos.x, minX, maxX);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);
        
        state.RawPosition = pos;
    }
}
