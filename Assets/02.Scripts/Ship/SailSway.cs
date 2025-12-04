using UnityEngine;

/// <summary>
/// 돛에 바람에 의한 흔들림 효과 추가 (회전 + 위치 진동)
/// </summary>
public class SailSway : MonoBehaviour
{
    [Header("Rotation Settings")]
    [SerializeField] private float _rotationAmountZ = 3f;        // Z축 회전 각도 (좌우 기울기)
    [SerializeField] private float _rotationSpeedZ = 0.6f;       // Z축 회전 속도
    [SerializeField] private float _rotationAmountX = 0.5f;      // X축 회전 각도 (위아래 기울기) - 미세
    [SerializeField] private float _rotationSpeedX = 0.8f;       // X축 회전 속도
    [SerializeField] private float _rotationAmountY = 0.3f;      // Y축 회전 각도 (좌우 넘어감) - 미세
    [SerializeField] private float _rotationSpeedY = 1.0f;       // Y축 회전 속도
    [SerializeField] private bool _enableRotation = true;

    [Header("Position Vibration Settings")]
    [SerializeField] private float _vibrationAmountX = 0.05f;   // X축 진동 크기
    [SerializeField] private float _vibrationAmountY = 0.03f;   // Y축 진동 크기
    [SerializeField] private float _vibrationSpeedX = 1.2f;     // X축 진동 속도
    [SerializeField] private float _vibrationSpeedY = 0.8f;     // Y축 진동 속도
    [SerializeField] private bool _enableVibration = true;

    private Vector3 _initialPosition;
    private Quaternion _initialRotation;

    void Start()
    {
        _initialPosition = transform.localPosition;
        _initialRotation = transform.localRotation;
    }

    void Update()
    {
        // Position 계산
        Vector3 finalPosition = _initialPosition;
        if (_enableVibration)
        {
            float offsetX = Mathf.Sin(Time.time * _vibrationSpeedX) * _vibrationAmountX;
            float offsetY = Mathf.Sin(Time.time * _vibrationSpeedY) * _vibrationAmountY;
            finalPosition += new Vector3(offsetX, offsetY, 0f);
        }

        // Rotation 계산 (X, Y, Z축 모두)
        Quaternion finalRotation = _initialRotation;
        if (_enableRotation)
        {
            float rotationX = Mathf.Sin(Time.time * _rotationSpeedX) * _rotationAmountX;
            float rotationY = Mathf.Sin(Time.time * _rotationSpeedY + 1.5f) * _rotationAmountY; // 위상차 추가
            float rotationZ = Mathf.Sin(Time.time * _rotationSpeedZ) * _rotationAmountZ;
            finalRotation = _initialRotation * Quaternion.Euler(rotationX, rotationY, rotationZ);
        }

        // 최종 적용
        transform.localPosition = finalPosition;
        transform.localRotation = finalRotation;
    }

    /// <summary>
    /// 회전 효과 활성화/비활성화
    /// </summary>
    public void SetRotationEnabled(bool enabled)
    {
        _enableRotation = enabled;

        if (!enabled)
        {
            transform.localRotation = _initialRotation;
        }
    }

    /// <summary>
    /// 진동 효과 활성화/비활성화
    /// </summary>
    public void SetVibrationEnabled(bool enabled)
    {
        _enableVibration = enabled;

        if (!enabled)
        {
            transform.localPosition = _initialPosition;
        }
    }

    /// <summary>
    /// 회전 각도 설정 (Z축)
    /// </summary>
    public void SetRotationAmount(float amount)
    {
        _rotationAmountZ = Mathf.Max(0f, amount);
    }

    /// <summary>
    /// X/Y/Z축 회전 각도 개별 설정
    /// </summary>
    public void SetRotationAmounts(float amountX, float amountY, float amountZ)
    {
        _rotationAmountX = Mathf.Max(0f, amountX);
        _rotationAmountY = Mathf.Max(0f, amountY);
        _rotationAmountZ = Mathf.Max(0f, amountZ);
    }

    /// <summary>
    /// 진동 크기 설정
    /// </summary>
    public void SetVibrationAmount(float amountX, float amountY)
    {
        _vibrationAmountX = Mathf.Max(0f, amountX);
        _vibrationAmountY = Mathf.Max(0f, amountY);
    }

    /// <summary>
    /// 바람 강도 설정 (회전 + 진동 동시 조절)
    /// </summary>
    public void SetWindStrength(float strength)
    {
        // strength: 0 (잔잔) ~ 1 (기본) ~ 2 (강풍)
        _rotationAmountX = 0.5f * strength;
        _rotationAmountY = 0.3f * strength;
        _rotationAmountZ = 3f * strength;
        _rotationSpeedX = 0.8f * strength;
        _rotationSpeedY = 1.0f * strength;
        _rotationSpeedZ = 0.6f * strength;
        _vibrationAmountX = 0.05f * strength;
        _vibrationAmountY = 0.03f * strength;
        _vibrationSpeedX = 1.2f * strength;
        _vibrationSpeedY = 0.8f * strength;
    }

#if UNITY_EDITOR
    void OnValidate()
    {
        _rotationAmountX = Mathf.Clamp(_rotationAmountX, 0f, 100f);
        _rotationAmountY = Mathf.Clamp(_rotationAmountY, 0f, 100f);
        _rotationAmountZ = Mathf.Clamp(_rotationAmountZ, 0f, 100f);
        _rotationSpeedX = Mathf.Max(0.1f, _rotationSpeedX);
        _rotationSpeedY = Mathf.Max(0.1f, _rotationSpeedY);
        _rotationSpeedZ = Mathf.Max(0.1f, _rotationSpeedZ);
        _vibrationAmountX = Mathf.Clamp(_vibrationAmountX, 0f, 100f);
        _vibrationAmountY = Mathf.Clamp(_vibrationAmountY, 0f, 100f);
        _vibrationSpeedX = Mathf.Max(0.1f, _vibrationSpeedX);
        _vibrationSpeedY = Mathf.Max(0.1f, _vibrationSpeedY);
    }
#endif
}
