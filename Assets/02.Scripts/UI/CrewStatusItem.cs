using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CrewStatusItem : MonoBehaviour
{
    [Header("Basic Info")]
    [SerializeField] private TextMeshProUGUI _nameText;
    [SerializeField] private TextMeshProUGUI _statusText;

    [Header("Vital Gauges")]
    [SerializeField] private Slider _hungerSlider;
    [SerializeField] private Slider _thirstSlider;
    [SerializeField] private Slider _temperatureSlider;

    [Header("Visual")]
    [SerializeField] private Image _backgroundImage;

    [Header("Status Colors")]
    [SerializeField] private Color _healthyColor = new Color(0.2f, 0.8f, 0.2f, 0.3f);   // 초록
    [SerializeField] private Color _poorColor = new Color(0.9f, 0.7f, 0.1f, 0.3f);      // 노랑
    [SerializeField] private Color _criticalColor = new Color(0.9f, 0.2f, 0.2f, 0.3f);  // 빨강
    [SerializeField] private Color _deadColor = new Color(0.3f, 0.3f, 0.3f, 0.5f);      // 회색

    private CrewMember _currentCrew;

    // 선원 데이터 설정
    public void SetCrewData(CrewMember crew)
    {
        _currentCrew = crew;
        UpdateDisplay();
    }

    // UI 표시 갱신
    public void UpdateDisplay()
    {
        if (_currentCrew == null) return;

        // 이름 표시
        if (_nameText != null)
        {
            _nameText.text = _currentCrew.CrewName;
        }

        // 상태 텍스트 표시
        if (_statusText != null)
        {
            _statusText.text = GetStatusText(_currentCrew.Status);
        }

        // 배고픔 게이지
        if (_hungerSlider != null)
        {
            _hungerSlider.maxValue = 100f;
            _hungerSlider.value = _currentCrew.Hunger;
        }

        // 갈증 게이지
        if (_thirstSlider != null)
        {
            _thirstSlider.maxValue = 100f;
            _thirstSlider.value = _currentCrew.Thirst;
        }

        // 체온 게이지
        if (_temperatureSlider != null)
        {
            _temperatureSlider.maxValue = 100f;
            _temperatureSlider.value = _currentCrew.Temperature;
        }

        // 배경색 설정 (상태에 따라)
        if (_backgroundImage != null)
        {
            _backgroundImage.color = GetStatusColor(_currentCrew.Status);
        }
    }

    // 상태에 따른 텍스트 반환
    private string GetStatusText(CrewStatus status)
    {
        switch (status)
        {
            case CrewStatus.Healthy:
                return "양호";
            case CrewStatus.Poor:
                return "주의";
            case CrewStatus.Critical:
                return "위험!";
            case CrewStatus.Dead:
                return "사망";
            default:
                return "???";
        }
    }

    // 상태에 따른 색상 반환
    private Color GetStatusColor(CrewStatus status)
    {
        switch (status)
        {
            case CrewStatus.Healthy:
                return _healthyColor;
            case CrewStatus.Poor:
                return _poorColor;
            case CrewStatus.Critical:
                return _criticalColor;
            case CrewStatus.Dead:
                return _deadColor;
            default:
                return Color.white;
        }
    }
}
