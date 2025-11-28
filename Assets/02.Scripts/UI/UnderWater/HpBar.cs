using UnityEngine;

public class HpBar : SegmentedBar
{
    [SerializeField] private DiverStatus _diver;
    
    public override int GetValue()
    {
        if (_diver == null || _diver.MaxHp <= 0) return 0;
        
        float ratio = (float)_diver.CurrentHp / _diver.MaxHp; 
        int percent = Mathf.RoundToInt(ratio * 100f);         

        return percent;
    }
}
