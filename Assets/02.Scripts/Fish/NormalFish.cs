using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class NormalFish : FishBase
{
    [SerializeField] private float _escapeForceMultiplier = 2f;
    [SerializeField] private float _randomVecFactor = 0.2f;
    
    private Rigidbody2D _rigid;
    
    private void Awake()
    {
       Init();
    }
    

    private void Init()
    {
        _rigid = GetComponent<Rigidbody2D>();
        
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
            Vector3 randomVec = Random.insideUnitCircle * _randomVecFactor;
            dir = (inverseVec + (Vector2)randomVec).normalized;
        }
        else
        {
            dir = UnityEngine.Random.insideUnitCircle.normalized;
        }

        float escapeForce = _escapeForceMultiplier;
        _rigid.linearVelocity = dir * escapeForce;
        
    }
    
}
