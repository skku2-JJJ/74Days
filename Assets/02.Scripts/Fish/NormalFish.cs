using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class NormalFish : FishBase
{
    [Header("Escape Settings")]
    [SerializeField] private float _escapeBurstSpeed = 15f;   
    [SerializeField] private float _escapeBurstDuration = 3f;
    [SerializeField] private float _escapeRandomFactor = 0.2f; 
    
    
    private Rigidbody2D _rigid;
    private FishMoveController _moveController;
    
    private void Awake()
    {
       Init();
    }
    

    private void Init()
    {
        _rigid = GetComponent<Rigidbody2D>();
        _moveController = GetComponent<FishMoveController>();
        diver = GameObject.FindGameObjectWithTag("Player").transform;

        if (visualController == null)
        {
            visualController = GetComponentInChildren<FishVisualController>();
        }
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
        
        _moveController.PlayBurst(dir, _escapeBurstSpeed, _escapeBurstDuration);
        
    }
    
}
