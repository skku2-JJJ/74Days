using UnityEngine;
using UnityEngine.Rendering;

/// <summary>
/// 플레이어의 수심(Y축 위치)에 따라 Post-Processing Volume의 가중치를 동적으로 조절
/// 얕은 곳 → 중간 → 깊은 곳으로 갈수록 다른 시각 효과 적용
/// </summary>
public class DepthBasedPostProcessing : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform _playerTransform; // 플레이어 Transform

    [Header("Volume References")]
    [SerializeField] private Volume _surfaceVolume;   // 수면 근처 (Y > -10)
    [SerializeField] private Volume _shallowVolume;   // 얕은 수심 (-10 ~ -40)
    [SerializeField] private Volume _mediumVolume;    // 중간 수심 (-40 ~ -100)
    [SerializeField] private Volume _deepVolume;      // 깊은 수심 (-100 이하)

    [Header("Depth Thresholds")]
    [SerializeField] private float _surfaceDepth = -10f;   // 수면 기준
    [SerializeField] private float _shallowDepth = -40f;   // 얕은 곳 기준
    [SerializeField] private float _mediumDepth = -100f;   // 중간 기준
    [SerializeField] private float _blendRange = 5f;       // 전환 구간 범위

    [Header("Settings")]
    [SerializeField] private float _transitionSpeed = 2f; // 전환 속도

    private float _targetSurfaceWeight = 1f;
    private float _targetShallowWeight = 0f;
    private float _targetMediumWeight = 0f;
    private float _targetDeepWeight = 0f;

    void Start()
    {
        // 플레이어가 지정되지 않았으면 자동으로 찾기
        if (_playerTransform == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null)
            {
                _playerTransform = player.transform;
            }
            else
            {
                Debug.LogWarning("[DepthBasedPostProcessing] Player를 찾을 수 없습니다. Tag가 'Player'인지 확인하세요.");
            }
        }

        // Volume이 할당되지 않았으면 경고
        if (_surfaceVolume == null || _shallowVolume == null || _mediumVolume == null || _deepVolume == null)
        {
            Debug.LogError("[DepthBasedPostProcessing] Volume이 할당되지 않았습니다. Inspector에서 설정하세요.");
        }

        // 초기 가중치 설정
        UpdateVolumeWeights(true);
    }

    void Update()
    {
        if (_playerTransform == null) return;

        UpdateVolumeWeights(false);
    }

    /// <summary>
    /// 플레이어 깊이에 따라 Volume 가중치 계산 및 적용
    /// Y > -10: Surface (밝음)
    /// Y -10 ~ -40: Shallow (청록색)
    /// Y -40 ~ -100: Medium (파란색)
    /// Y < -100: Deep (어두운 남색)
    /// </summary>
    private void UpdateVolumeWeights(bool instant)
    {
        float playerDepth = _playerTransform.position.y;

        // 깊이별 가중치 계산
        if (playerDepth > _surfaceDepth)
        {
            // 수면 근처 (Y > -10)
            _targetSurfaceWeight = 1f;
            _targetShallowWeight = 0f;
            _targetMediumWeight = 0f;
            _targetDeepWeight = 0f;
        }
        else if (playerDepth > _shallowDepth)
        {
            // 수면 ↔ 얕은 곳 전환 (-10 ~ -40)
            float t = Mathf.InverseLerp(_surfaceDepth, _surfaceDepth - _blendRange, playerDepth);
            _targetSurfaceWeight = 1f - t;
            _targetShallowWeight = t;
            _targetMediumWeight = 0f;
            _targetDeepWeight = 0f;
        }
        else if (playerDepth > _mediumDepth)
        {
            // 얕은 곳 ↔ 중간 전환 (-40 ~ -100)
            float t = Mathf.InverseLerp(_shallowDepth, _shallowDepth - _blendRange, playerDepth);
            _targetSurfaceWeight = 0f;
            _targetShallowWeight = 1f - t;
            _targetMediumWeight = t;
            _targetDeepWeight = 0f;
        }
        else
        {
            // 중간 ↔ 깊은 곳 전환 (-100 이하)
            float t = Mathf.InverseLerp(_mediumDepth, _mediumDepth - _blendRange, playerDepth);
            _targetSurfaceWeight = 0f;
            _targetShallowWeight = 0f;
            _targetMediumWeight = 1f - t;
            _targetDeepWeight = t;
        }

        // 가중치 적용 (부드러운 전환)
        if (_surfaceVolume != null)
        {
            _surfaceVolume.weight = instant ? _targetSurfaceWeight :
                Mathf.Lerp(_surfaceVolume.weight, _targetSurfaceWeight, Time.deltaTime * _transitionSpeed);
        }

        if (_shallowVolume != null)
        {
            _shallowVolume.weight = instant ? _targetShallowWeight :
                Mathf.Lerp(_shallowVolume.weight, _targetShallowWeight, Time.deltaTime * _transitionSpeed);
        }

        if (_mediumVolume != null)
        {
            _mediumVolume.weight = instant ? _targetMediumWeight :
                Mathf.Lerp(_mediumVolume.weight, _targetMediumWeight, Time.deltaTime * _transitionSpeed);
        }

        if (_deepVolume != null)
        {
            _deepVolume.weight = instant ? _targetDeepWeight :
                Mathf.Lerp(_deepVolume.weight, _targetDeepWeight, Time.deltaTime * _transitionSpeed);
        }
    }

    /// <summary>
    /// 특정 깊이의 가중치를 강제 설정 (테스트용)
    /// </summary>
    public void SetDepthManually(float surface, float shallow, float medium, float deep)
    {
        _targetSurfaceWeight = Mathf.Clamp01(surface);
        _targetShallowWeight = Mathf.Clamp01(shallow);
        _targetMediumWeight = Mathf.Clamp01(medium);
        _targetDeepWeight = Mathf.Clamp01(deep);

        if (_surfaceVolume != null) _surfaceVolume.weight = _targetSurfaceWeight;
        if (_shallowVolume != null) _shallowVolume.weight = _targetShallowWeight;
        if (_mediumVolume != null) _mediumVolume.weight = _targetMediumWeight;
        if (_deepVolume != null) _deepVolume.weight = _targetDeepWeight;
    }

    /// <summary>
    /// 현재 플레이어 깊이 반환 (디버깅용)
    /// </summary>
    public float GetCurrentDepth()
    {
        return _playerTransform != null ? _playerTransform.position.y : 0f;
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        // 깊이 순서 검증
        if (_shallowDepth > _surfaceDepth)
        {
            _shallowDepth = _surfaceDepth - 10f;
        }
        if (_mediumDepth > _shallowDepth)
        {
            _mediumDepth = _shallowDepth - 10f;
        }

        _blendRange = Mathf.Max(1f, _blendRange);
        _transitionSpeed = Mathf.Max(0.1f, _transitionSpeed);
    }

    /// <summary>
    /// Scene 뷰에서 깊이 구간 시각화
    /// </summary>
    void OnDrawGizmosSelected()
    {
        // 수면 구간 (White)
        Gizmos.color = Color.white;
        Gizmos.DrawLine(new Vector3(-100f, _surfaceDepth, 0f), new Vector3(100f, _surfaceDepth, 0f));

        // 얕은 구간 (Cyan)
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(new Vector3(-100f, _shallowDepth, 0f), new Vector3(100f, _shallowDepth, 0f));

        // 중간 구간 (Blue)
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(new Vector3(-100f, _mediumDepth, 0f), new Vector3(100f, _mediumDepth, 0f));

        // 전환 구간 (Yellow)
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(new Vector3(-100f, _surfaceDepth - _blendRange, 0f), new Vector3(100f, _surfaceDepth - _blendRange, 0f));
        Gizmos.DrawLine(new Vector3(-100f, _shallowDepth - _blendRange, 0f), new Vector3(100f, _shallowDepth - _blendRange, 0f));
        Gizmos.DrawLine(new Vector3(-100f, _mediumDepth - _blendRange, 0f), new Vector3(100f, _mediumDepth - _blendRange, 0f));

        // 플레이어 위치 표시
        if (_playerTransform != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(_playerTransform.position, 2f);
        }
    }
#endif
}
