using TMPro;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public class DayShow : MonoBehaviour
{
    private TextMeshProUGUI _textMeshProUGUI;
    private void Start()
    {
        _textMeshProUGUI = GetComponent<TextMeshProUGUI>();
        if (DayManager.Instance != null ) DayUpdate(DayManager.Instance.CurrentDay);
    }
    public void DayUpdate(int value)
    {
        _textMeshProUGUI.text = $"Day {value}";
    }

}
