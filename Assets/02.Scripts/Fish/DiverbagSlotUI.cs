using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DiverbagSlotUI : MonoBehaviour
{
    
    [SerializeField] private Image _icon;
    [SerializeField] private TextMeshProUGUI _amountText;
    
    public void Set(ResourceMetaData data, int amount)
    {
        _icon.sprite = data.icon;
        _amountText.text = amount.ToString();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
