using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class NormalFish : FishBase
{
    [Header("Escape Settings")]
    [SerializeField] private float _escapeBurstSpeed = 8f;   // QTE 실패 시 한번에 튀는 속도
    [SerializeField] private float _escapeRandomFactor = 0.2f; // 방향 랜덤 섞기 정도
    
    private Rigidbody2D _rigid;
    
    private void Awake()
    {
       Init();
    }
    

    private void Init()
    {
        _rigid = GetComponent<Rigidbody2D>();
        diver = GameObject.FindGameObjectWithTag("Player").transform;
        
    }

    public override void OnCapture()
    {
        _rigid.simulated = false;
    }

    public override void OnCaptureFailed()
    {
        Vector2 dir = Vector2.zero;

        if (diver != null)
        {
            Vector2 inverseVec = ((Vector2)transform.position - (Vector2)diver.position).normalized;
            Vector2 randomVec = Random.insideUnitCircle * _escapeRandomFactor;
            dir = (inverseVec + randomVec).normalized;
        }
        else
        {
            dir = Random.insideUnitCircle.normalized;
        }
        
        float speed = _escapeBurstSpeed;
        _rigid.linearVelocity = dir * speed;
        
    }
    
}
