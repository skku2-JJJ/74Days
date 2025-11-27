using UnityEngine;

public class HpBar : SegmentedBar
{
    public int testHP = 100;
    public override int GetValue()
    {
        //현재체력을 받아온다
        //_currentHP = Player.Instance.HP ~
        //test용
        return testHP;
    }
}
