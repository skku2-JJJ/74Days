using System.Collections;
using DG.Tweening;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    public static CameraShake Instance { get; private set; }

    [Header("셰이크 타겟")]
    [SerializeField] private Transform _shakeTarget; // Cinemachine VCam or Main Camera

    [Header("원샷 셰이크 기본값")]
    [SerializeField] private float _baseOneShotDuration = 0.15f;
    [SerializeField] private float _baseOneShotStrength = 0.4f;
    [SerializeField] private int _oneShotVibrato = 20;
    [SerializeField] private float _oneShotRandomness = 90f;

    [Header("연속 셰이크 기본값 (QTE용)")]
    [SerializeField] private float _continuousBaseStrength = 0.15f;
    [SerializeField] private float _continuousDuration = 0.25f;
    [SerializeField] private int _continuousVibrato = 10;
    [SerializeField] private float _continuousRandomness = 45f;

    private Vector3 _originalLocalPos;
    private Tweener _oneShotTween;
    private Coroutine _continuousRoutine;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (_shakeTarget == null)
        {
            _shakeTarget = transform;
        }

        _originalLocalPos = _shakeTarget.localPosition;
    }

    private void OnDisable()
    {
        _oneShotTween?.Kill();
        if (_continuousRoutine != null)
        {
            StopCoroutine(_continuousRoutine);
            _continuousRoutine = null;
        }

        if (_shakeTarget != null)
        {
            _shakeTarget.localPosition = _originalLocalPos;
        }
    }

    /// <summary>
    /// 한 번 퍽! 하고 튀기는 셰이크 (발사/히트/성공/실패용)
    /// intensity = 1 기준, 값 키우면 더 세게
    /// </summary>
    public void ShakeOneShot(float intensity = 1f)
    {
        if (_shakeTarget == null) return;

        _oneShotTween?.Kill();
        _shakeTarget.localPosition = _originalLocalPos;

        float strength = _baseOneShotStrength * intensity;

        _oneShotTween = _shakeTarget.DOShakePosition(
            _baseOneShotDuration,
            strength,
            _oneShotVibrato,
            _oneShotRandomness,
            snapping: false,
            fadeOut: true
        ).OnComplete(() =>
        {
            if (_continuousRoutine == null) // 연속 셰이크 없는 상태에서만 원위치
            {
                _shakeTarget.localPosition = _originalLocalPos;
            }
        });
    }

    /// <summary>
    /// QTE 동안 계속 미세하게 출렁이는 셰이크 시작
    /// intensity = 1 기준, 값 키우면 더 세게
    /// </summary>
    public void StartContinuous(float intensity = 1f)
    {
        if (_shakeTarget == null) return;

        if (_continuousRoutine != null)
        {
            StopCoroutine(_continuousRoutine);
        }
        _continuousRoutine = StartCoroutine(ContinuousShakeRoutine(intensity));
    }

    /// <summary>
    /// 연속 셰이크 정지
    /// </summary>
    public void StopContinuous()
    {
        if (_continuousRoutine != null)
        {
            StopCoroutine(_continuousRoutine);
            _continuousRoutine = null;
        }

        _oneShotTween?.Kill();
        _oneShotTween = null;

        if (_shakeTarget != null)
        {
            _shakeTarget.localPosition = _originalLocalPos;
        }
    }

    private IEnumerator ContinuousShakeRoutine(float intensity)
    {
        float strength = _continuousBaseStrength * intensity;

        while (true)
        {
            // 한 번 흔들고
            _shakeTarget.localPosition = _originalLocalPos;

            Tweener t = _shakeTarget.DOShakePosition(
                _continuousDuration,
                strength,
                _continuousVibrato,
                _continuousRandomness,
                snapping: false,
                fadeOut: true
            );

            yield return t.WaitForCompletion();
        }
    }
}
