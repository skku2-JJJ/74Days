using TMPro;
using UnityEngine;

public class FixAmountText : MonoBehaviour
{
    private TextMeshProUGUI _textMeshPro;
    

    void Awake()
    {
        _textMeshPro = GetComponent<TextMeshProUGUI>();
    }
    public void AmountTextUpdate(int amount)
    {
        _textMeshPro.text = $"{amount}";
    }
}
