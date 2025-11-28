using System;
using TMPro;
using UnityEngine;

public class DiverbagUI : MonoBehaviour
{
    [Header("참조")]
    [SerializeField] private DiverStatus _diverStatus;
    private ResourceDatabase _resourceDatabase;
   
    
    [Header("Slots")]
    [SerializeField] private DiverbagSlotUI[] _slots;


    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        _resourceDatabase = GetComponent<ResourceDatabase>();
    }

    public void Refresh()
    {
        var items = _diverStatus.DiveBag.Items;

        int i = 0;

        // 현재 보유 아이템만큼 슬롯 업데이트
        foreach (var kvp in items)
        {
            if (i >= _slots.Length) break;  // 슬롯보다 아이템 많으면 그냥 자름

            var type = kvp.Key;
            var amount = kvp.Value;

            var data = _resourceDatabase.Get(type);
            _slots[i].gameObject.SetActive(true);
            _slots[i].Set(data, amount);
            i++;
        }
        
    }
}
