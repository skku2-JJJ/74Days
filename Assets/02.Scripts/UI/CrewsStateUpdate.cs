using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class CrewsStateUpdate : MonoBehaviour
{
    [SerializeField]
    private Slider _hpSlider;

    [SerializeField]
    private Slider _waterSlider;

    [SerializeField]
    private Slider _warmSlider;

    

    private float _dotweenDuration = 1f;

    void Update()
    {
    }

    public void HpSliderUpdate(float hp)
    {
        if (_hpSlider == null)
        {
            Debug.LogWarning("[CrewsStateUpdate] _hpSlider가 null입니다!");
            return;
        }

        float targetValue = hp * 0.01f; // 0~100 → 0~1
        Debug.Log($"[HpSlider] 현재값: {_hpSlider.value}, 목표값: {targetValue} (원본: {hp})");

        DOTween.To(() => _hpSlider.value, x => _hpSlider.value = x, targetValue, _dotweenDuration);
    }

    public void WaterSliderUpdate(float water)
    {
        if (_waterSlider == null)
        {
            Debug.LogWarning("[CrewsStateUpdate] _waterSlider가 null입니다!");
            return;
        }

        float targetValue = water * 0.01f; // 0~100 → 0~1
        Debug.Log($"[WaterSlider] 현재값: {_waterSlider.value}, 목표값: {targetValue} (원본: {water})");

        DOTween.To(() => _waterSlider.value, x => _waterSlider.value = x, targetValue, _dotweenDuration);
    }

    public void WarmSliderUpdate(float warm)
    {
        if (_warmSlider == null)
        {
            Debug.LogWarning("[CrewsStateUpdate] _warmSlider가 null입니다!");
            return;
        }

        float targetValue = warm * 0.01f; // 0~100 → 0~1
        Debug.Log($"[WarmSlider] 현재값: {_warmSlider.value}, 목표값: {targetValue} (원본: {warm})");

        DOTween.To(() => _warmSlider.value, x => _warmSlider.value = x, targetValue, _dotweenDuration);
    }
}
