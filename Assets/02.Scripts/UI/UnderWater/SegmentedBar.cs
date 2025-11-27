using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

abstract public class SegmentedBar : MonoBehaviour
{
    private Image[] _segments;

    

    private float _currentGauge;
    public float CurrentGauge { get => _currentGauge; set { _currentGauge = Mathf.Clamp(value, 0, 100); } }
    void Start()
    {
        _segments = GetComponentsInChildren<Image>();
        GaugeInit();

    }
    void Update()
    {
        GaugeUpdate();
    }
    public void GaugeUpdate()
    {

        
        CurrentGauge = GetValue();
        float gaugeArrange = CurrentGauge * 0.1f;

        for (int i = 0; i < _segments.Length; i++)
        {
            if (i < gaugeArrange)
            {
                _segments[i].DOColor(Color.white, 1f);
            }else
            {
                _segments[i].DOColor(Color.clear, 1f);
            }       
        }
    }

    public void GaugeInit()
    {
        foreach (var segment in _segments)
        {
            segment.color = Color.white;
        }
    }

    abstract public int GetValue();
    
}
