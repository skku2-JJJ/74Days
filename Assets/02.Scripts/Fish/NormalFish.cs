using System;
using UnityEngine;

public class NormalFish : FishBase
{
    
    private void Awake()
    {
       Init();
    }

    private void Init()
    {
        health = maxHealth;
        
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        originalScale = transform.localScale;
    }
    public override void TakeHarpoonHit(float damage,Vector2 harpoonDir)
    {
        health = Mathf.Max(0, health - damage);
        
        ApplyHitVisualEffect();
        ApplyHitPhysics(harpoonDir);
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
