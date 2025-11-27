using System;
using Unity.VisualScripting;
using UnityEngine;

public class DiverStatus : MonoBehaviour
{
    [Header("체력")]
    [SerializeField] private float _currentHp = 100f;
    [SerializeField] private float _maxHp = 100f;

    [Header("산소")]
    [SerializeField] private float _currentOxygen = 100f;
    [SerializeField] private float _maxOxygen = 100f;
    [SerializeField] private float _oxygenConsumePerSecond = 1f;

    // 가방
    private Inventory _diveBag  = new Inventory();
    
    // 프로퍼티
    public float MaxHp => _maxHp;
    public float CurrentHp => _currentHp;
    public float MaxOxygen => _maxOxygen;
    public float CurrentOxygen => _currentOxygen;

    public Inventory DiveBag => _diveBag;

   


    private void Update()
    {
        // TODO : 산소 감소
    }

    public void GainResource(ResourceType type, int amount)
    {
        DiveBag.Add(type, amount);
    }


    private void OnCollisionEnter2D(Collision2D other)
    {
        throw new NotImplementedException();
    }
}
