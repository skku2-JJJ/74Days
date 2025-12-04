using System.Collections;
using UnityEngine;

public class FocusSoundControl : MonoBehaviour
{
    [SerializeField] private AudioLowPassFilter _lpf;
    [SerializeField] private float _normalCutoff = 22000f;
    [SerializeField] private float _focusCutoff = 800f;
    [SerializeField] private float _fadeTime = 0.2f;
    private Coroutine _co;

    public void SetFocus(bool on)
    {
        if (_co != null) StopCoroutine(_co);
        _co = StartCoroutine(FadeLPF(on ? _focusCutoff : _normalCutoff));
    }

    private IEnumerator FadeLPF(float target)
    {
        float start = _lpf.cutoffFrequency;
        float t = 0f;
        while (t < _fadeTime)
        {
            t += Time.deltaTime;
            _lpf.cutoffFrequency = Mathf.Lerp(start, target, t / _fadeTime);
            yield return null;
        }
        _lpf.cutoffFrequency = target;
    }
}
