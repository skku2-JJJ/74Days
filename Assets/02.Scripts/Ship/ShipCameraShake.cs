using UnityEngine;

/// <summary>
/// Ship 씬 카메라에 미묘한 흔들림 효과 추가 (파도 느낌)
/// </summary>
public class ShipCameraShake : MonoBehaviour
{
    [Header("Shake Settings")]
    [SerializeField] private float _shakeIntensity = 0.03f;  // 흔들림 강도
    [SerializeField] private float _shakeSpeed = 0.5f;       // 흔들림 속도 (낮을수록 부드러움)
    [SerializeField] private bool _enableShake = true;       // 흔들림 활성화

    private Vector3 _initialPosition;

    void Start()
    {
        // 초기 위치 저장
        _initialPosition = transform.localPosition;
    }

    void Update()
    {
        if (!_enableShake) return;

        // Perlin Noise로 부드러운 랜덤 움직임 생성
        float time = Time.time * _shakeSpeed;

        float offsetX = (Mathf.PerlinNoise(time, 0f) * 2f - 1f) * _shakeIntensity;
        float offsetY = (Mathf.PerlinNoise(0f, time) * 2f - 1f) * _shakeIntensity;

        // 카메라 위치 업데이트
        transform.localPosition = _initialPosition + new Vector3(offsetX, offsetY, 0f);
    }

    /// <summary>
    /// 흔들림 활성화/비활성화
    /// </summary>
    public void SetShakeEnabled(bool enabled)
    {
        _enableShake = enabled;

        // 비활성화 시 원래 위치로 복귀
        if (!enabled)
        {
            transform.localPosition = _initialPosition;
        }
    }

    /// <summary>
    /// 흔들림 강도 설정
    /// </summary>
    public void SetShakeIntensity(float intensity)
    {
        _shakeIntensity = Mathf.Max(0f, intensity);
    }

    /// <summary>
    /// 일시적인 강한 흔들림 (충격 효과)
    /// </summary>
    public void ShakeOnce(float intensity, float duration)
    {
        StartCoroutine(ShakeCoroutine(intensity, duration));
    }

    private System.Collections.IEnumerator ShakeCoroutine(float intensity, float duration)
    {
        float elapsed = 0f;
        float originalIntensity = _shakeIntensity;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            // 시간에 따라 강도 감소
            float currentIntensity = Mathf.Lerp(intensity, 0f, elapsed / duration);

            float offsetX = Random.Range(-1f, 1f) * currentIntensity;
            float offsetY = Random.Range(-1f, 1f) * currentIntensity;

            transform.localPosition = _initialPosition + new Vector3(offsetX, offsetY, 0f);

            yield return null;
        }

        // 원래 강도로 복구
        _shakeIntensity = originalIntensity;
        transform.localPosition = _initialPosition;
    }

    /// <summary>
    /// 초기 위치 재설정 (카메라 위치 변경 시)
    /// </summary>
    public void ResetInitialPosition()
    {
        _initialPosition = transform.localPosition;
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        // Inspector에서 값 변경 시 실시간 반영
        _shakeIntensity = Mathf.Max(0f, _shakeIntensity);
        _shakeSpeed = Mathf.Max(0.1f, _shakeSpeed);
    }
#endif
}