using Unity.Cinemachine;
using UnityEngine;


/// <summary>
/// PixelPerfectCamera에 의한 MainCamera의 orthographicSize 동기화
/// </summary>
[ExecuteAlways]
[RequireComponent(typeof(CinemachineCamera))]
public class SyncOrthoWithMain : MonoBehaviour
{
    private Camera _mainCam; 
    private CinemachineCamera _cmCamera;

    private void Awake()
    {
        _cmCamera = GetComponent<CinemachineCamera>();
        _mainCam = Camera.main;
    }

    private void LateUpdate()
    {
        if (!Application.isPlaying) return;
        if (_cmCamera == null || _mainCam == null) return;
        if (!_mainCam.orthographic) return;
        
        float orthoSize = _mainCam.orthographicSize;
        
        var lens = _cmCamera.Lens;
        if (!Mathf.Approximately(lens.OrthographicSize, orthoSize))
        {
            lens.OrthographicSize = orthoSize;
            _cmCamera.Lens = lens;
        }
    }
}
