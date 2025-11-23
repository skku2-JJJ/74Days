using System;
using UnityEngine;

public class NormalFish : FishBase
{
    private void Awake()
    {
        health = maxHealth;
    }

    public override void TakeHarpoonHit(float damage)
    {
        health = Mathf.Max(0, health - damage);
        
        // TODO : 피격 연출
    }

    public override void Capture()
    {
        // 포획 처리
        // TODO : 인벤토리 추가 ,이펙트 
        Debug.Log($"{name} captured!");
        gameObject.SetActive(false);
    }

    public override void OnCaptureFailed()
    {
        Debug.Log($"{name} escaped!");
    }
}
