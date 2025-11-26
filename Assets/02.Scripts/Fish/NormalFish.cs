using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class NormalFish : FishBase
{
    private const float RandomVecFactor = 0.2f;
    private void Awake()
    {
       Init();
    }
    
    private void Update()
    {
        if (isCapturedByHarpoon)
        {
            UpdateCapturedMovement();
            UpdateCapturedWiggle();
        }
    }
    

    private void Init()
    {
        health = maxHealth;
        rigid = GetComponent<Rigidbody2D>();
        animator = GetComponentInChildren<Animator>();
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
        Vector2 dir = Vector2.zero;
        if (capturedDiver != null)
        {
            Vector2 inverseVec = ((Vector2)transform.position - (Vector2)capturedDiver.position).normalized;
            Vector3 randomVec = Random.insideUnitCircle * RandomVecFactor;
            dir =(inverseVec +  (Vector2)randomVec).normalized;
        }
        else
        {
            dir = UnityEngine.Random.insideUnitCircle.normalized;
        }

        float escapeForce = escapeMoveSpeed * 2.0f; 
        rigid.linearVelocity = dir * escapeForce;
        
    }

    
}
