using TMPro;
using UnityEngine;

public class DiverbagUI : MonoBehaviour
{
    [SerializeField] private DiverStatus _diverStatus;
    [SerializeField] private DiverbagSlotUI[] _slots;
    [SerializeField] private ResourceDatabase _db;

    private void OnEnable()
    {
        Refresh();
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

            var data = _db.Get(type);
            _slots[i].gameObject.SetActive(true);
            _slots[i].Set(data, amount);
            i++;
        }

        /*// 남은 슬롯들은 숨김
        for (; i < _slots.Length; i++)
        {
            _slots[i].Hide();
        }*/
    }
}
