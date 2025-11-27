using UnityEngine;

public class O2Bar : SegmentedBar
{
    public int testO2 = 100;
    public override int GetValue()
    {
        //현재체력을 받아온다
        //_currentHP = Player.Instance.HP ~
        //test용
        return testO2;
    }
}
