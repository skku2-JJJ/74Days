using TMPro;
using UnityEngine;

public class DayUI : MonoBehaviour
{
    public int Day = 10;
    
    [SerializeField]
    private TextMeshProUGUI _dayCountText;
    void Start()
    {
        DayUpdate(Day);
    }

    
    public void DayUpdate(int day)
    {
        _dayCountText.text = $"{day}";
    }
}
