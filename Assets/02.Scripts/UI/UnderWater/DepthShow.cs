using TMPro;
using UnityEngine;

public class DepthShow : MonoBehaviour
{
    private TextMeshProUGUI _textMeshProUGUI;
    private void Start()
    {
        _textMeshProUGUI = GetComponent<TextMeshProUGUI>();
    }
    public void DepthUpdate(int value)
    {
        _textMeshProUGUI.text = $"value";
    }
}
