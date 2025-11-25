using Unity.Cinemachine;
using UnityEngine;

public class HarpoonAimCameraFraming : MonoBehaviour
{
    [Header("참조")]
    [SerializeField] private HarpoonShooter _shooter; 
    private Transform _diverTransform; 
    private Camera _camera; 
    
    [Header("프레이밍 설정")]
    [Tooltip("카메라가 플레이어에서 최대 얼마나 떨어져서 프레이밍 될지(월드 단위)")]
    [SerializeField] private float _maxOffsetDistance = 2.5f;

    [Tooltip("오프셋 변경 부드러움 (값이 작을수록 더 빠르게 따라감)")]
    [SerializeField] private float _smoothTime = 0.15f;
    
    
    private CinemachineCamera _cinemachineCamera;
    private CinemachinePositionComposer _composer;

    private Vector3 _baseOffset;
    private Vector3 _currentOffset;
    private Vector3 _offsetVelocity;

    private const float DirectionEpsilonSq = 0.0001f;

    private void Awake()
    {
        Init();  
    }

    private void Init()
    {
        _cinemachineCamera = GetComponent<CinemachineCamera>();
        _composer = _cinemachineCamera.GetComponent<CinemachinePositionComposer>();
        
        _camera = Camera.main;
        _diverTransform = _shooter.transform;
        
        _baseOffset = _composer.TargetOffset;
        _currentOffset = _baseOffset;
    }
    private void LateUpdate()
    {
        AimFraming();
    }
    
    private void AimFraming()
    {
        Vector3 desiredOffset = _baseOffset;

        bool useAimFraming =  _shooter.IsAiming;

        if (useAimFraming)
        {
            Vector3 mouseScreen = Input.mousePosition;
            Vector3 mouseWorld = _camera.ScreenToWorldPoint(mouseScreen);
            mouseWorld.z = 0f;
            
            Vector3 toMouse = mouseWorld - _diverTransform.position;
            toMouse.z = 0f;

            if (toMouse.sqrMagnitude > DirectionEpsilonSq)
            {
                Vector3 dir = toMouse.normalized;
                Vector3 aimOffset = dir * _maxOffsetDistance;

                desiredOffset = _baseOffset + aimOffset;
            }
        }
        
        _currentOffset = Vector3.SmoothDamp(
            _currentOffset,
            desiredOffset,
            ref _offsetVelocity,
            _smoothTime
        );

        _composer.TargetOffset = _currentOffset;
    }
}
