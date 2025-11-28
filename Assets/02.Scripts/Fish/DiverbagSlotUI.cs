using UnityEngine;
using UnityEngine.UI;

public class DiverbagSlotUI : MonoBehaviour
{
    
    [SerializeField] private Image _icon;
    [SerializeField] private Text _nameText;
    [SerializeField] private Text _amountText;

    // TODO : Text ->  TextMeshProUGUI
    
    public void Set(ResourceData data, int amount)
    {
        _icon.sprite = data.Icon;
        _nameText.text = data.DisplayName;
        _amountText.text = amount.ToString();
    }

    public void Hide()
    {
        gameObject.SetActive(false);
    }
}
