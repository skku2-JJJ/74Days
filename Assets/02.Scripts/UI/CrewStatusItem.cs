using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CrewStatusItem : MonoBehaviour
{
    [Header("Basic Info")]
    [SerializeField] private TextMeshProUGUI _nameTextUI;
    [SerializeField] private TextMeshProUGUI _statusTextUI;

    [Header("Vital Gauges")]
    [SerializeField] private Slider _hungerSliderUI;
    [SerializeField] private Slider _thirstSliderUI;
    [SerializeField] private Slider _temperatureSliderUI;

    [Header("Visual")]
    [SerializeField] private Image _backgroundImageUI;

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
        if (_nameTextUI != null)
        {
            _nameTextUI.text = _currentCrew.CrewName;
        }

        // 상태 텍스트 표시
        if (_statusTextUI != null)
        {
            _statusTextUI.text = GetStatusText(_currentCrew.Status);
        }

        // 배고픔 게이지
        if (_hungerSliderUI != null)
        {
            _hungerSliderUI.maxValue = 100f;
            _hungerSliderUI.value = _currentCrew.Hunger;
        }

        // 갈증 게이지
        if (_thirstSliderUI != null)
        {
            _thirstSliderUI.maxValue = 100f;
            _thirstSliderUI.value = _currentCrew.Thirst;
        }

        // 체온 게이지
        if (_temperatureSliderUI != null)
        {
            _temperatureSliderUI.maxValue = 100f;
            _temperatureSliderUI.value = _currentCrew.Temperature;
        }

        // 배경색 설정 (상태에 따라)
        if (_backgroundImageUI != null)
        {
            _backgroundImageUI.color = GetStatusColor(_currentCrew.Status);
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
