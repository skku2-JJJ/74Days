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

    //test용 코드
    public float _health = 0;

    void Update()
    {
        //test용 코드
        if (Input.GetKeyUp(KeyCode.S))
        {
            HpSliderUpdate(_health);
            Debug.Log(_health);
        }
    }

    public void HpSliderUpdate(float hp)
    {
        float currentValue = _hpSlider.value;

        DOTween.To(() => _hpSlider.value, x => _hpSlider.value = x, hp * 0.01f, _dotweenDuration);
    }

    public void WaterSliderUpdate(float water) 
    {
        float currentValue = _waterSlider.value;

        DOTween.To(() => _waterSlider.value, x => _waterSlider.value = x, water * 0.01f, _dotweenDuration);
    }

    public void WarmSliderUpdate(float warm)
    {
        float currentValue = _warmSlider.value;

        DOTween.To(() => _warmSlider.value, x => _waterSlider.value = x, warm * 0.01f, _dotweenDuration);
    }
}
