using UnityEngine;

public class O2Bar : SegmentedBar
{
    [SerializeField] private DiverStatus _diver;
    
    public override int GetValue()
    {
        if (_diver == null || _diver.MaxOxygen <= 0) return 0;

        float ratio = (float)_diver.CurrentOxygen / _diver.MaxOxygen;
        int percent = Mathf.RoundToInt(ratio * 100f);

        return percent;
    }
}
