using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DiverbagSlotUI : MonoBehaviour
{
    
    [SerializeField] private Image _icon;
    [SerializeField] private TextMeshProUGUI _amountText;
    
    public void Set(ResourceData data, int amount)
    {
        _icon.sprite = data.Icon;
        _amountText.text = amount.ToString();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
