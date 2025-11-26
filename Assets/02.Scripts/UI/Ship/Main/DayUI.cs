using TMPro;
using UnityEngine;

public class DayUI : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI _dayCountText;

    void Start()
    {
        // DayManager 이벤트 구독
        if (DayManager.Instance != null)
        {
            DayManager.Instance.OnDayStart += DayUpdate;

            // 초기 날짜 표시
            DayUpdate(DayManager.Instance.CurrentDay);
        }
    }

    void OnDestroy()
    {
        // 이벤트 구독 해제
        if (DayManager.Instance != null)
        {
            DayManager.Instance.OnDayStart -= DayUpdate;
        }
    }

    public void DayUpdate(int day)
    {
        if (_dayCountText != null)
        {
            _dayCountText.text = $"{day}";
        }
    }
}
